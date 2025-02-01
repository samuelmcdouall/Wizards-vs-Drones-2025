using System.Collections.Generic;
using UnityEngine;

public class WVDPowerUpSpawner : MonoBehaviour
{
    [SerializeField]
    bool _spawning;

    public List<WVDPowerUp> SpawnedPowerUps = new List<WVDPowerUp>();

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
    int _currentPowerUpsSpawned;
    [SerializeField]
    int _maxPowerUpsSpawned;

    [Header("Power Ups")]
    [SerializeField]
    List<GameObject> _unavailableColouredPowerUps;
    [SerializeField]
    List<GameObject> _availableColouredPowerUps;
    [SerializeField]
    GameObject _upgradePowerUp;
    [SerializeField]
    float _chanceSpawnColouredPowerUp;
    [SerializeField]
    WVDLevelManager _levelManagerScript;
    [SerializeField]
    GameObject _tome;
    [SerializeField]
    float _chanceSpawnTome; // Tome instead of power up (power up could then be coloured or upgrade)
    [SerializeField]
    int _minLevelTomeCanSpawn;
    bool _tomeSpawned; // Counts towards the max number of power ups but can only have one of these out at a time
    bool _firstSpawn; // very first spawn has to be a coloured power up to explain mechanic
    bool _firstPowerUpAdded;

    [Header("Spawning Times")]
    [SerializeField]
    float _spawnTimeMin;
    [SerializeField]
    float _spawnTimeMax;
    [SerializeField]
    float _spawnTimer;

    public int CurrentPowerUpsSpawned 
    { 
        get => _currentPowerUpsSpawned; 
        set => _currentPowerUpsSpawned = value; 
    }
    public List<Transform> AvailableSpawnPositions 
    { 
        get => _availableSpawnPositions; 
        set => _availableSpawnPositions = value; 
    }
    public bool Spawning 
    { 
        get => _spawning; 
        set => _spawning = value; 
    }
    public int MaxPowerUpsSpawned 
    { 
        get => _maxPowerUpsSpawned; 
        set => _maxPowerUpsSpawned = value; 
    }
    public bool TomeSpawned 
    { 
        get => _tomeSpawned; 
        set => _tomeSpawned = value; 
    }

    void Start()
    {
        _spawnTimer = Random.Range(_spawnTimeMin, _spawnTimeMax);
        _currentPowerUpsSpawned = 0;
        _firstSpawn = true;
    }

    void Update()
    {
        if (_spawning)
        {
            if (_spawnTimer < 0.0f)
            {
                _spawnTimer = Random.Range(_spawnTimeMin, _spawnTimeMax);
                if (_currentPowerUpsSpawned < _maxPowerUpsSpawned && _availableSpawnPositions.Count > 0 && !WVDFunctionsCheck.IsDead) // todo bug with power ups not spawning, for one of these reasons
                {
                    SpawnRandomPowerUp();
                }
            }
            else
            {
                _spawnTimer -= Time.deltaTime;
            }
        }
    }

    public void AddRandomColouredPowerUpToList() // should be triggered with each X level increases
    {
        if (!_firstPowerUpAdded) // first power up should always be the first in the list (i.e. green, set this in the inspector)
        {
            _availableColouredPowerUps.Add(_unavailableColouredPowerUps[0]);
            _unavailableColouredPowerUps.Remove(_unavailableColouredPowerUps[0]);
            _firstPowerUpAdded = true;
        }
        else if (_unavailableColouredPowerUps.Count > 0)
        {
            int randIndex = Random.Range(0, _unavailableColouredPowerUps.Count);
            GameObject chosenPowerUp = _unavailableColouredPowerUps[randIndex];
            _availableColouredPowerUps.Add(chosenPowerUp);
            _unavailableColouredPowerUps.Remove(chosenPowerUp);
        }
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
        foreach(Transform position in spawnPositions)
        {
            _availableSpawnPositions.Add(position);
        }
    }

    void SpawnRandomPowerUp() // Includes potential to spawn a tome
    {
        GameObject chosenPowerUp = null;
        Vector3 spawnOffset = Vector3.zero;
        
        float rand = Random.Range(0.0f, 1.0f);
        if (_levelManagerScript.Level >= _minLevelTomeCanSpawn && 
            rand < _chanceSpawnTome && 
            !_tomeSpawned
            )
        {
            chosenPowerUp = _tome;
            spawnOffset = new Vector3(0.0f, 0.4f, 0.0f); // Tome needs to spawn in slightly higher than power ups
            _tomeSpawned = true;
        }
        else
        {
            if (Random.Range(0.0f, 1.0f) < _chanceSpawnColouredPowerUp || _firstSpawn)
            {
                chosenPowerUp = _availableColouredPowerUps[Random.Range(0, _availableColouredPowerUps.Count)];
                _firstSpawn = false;
            }
            else
            {
                chosenPowerUp = _upgradePowerUp;
            }
        }

        int randIndex = Random.Range(0, _availableSpawnPositions.Count);
        Transform spawnedTransform = _availableSpawnPositions[randIndex];
        WVDPowerUp powerUp = Instantiate(chosenPowerUp, spawnedTransform.position + spawnOffset, chosenPowerUp.transform.rotation).GetComponent<WVDPowerUp>();
        powerUp.SetSpawnerParameters(this, spawnedTransform);
        SpawnedPowerUps.Add(powerUp);
        _availableSpawnPositions.Remove(spawnedTransform);
        _currentPowerUpsSpawned++;

    }
}
