using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WVDPowerUp : MonoBehaviour
{
    [SerializeField]
    WVDPlayerPowerUpManager.PowerUpType _selectedPowerUpType;
    WVDPowerUpSpawner _powerUpSpawner;
    Transform _spawnedTransform;
    private WVDSoundManager _soundManager;

    public Transform SpawnedTransform { get => _spawnedTransform; set => _spawnedTransform = value; }

    private void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
    }

    public void SetSpawnerParameters(WVDPowerUpSpawner spawner, Transform spawnedPosition)
    {
        _powerUpSpawner = spawner;
        _spawnedTransform = spawnedPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_selectedPowerUpType == WVDPlayerPowerUpManager.PowerUpType.Tome)
            {
                List<IWVDDamageable> drones = other.gameObject.GetComponent<WVDPlayer>().Drones;
                foreach (IWVDDamageable drone in drones.ToList())
                {
                    drone.ResolveAttack(100, new WVDAttackEffects());
                }
                _powerUpSpawner.TomeSpawned = false;
                _soundManager.PlaySFXAtPlayer(_soundManager.TomePowerUpSFX);
            }
            else if (_selectedPowerUpType == WVDPlayerPowerUpManager.PowerUpType.Upgrade)
            {
                other.gameObject.GetComponent<WVDPlayerPowerUpManager>().SecondaryPowerUpCountHeld++;
                _soundManager.PlaySFXAtPlayer(_soundManager.PickupPowerUpSFX);

            }
            else
            {
                other.gameObject.GetComponent<WVDPlayerPowerUpManager>().PrimaryPowerUpHeld = _selectedPowerUpType;
                _soundManager.PlaySFXAtPlayer(_soundManager.PickupPowerUpSFX);
            }
            _powerUpSpawner.CurrentPowerUpsSpawned--;
            _powerUpSpawner.AvailableSpawnPositions.Add(_spawnedTransform);
            _powerUpSpawner.SpawnedPowerUps.Remove(this);
            Destroy(gameObject);
        }
    }
}