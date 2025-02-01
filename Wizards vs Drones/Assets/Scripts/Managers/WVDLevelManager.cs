using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WVDLevelManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    int _level;
    [SerializeField]
    WVDPlayer _playerScript;
    [SerializeField]
    TMP_Text _levelUI;
    [SerializeField]
    TMP_Text _bossTextUI;

    [Header("Boss")]
    [SerializeField]
    WVDBossCutsceneManager _bossCutsceneManagerScript;
    [SerializeField]
    float _bossCutsceneTriggerDelay;
    [SerializeField]
    WVDMusicManager _musicManagerScript;

    [Header("Spawning")]
    [SerializeField]
    WVDDroneSpawner _droneSpawnerScript;
    [SerializeField]
    WVDPowerUpSpawner _powerUpSpawnerScript;

    [Header("Shop")]
    [SerializeField]
    bool _shopOpen;
    [SerializeField]
    GameObject _greatHallShop;
    [SerializeField]
    GameObject _towerShop;
    //[SerializeField]
    //GameObject _battlementsShop;
    [SerializeField]
    GameObject _dungeonShop;
    [SerializeField]
    List<GameObject> _availableShops; // starts off with Courtyard only
    [SerializeField]
    float _shopTime;
    float _shopTimer;
    int _lastShopTimerValue; // optimization so don't have to format strings every frame
    GameObject _chosenShop;
    [SerializeField]
    GameObject _shopUI;
    bool _shopTrailCoroutineRunning;
    [SerializeField]
    float _shopTrailAnimationDistGap;
    [SerializeField]
    float _shopTrailAnimationTimeGap;
    [SerializeField]
    float _playerToShopTrailThreshold;
    [SerializeField]
    List<GameObject> _trails;
    int _trailCounter = 0;
    Coroutine _playerToShopCoroutine;
    [SerializeField]
    Slider _skipShopBarSlider;
    [SerializeField]
    GameObject _skipShopGameObject;
    float _skipShopProgress;
    readonly float _skipShopProgressComplete = 1.0f; // seconds need to hold down to skip
    readonly float _skippedShopTimeRemaining = 5.0f;
    WVDTutorialManager _tutorialManager;
    [SerializeField]
    TMP_Text _dronesAndShopTimerUI;

    [Header("Unlocking Sections")]
    [SerializeField]
    GameObject _greatHallBarrier;
    [SerializeField]
    GameObject _towerBarrier;
    //[SerializeField]
    //GameObject _battlementsBarrier;
    [SerializeField]
    GameObject _dungeonBarrier;
    List<UnlockableSections> _lockedSections = new List<UnlockableSections> { UnlockableSections.GreatHall, UnlockableSections.Tower, UnlockableSections.Dungeon };

    public float SkipShopProgress 
    { 
        get => _skipShopProgress;
        set 
        {
            _skipShopProgress = value;
            _skipShopProgress = Mathf.Min(_skipShopProgress, _skipShopProgressComplete);
            _skipShopBarSlider.value = _skipShopProgress / _skipShopProgressComplete;
        } 
    }

    public int Level 
    { 
        get => _level; 
        set => _level = value;
    }

    void Start()
    {
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<WVDTutorialManager>();
        _shopTimer = _shopTime;
        _musicManagerScript.InitialMusicSetup();
        WVDFunctionsCheck.SetToDefault();
        WVDEventBus.Subscribe<WVDEventDataLevelComplete>(LevelCompleted);
        StartNewLevel();
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B)) // debug way to skip to boss level
        //{
        //    _level = 7;
        //}
        if (_shopOpen)
        {
            if (Vector3.Distance(_playerScript.gameObject.transform.position, _chosenShop.transform.position) > _playerToShopTrailThreshold)
            {
                ShowShopTrail();
            }

            if (_shopTimer < 0.0f)
            {
                _shopTimer = _shopTime;
                _shopOpen = false;
                StartNewLevel();
            }
            else
            {
                if ((int)_shopTimer != _lastShopTimerValue) // update UI if UI needs to be updated
                {
                    _dronesAndShopTimerUI.text = "Shop time remaining: " + (int)_shopTimer;
                    _lastShopTimerValue = (int)_shopTimer;
                }

                _shopTimer -= Time.deltaTime;
            }

            HandleShopSkip();
        }
    }
    void ShowShopTrail()
    {
        if (!_shopTrailCoroutineRunning)
        {
            _shopTrailCoroutineRunning = true;
            _playerToShopCoroutine = StartCoroutine(TrailToShopAnimation());
        }
    }
    void HandleShopSkip()
    {
        if (_skipShopGameObject.activeSelf)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                SkipShopProgress += Time.deltaTime;

                if (_skipShopProgress == _skipShopProgressComplete)
                {
                    _skipShopGameObject.SetActive(false);
                    _shopTimer = Mathf.Min(_shopTimer, _skippedShopTimeRemaining);
                }
            }
            else
            {
                SkipShopProgress = 0.0f;
            }
        }
    }
    void StartNewLevel()
    {
        _level++;
        _chosenShop?.SetActive(false);
        _chosenShop = null;
        _shopUI.SetActive(false);
        _skipShopGameObject.SetActive(false);
        if (_playerToShopCoroutine != null)
        {
            _shopTrailCoroutineRunning = false;
            StopCoroutine(_playerToShopCoroutine);
        }
        // Hide remaining trails
        foreach (GameObject trail in _trails)
        {
            trail.SetActive(false);
        }

        // Should unlock new section on levels 3, 5 and 7 (2, 4, 6 in code)
        if (_level <= 6 &&
            _level >= 2 &&
            _level % 2 == 0
            )
        {
            AddNewSection();
        }
        // Should unlock new power up on levels 2, 4, 6 and 8 (1, 3, 5 and 7 in code, won't add any more if there aren't any more to add)
        if (_level % 2 == 1)
        {
            _powerUpSpawnerScript.AddRandomColouredPowerUpToList();
        }
        // Play new area tutorial tip on 2nd level
        if (_level == 1)
        {
            WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.NewAreas, 1.0f));
        }

        if (_level < 8) // Normal levels
        {
            StartRegularLevel();
        }
        else if (_level == 8) // Level '9' i.e. boss level
        {
            StartBossLevel();
        }
    }
    void AddNewSection()
    {
        int randIndex = Random.Range(0, _lockedSections.Count);
        UnlockableSections chosenSection = _lockedSections[randIndex];
        print("Unlocking the :" + chosenSection);

        switch (chosenSection)
        {
            case UnlockableSections.GreatHall:
                _greatHallBarrier.SetActive(false);
                _availableShops.Add(_greatHallShop);
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.GreatHall, 2.0f));
                break;
            case UnlockableSections.Tower:
                _towerBarrier.SetActive(false);
                _availableShops.Add(_towerShop);
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.Library, 2.0f));
                break;
            //case UnlockableSections.Battlements:
            //    _battlementsBarrier.SetActive(false);
            //    _availableShops.Add(_battlementsShop);
            //    break;
            case UnlockableSections.Dungeon:
                _dungeonBarrier.SetActive(false);
                _availableShops.Add(_dungeonShop);
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.Dungeon, 2.0f));
                break;
        }

        _droneSpawnerScript.AddSpawnPositionsToListFromSection(chosenSection);
        _powerUpSpawnerScript.AddSpawnPositionsToListFromSection(chosenSection);
        //_powerUpSpawnerScript.MaxPowerUpsSpawned += 0; // possibly increase the max amount that can be spawned as the map is now larger
        _lockedSections.Remove(_lockedSections[randIndex]);

    }
    void StartRegularLevel()
    {
        _droneSpawnerScript.CreatePoolForLevel(_level);
        _droneSpawnerScript.Spawning = true;
        _levelUI.text = $"Level: {_level + 1} /8";
        _musicManagerScript.FadeCurrentMusicOutAndNewMusicIn(_musicManagerScript.PickNewRandomCombatMusicClip());
        if (_level > 0) // i.e. start spawning power ups from level 2
        {
            _powerUpSpawnerScript.Spawning = true;
        }
    }
    void StartBossLevel()
    {
        Invoke("StartBossCutscene", _bossCutsceneTriggerDelay);
        _levelUI.gameObject.SetActive(false);
        _dronesAndShopTimerUI.gameObject.SetActive(false);
        _bossTextUI.gameObject.SetActive(true);
        _musicManagerScript.FadeCurrentMusicOutAndBossMusicIn();
    }
    void StartBossCutscene()
    {
        _bossCutsceneManagerScript.StartBossCutscene();
    }
    public void LevelCompleted(WVDEventDataLevelComplete data)
    {
        _shopOpen = true;
        _chosenShop = _availableShops[Random.Range(0, _availableShops.Count)];
        _chosenShop.SetActive(true);
        print($"Chosen shop: {_chosenShop.name}");

        foreach (WVDPowerUp powerUp in _powerUpSpawnerScript.SpawnedPowerUps)
        {
            _powerUpSpawnerScript.AvailableSpawnPositions.Add(powerUp.SpawnedTransform);
            Destroy(powerUp.gameObject);
        }
        _powerUpSpawnerScript.CurrentPowerUpsSpawned = 0;
        _powerUpSpawnerScript.TomeSpawned = false;
        _powerUpSpawnerScript.SpawnedPowerUps.Clear();
        _powerUpSpawnerScript.Spawning = false;
        _droneSpawnerScript.TriggeredHelpUIThisLevel = false;
        _playerScript.CurrentHealth = _playerScript.MaxHealth;
        _skipShopGameObject.SetActive(true);
        _musicManagerScript.FadeCurrentMusicOutAndNewMusicIn(_musicManagerScript.PickOtherShopMusicClip());
        if (_level == 0) // First level done, show shop tutorial tip
        {
            WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.Shop, 1.0f));
        }
    }
    IEnumerator TrailToShopAnimation()
    {
        List<Vector3> pointsForThisAnimation = GetTrailWayPoints();
        print("Points in path: " + pointsForThisAnimation.Count);
        if (pointsForThisAnimation.Count == 0)
        {
            _shopTrailCoroutineRunning = false;
            _playerToShopCoroutine = null;
            yield break;
        }
        Vector3 startingPos = pointsForThisAnimation[0];
        int i = 0;
        while (i < pointsForThisAnimation.Count - 1)
        {
            Vector3 directionToNextPoint = (pointsForThisAnimation[i + 1] - startingPos).normalized;
            startingPos += directionToNextPoint * _shopTrailAnimationDistGap;
            _trails[_trailCounter].transform.position = startingPos;
            _trails[_trailCounter].SetActive(true);
            if (_trailCounter == _trails.Count - 1)
            {
                _trailCounter = 0;
            }
            else
            {
                _trailCounter++;
            }
            if (Vector3.Distance(startingPos, pointsForThisAnimation[i + 1]) <= _shopTrailAnimationDistGap)
            {
                i++;
            }
            yield return new WaitForSeconds(_shopTrailAnimationTimeGap);
        }
        _shopTrailCoroutineRunning = false;
        _playerToShopCoroutine = null;
    }
    List<Vector3> GetTrailWayPoints()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(_playerScript.gameObject.transform.position, _chosenShop.transform.position, ~3, path); // 3 is the trail mask nav mesh area
        List<Vector3> pointsForThisAnimation = new List<Vector3>();
        foreach (Vector3 point in path.corners)
        {
            pointsForThisAnimation.Add(new Vector3(point.x, point.y + 0.8f, point.z)); // adding vertical offset
        }

        return pointsForThisAnimation;
    }
    public enum UnlockableSections
    {
        GreatHall,
        Tower,
        Battlements, // Unused section
        Dungeon
    }
}