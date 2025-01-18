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

    void Start()
    {
        _spawnTimer = Random.Range(_spawnTimeMin, _spawnTimeMax);
        _currentPowerUpsSpawned = 0;
    }

    void Update()
    {
        if (_spawning)
        {
            if (_spawnTimer < 0.0f)
            {
                _spawnTimer = Random.Range(_spawnTimeMin, _spawnTimeMax);
                if (_currentPowerUpsSpawned < _maxPowerUpsSpawned && _availableSpawnPositions.Count > 0)
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
        if (_unavailableColouredPowerUps.Count > 0)
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

    void SpawnRandomPowerUp()
    {
        GameObject chosenPowerUp = null;
        if (Random.Range(0.0f, 1.0f) < _chanceSpawnColouredPowerUp)
        {
            chosenPowerUp = _availableColouredPowerUps[Random.Range(0, _availableColouredPowerUps.Count)];
        }
        else
        {
            chosenPowerUp = _upgradePowerUp;
        }

        int randIndex = Random.Range(0, _availableSpawnPositions.Count);
        Transform spawnedTransform = _availableSpawnPositions[randIndex];
        WVDPowerUp powerUp = Instantiate(chosenPowerUp, spawnedTransform.position, chosenPowerUp.transform.rotation).GetComponent<WVDPowerUp>();
        powerUp.SetSpawnerParameters(this, spawnedTransform);
        SpawnedPowerUps.Add(powerUp);
        _availableSpawnPositions.Remove(spawnedTransform);
        _currentPowerUpsSpawned++;

    }
}
