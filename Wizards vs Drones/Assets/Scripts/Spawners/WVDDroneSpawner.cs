using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WVDDroneSpawner : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    bool _spawning;
    [SerializeField]
    WVDLevelManager _levelManagerScript;
    [SerializeField]
    Transform _player;
    [SerializeField]
    WVDPlayer _playerScript;
    WVDDifficultySettingsManager _difficultyModeManager;

    [Header("Drones Spawn Stats")]
    [SerializeField]
    List<WVDDroneSpawnRound> _dronesPerRound;

    [Header("Spawn Positions")]
    [SerializeField]
    List<Transform> _greatHallSpawnPositions;
    [SerializeField]
    List<Transform> _towerSpawnPositions;
    [SerializeField]
    List<Transform> _battlementsSpawnPositions;
    [SerializeField]
    List<Transform> _dungeonSpawnPositions;
    [SerializeField]
    List<Transform> _availableSpawnPositions; // always start off with all the courtyard positions, then add the others

    [Header("Spawn Limits")]
    [SerializeField]
    int _currentDronesSpawned;
    [SerializeField]
    int _maxDronesSpawned;
    [SerializeField]
    float _playerThreshold;

    [Header("Spawning Times")]
    [SerializeField]
    float _spawnTimeMin;
    [SerializeField]
    float _spawnTimeMax;
    [SerializeField]
    float _spawnTimer;

    [Header("Drones")]
    [SerializeField]
    GameObject _electricDrone;
    [SerializeField]
    GameObject _laserDrone;
    [SerializeField]
    GameObject _fastDrone;
    [SerializeField]
    GameObject _teleportDrone;

    [Header("Level")]
    [SerializeField]
    List<GameObject> _spawnPool;
    int _levelDronesRemaining;
    [SerializeField]
    TMP_Text _levelDronesRemainingUI; // this UI is also used for the shop timer, switches to it in the level manager after a level is completed
    public int DronesRemainingHelpUIThreshold;
    bool _triggeredHelpUIThisLevel;

    public int CurrentDronesSpawned 
    { 
        get => _currentDronesSpawned; 
        set
        {
            _currentDronesSpawned = value;
            if (_currentDronesSpawned == 0 && _spawnPool.Count == 0) // No more drones currently out or to be spawned, so level must be complete
            {
                WVDEventBus.Raise(new WVDEventDataLevelComplete());
            }
        }
    }
    public bool Spawning 
    { 
        get => _spawning; 
        set => _spawning = value; 
    }
    public int LevelDronesRemaining 
    { 
        get => _levelDronesRemaining;
        set 
        {
            _levelDronesRemaining = value;
            _levelDronesRemainingUI.text = "Drones remaining: " + _levelDronesRemaining;
            if (!_triggeredHelpUIThisLevel && _levelDronesRemaining <= DronesRemainingHelpUIThreshold) // If the drones reach below threshold then all ones already spawned must show their UI helper
            {
                print($"Below threshold in level {_levelManagerScript.Level}, need to spawn in the help UI!");
                foreach (IWVDDamageable drone in _playerScript.Drones)
                {
                    drone.GetTransform().gameObject.GetComponent<WVDBaseDrone>().SpawnDroneRemainingHelpUI();
                }
                _triggeredHelpUIThisLevel = true;
            }
        }
    }

    public List<WVDDroneSpawnRound> DronesPerRound 
    { 
        get => _dronesPerRound; 
        set => _dronesPerRound = value; 
    }
    public bool TriggeredHelpUIThisLevel 
    { 
        get => _triggeredHelpUIThisLevel; 
        set => _triggeredHelpUIThisLevel = value; 
    }
    public List<Transform> AvailableSpawnPositions 
    { 
        get => _availableSpawnPositions; 
        set => _availableSpawnPositions = value; 
    }

    void Start()
    {
        _spawnTimer = 4.0f; // need time to show tutorial before drones start spawning in
        _difficultyModeManager = GameObject.FindGameObjectWithTag("DifficultyModeManager").GetComponent<WVDDifficultySettingsManager>();
    }
    void Update()
    {
        if (_spawning)
        {
            if (_spawnTimer < 0.0f)
            {
                _spawnTimer = Random.Range(_spawnTimeMin, _spawnTimeMax);
                if (_currentDronesSpawned < _maxDronesSpawned && !WVDFunctionsCheck.IsDead)
                {
                    if (_spawnPool.Count > 0)
                    {
                        SpawnRandomDrone();
                    }
                    else
                    {
                        _spawning = false;
                    }
                }
            }
            else
            {
                _spawnTimer -= Time.deltaTime;
            }
        }
    }
    void SpawnRandomDrone()
    {
        GameObject chosenDrone = null;
        int randIndex = Random.Range(0, _spawnPool.Count);
        chosenDrone = _spawnPool[randIndex];

        Transform spawnedTransform = _availableSpawnPositions[Random.Range(0, _availableSpawnPositions.Count)];
        while (Vector3.Distance(_player.transform.position, spawnedTransform.position) <= _playerThreshold) // this is so Drones don't spawn right next to a player
        {
            spawnedTransform = _availableSpawnPositions[Random.Range(0, _availableSpawnPositions.Count)];
        }

        WVDBaseDrone drone = Instantiate(chosenDrone, spawnedTransform.position, chosenDrone.transform.rotation).GetComponent<WVDBaseDrone>();
        drone.SetSpawnerParameters(this);

        _spawnPool.Remove(_spawnPool[randIndex]);
        _currentDronesSpawned++;
    }
    public void CreatePoolForLevel(int level) // should be triggered with each level completed
    {
        _spawnPool.Clear();
        WVDDroneSpawnRound dronesForThisRound = _dronesPerRound[level];

        // Choose modifier of drone number based on difficulty
        float modifier = 1.0f;
        if (_difficultyModeManager.SelectedDifficulty == WVDDifficultySettingsManager.Difficulty.Medium)
        {
            modifier = 0.75f;
        }
        else if (_difficultyModeManager.SelectedDifficulty == WVDDifficultySettingsManager.Difficulty.Easy)
        {
            modifier = 0.5f;
        }
        print($"Challenge modifier: {modifier}");
        _maxDronesSpawned = (int)(modifier * dronesForThisRound.MaxDroneLimit);
        print($"Max drones at one time this round: {_maxDronesSpawned}");

        AddDroneNumberToPool(_electricDrone, (int)(modifier * dronesForThisRound.MinElectric), (int)(modifier * dronesForThisRound.MaxElectric));
        AddDroneNumberToPool(_laserDrone, (int)(modifier * dronesForThisRound.MinLaser), (int)(modifier * dronesForThisRound.MaxLaser));
        AddDroneNumberToPool(_fastDrone, (int)(modifier * dronesForThisRound.MinFast), (int)(modifier * dronesForThisRound.MaxFast));
        AddDroneNumberToPool(_teleportDrone, (int)(modifier * dronesForThisRound.MinTeleport), (int)(modifier * dronesForThisRound.MaxTeleport));
        LevelDronesRemaining = _spawnPool.Count;
        _triggeredHelpUIThisLevel = false;
    }
    void AddDroneNumberToPool(GameObject drone, int min, int max)
    {
        int randNumDrones = Random.Range(min, max + 1);
        for (int i = 0; i < randNumDrones; i++)
        {
            _spawnPool.Add(drone);
        }
        print($"Added {randNumDrones} {drone.name} to the pool ({min} - {max})");
    }
    public void AddSpawnPositionsToListFromSection(WVDLevelManager.UnlockableSections section)
    {
        switch (section)
        {
            case WVDLevelManager.UnlockableSections.GreatHall:
                AddSpawnPositionsToList(_greatHallSpawnPositions);
                break;
            case WVDLevelManager.UnlockableSections.Tower:
                AddSpawnPositionsToList(_towerSpawnPositions);
                break;
            case WVDLevelManager.UnlockableSections.Battlements:
                AddSpawnPositionsToList(_battlementsSpawnPositions);
                break;
            case WVDLevelManager.UnlockableSections.Dungeon:
                AddSpawnPositionsToList(_dungeonSpawnPositions);
                break;
        }
    }
    void AddSpawnPositionsToList(List<Transform> spawnPositions)
    {
        foreach (Transform position in spawnPositions)
        {
            _availableSpawnPositions.Add(position);
        }
    }
}