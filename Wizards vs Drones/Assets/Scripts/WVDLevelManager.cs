using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WVDLevelManager : MonoBehaviour
{
    [SerializeField]
    int _level;
    [SerializeField]
    WVDDroneSpawner _droneSpawnerScript;
    [SerializeField]
    WVDPowerUpSpawner _powerUpSpawnerScript;
    [SerializeField]
    WVDPlayer _playerScript;
    [SerializeField]
    TMP_Text _dronesAndShopTimerUI;
    [SerializeField]
    TMP_Text _levelUI;
    [SerializeField]
    TMP_Text _bossTextUI;
    [SerializeField]
    WVDBossCutsceneManager _bossCutsceneManagerScript;
    [SerializeField]
    float _bossCutsceneTriggerDelay;
    [SerializeField]
    WVDMusicManager _musicManagerScript;

    //public delegate void NotifyAddNewSection(UnlockableSections section); // todo can do this with delegates/events but not really worth it 
    //public event NotifyAddNewSection OnAddNewSection;

    List<UnlockableSections> _lockedSections = new List<UnlockableSections> { UnlockableSections.GreatHall, UnlockableSections.Tower, UnlockableSections.Dungeon };

    [Header("Shop")]
    [SerializeField]
    bool _shopOpen;
    [SerializeField]
    GameObject _greatHallShop;
    [SerializeField]
    GameObject _towerShop;
    [SerializeField]
    GameObject _battlementsShop;
    [SerializeField]
    GameObject _dungeonShop;
    [SerializeField]
    List<GameObject> _availableShops; // starts off with Courtyard only
    [SerializeField]
    float _shopTime;
    float _shopTimer;
    int _lastShopTimer; // optimization so don't have to format strings every frame
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


    [Header("Section Barriers")]
    [SerializeField]
    GameObject _greatHallBarrier;
    [SerializeField]
    GameObject _towerBarrier;
    [SerializeField]
    GameObject _battlementsBarrier;
    [SerializeField]
    GameObject _dungeonBarrier;

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
                _tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.GreatHall, 2.0f);
                break;
            case UnlockableSections.Tower:
                _towerBarrier.SetActive(false);
                _availableShops.Add(_towerShop);
                _tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.Library, 2.0f);
                break;
            case UnlockableSections.Battlements:
                _battlementsBarrier.SetActive(false);
                _availableShops.Add(_battlementsShop);
                break;
            case UnlockableSections.Dungeon:
                _dungeonBarrier.SetActive(false);
                _availableShops.Add(_dungeonShop);
                _tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.Dungeon, 2.0f);
                break;
        }

        //OnAddNewSection?.Invoke(chosenSection);

        _droneSpawnerScript.AddSpawnPositionsToListFromSection(chosenSection);
        _powerUpSpawnerScript.AddSpawnPositionsToListFromSection(chosenSection);
        _powerUpSpawnerScript.AddRandomColouredPowerUpToList();
        _powerUpSpawnerScript.MaxPowerUpsSpawned += 0; // increase the max amount that can be spawned as the map is now larger todo decide if needed

        _lockedSections.Remove(_lockedSections[randIndex]);

    }

    public void LevelCompleted()
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
        if (_level == 0)
        {
            _tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.Shop, 1.0f);
        }
    }

    public void StartNewLevel()
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
        foreach (GameObject trail in _trails)
        {
            //if (trail) // alternate create/destroy
            //{
            //    Destroy(trail);
            //}
            trail.SetActive(false);
        }

        /// should unlock new section on levels 3, 5 and 7 (2, 4, 6 in code)
        if (_level <= 6 &&
            _level >= 2 &&
            _level % 2 == 0
            )
        {
            AddNewSection();
        }
        if (_level < 8) // normal level
        {
            _droneSpawnerScript.CreatePoolForLevel(_level);
            _droneSpawnerScript.Spawning = true;
            _powerUpSpawnerScript.Spawning = true;
            _levelUI.text = $"Level: {_level + 1} /8";
            _musicManagerScript.FadeCurrentMusicOutAndNewMusicIn(_musicManagerScript.PickNewRandomCombatMusicClip());
        }
        else if (_level == 8) // i.e. boss level
        {
            Invoke("StartBossCutscene", _bossCutsceneTriggerDelay);
            _levelUI.gameObject.SetActive(false);
            _dronesAndShopTimerUI.gameObject.SetActive(false);
            _bossTextUI.gameObject.SetActive(true);
            _musicManagerScript.FadeCurrentMusicOutAndBossMusicIn();
        }




    }

    void StartBossCutscene()
    {
        _bossCutsceneManagerScript.StartBossCutscene();
    }




    private void Start()
    {
        // todo try event bus here for level complete, subscribe
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<WVDTutorialManager>();
        _shopTimer = _shopTime;
        _musicManagerScript.InitialMusicSetup();
        WVDFunctionsCheck.SetToDefault();
        StartNewLevel();

    }

    private void Update()
    {
        if (_shopOpen)
        {
            if (Vector3.Distance(_playerScript.gameObject.transform.position, _chosenShop.transform.position) > _playerToShopTrailThreshold)
            {
                if (!_shopTrailCoroutineRunning)
                {
                    _shopTrailCoroutineRunning = true;
                    _playerToShopCoroutine = StartCoroutine(TrailToShopAnimation());
                }
            }



            if (_shopTimer < 0.0f)
            {
                _shopTimer = _shopTime;
                _shopOpen = false;
                StartNewLevel();
            }
            else
            {
                if ((int)_shopTimer != _lastShopTimer) // update UI if UI needs to be updated
                {
                    _dronesAndShopTimerUI.text = "Shop time remaining: " + (int)_shopTimer;
                    _lastShopTimer = (int)_shopTimer;
                }

                _shopTimer -= Time.deltaTime;
            }

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
    }

    IEnumerator TrailToShopAnimation()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(_playerScript.gameObject.transform.position, _chosenShop.transform.position, ~3, path); // 3 is the trail mask nav mesh area
        List<Vector3> pointsForThisAnimation = new List<Vector3>();
        foreach(Vector3 point in path.corners)
        {
            pointsForThisAnimation.Add(new Vector3(point.x, point.y + 0.8f, point.z)); // adding vertical offset
        }
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
            Vector3 directionToNextPoint = (pointsForThisAnimation[i + 1] - startingPos).normalized; // might need to make this between the overshot point and the next target, rather than between each waypoint, see how it comes out first
            startingPos += directionToNextPoint * _shopTrailAnimationDistGap;
            //GameObject trail = Instantiate(_shopTrailFX, startingPos, Quaternion.identity); // alternate create/destroy
            //_trails.Add(trail);
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

    public enum UnlockableSections
    {
        GreatHall,
        Tower,
        Battlements,
        Dungeon
    }
}
