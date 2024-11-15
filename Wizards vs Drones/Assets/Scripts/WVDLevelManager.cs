using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

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
    float _playerToShopThreshold;

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
    GameObject _chosenShop;
    [SerializeField]
    GameObject _shopUI;

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
           



            if (_shopTimer < 0.0f)
            {
                _shopTimer = _shopTime;
                _shopOpen = false;
                StartNewLevel();
            }
            else
            {
                _shopTimer -= Time.deltaTime;
            }
        }
    }

    public enum UnlockableSections
    {
        GreatHall,
        Tower,
        Battlements,
        Dungeon
    }
}
