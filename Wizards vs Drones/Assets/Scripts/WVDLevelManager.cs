using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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

    //public delegate void NotifyAddNewSection(UnlockableSections section); // todo can do this with delegates/events but not really worth it 
    //public event NotifyAddNewSection OnAddNewSection;

    List<UnlockableSections> _lockedSections = new List<UnlockableSections> { UnlockableSections.GreatHall, UnlockableSections.Tower, UnlockableSections.Battlements, UnlockableSections.Dungeon };

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
    GameObject _shopTrailFX;
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

    [Header("Section Barriers")]
    [SerializeField]
    GameObject _greatHallBarrier;
    [SerializeField]
    GameObject _towerBarrier;
    [SerializeField]
    GameObject _battlementsBarrier;
    [SerializeField]
    GameObject _dungeonBarrier;


    void AddNewSection()
    {
        int randIndex = Random.Range(0, _lockedSections.Count);
        UnlockableSections chosenSection = _lockedSections[randIndex];

        switch (chosenSection)
        {
            case UnlockableSections.GreatHall:
                _greatHallBarrier.SetActive(false);
                _availableShops.Add(_greatHallShop);
                break;
            case UnlockableSections.Tower:
                _towerBarrier.SetActive(false);
                _availableShops.Add(_towerShop);
                break;
            case UnlockableSections.Battlements:
                _battlementsBarrier.SetActive(false);
                _availableShops.Add(_battlementsShop);
                break;
            case UnlockableSections.Dungeon:
                _dungeonBarrier.SetActive(false);
                _availableShops.Add(_dungeonShop);
                break;
        }

        //OnAddNewSection?.Invoke(chosenSection);

        _droneSpawnerScript.AddSpawnPositionsToListFromSection(chosenSection);
        _powerUpSpawnerScript.AddSpawnPositionsToListFromSection(chosenSection);
        _powerUpSpawnerScript.AddRandomColouredPowerUpToList();

        _lockedSections.Remove(_lockedSections[randIndex]);

    }

    public void LevelCompleted()
    {
        _shopOpen = true;
        _chosenShop = _availableShops[Random.Range(0, _availableShops.Count)];
        _chosenShop.SetActive(true);

        foreach (WVDPowerUp powerUp in _powerUpSpawnerScript.SpawnedPowerUps)
        {
            Destroy(powerUp.gameObject);
        }
        _powerUpSpawnerScript.SpawnedPowerUps.Clear();
        _powerUpSpawnerScript.Spawning = false;
    }

    public void StartNewLevel()
    {
        _level++;
        _chosenShop?.SetActive(false);
        _chosenShop = null;
        _shopUI.SetActive(false);
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
        _levelUI.text = $"Level: {_level + 1} /10";

        /// if right level, 1,3,5,7
        //if (_level <= 7 && _level % 2 == 1)
        //{
        //    AddNewSection();
        //}


        /// every level
        _droneSpawnerScript.CreatePoolForLevel(_level);
        //set UI for total drone num

        _droneSpawnerScript.Spawning = true;
        _powerUpSpawnerScript.Spawning = true;
    }

    


    private void Start()
    {
        _shopTimer = _shopTime;
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
        }
    }

    IEnumerator TrailToShopAnimation()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(_playerScript.gameObject.transform.position, _chosenShop.transform.position, NavMesh.AllAreas, path);
        List<Vector3> pointsForThisAnimation = new List<Vector3>();
        foreach(Vector3 point in path.corners)
        {
            pointsForThisAnimation.Add(new Vector3(point.x, point.y + 0.8f, point.z)); // adding vertical offset
        }

        Vector3 startingPos = pointsForThisAnimation[0];
        int i = 0;
        while (i < pointsForThisAnimation.Count - 1)
        {
            Vector3 directionToNextPoint = (pointsForThisAnimation[i + 1] - pointsForThisAnimation[i]).normalized; // might need to make this between the overshot point and the next target, rather than between each waypoint, see how it comes out first
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
