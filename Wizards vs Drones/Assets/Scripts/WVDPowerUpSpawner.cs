using System.Collections.Generic;
using UnityEngine;

public class WVDPowerUpSpawner : MonoBehaviour
{
    [SerializeField]
    bool _spawning;

    [Header("Spawn Positions")]
    [SerializeField]
    List<Transform> _availableSpawnPositions;

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
                if (_currentPowerUpsSpawned < _maxPowerUpsSpawned)
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

    void AddRandomColouredPowerUpToList() // should be triggered with each X level increases
    {
        if (_unavailableColouredPowerUps.Count > 0)
        {
            int randIndex = Random.Range(0, _unavailableColouredPowerUps.Count);
            GameObject chosenPowerUp = _unavailableColouredPowerUps[randIndex];
            _availableColouredPowerUps.Add(chosenPowerUp);
            _unavailableColouredPowerUps.Remove(chosenPowerUp);
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
        _availableSpawnPositions.Remove(spawnedTransform);
        _currentPowerUpsSpawned++;

    }
}
