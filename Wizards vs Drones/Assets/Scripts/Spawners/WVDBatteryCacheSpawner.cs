using System.Collections.Generic;
using UnityEngine;

public class WVDBatteryCacheSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField]
    List<Transform> _availableSpawnPositions;
    [SerializeField]
    int _minTotalNumCacheSpawns;
    [SerializeField]
    int _maxTotalNumCacheSpawns;
    [SerializeField]
    GameObject _cachePrefab;

    void Start()
    {
        int totalNumCacheSpawns = Random.Range(_minTotalNumCacheSpawns, _maxTotalNumCacheSpawns + 1);
        print($"Spawning {totalNumCacheSpawns} battery caches");
        for (int i = 0; i < totalNumCacheSpawns; i++)
        {
            int index = Random.Range(0, _availableSpawnPositions.Count);
            Vector3 spawnPosition = _availableSpawnPositions[index].position;
            Instantiate(_cachePrefab, spawnPosition, _cachePrefab.transform.rotation);
            _availableSpawnPositions.RemoveAt(index);
        }
    }
}
