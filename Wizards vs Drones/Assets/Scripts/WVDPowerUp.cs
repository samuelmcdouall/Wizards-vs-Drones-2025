using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPowerUp : MonoBehaviour
{
    [SerializeField]
    WVDPlayerPowerUpManager.PowerUpType _selectedPowerUpType;
    WVDPowerUpSpawner _powerUpSpawner;
    Transform _spawnedTransform;

    public Transform SpawnedTransform { get => _spawnedTransform; set => _spawnedTransform = value; }

    public void SetSpawnerParameters(WVDPowerUpSpawner spawner, Transform spawnedPosition)
    {
        _powerUpSpawner = spawner;
        _spawnedTransform = spawnedPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_selectedPowerUpType == WVDPlayerPowerUpManager.PowerUpType.Upgrade)
            {
                other.gameObject.GetComponent<WVDPlayerPowerUpManager>().SecondaryPowerUpCountHeld++;
            }
            else
            {
                other.gameObject.GetComponent<WVDPlayerPowerUpManager>().PrimaryPowerUpHeld = _selectedPowerUpType;
            }
            _powerUpSpawner.CurrentPowerUpsSpawned--;
            _powerUpSpawner.AvailableSpawnPositions.Add(_spawnedTransform);
            _powerUpSpawner.SpawnedPowerUps.Remove(this);
            Destroy(gameObject);
        }
    }
}