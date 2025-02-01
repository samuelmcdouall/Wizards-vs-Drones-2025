using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WVDPowerUp : MonoBehaviour
{
    [Header("Type")]
    [SerializeField]
    WVDPlayerPowerUpManager.PowerUpType _selectedPowerUpType;

    [Header("Spawning")]
    WVDPowerUpSpawner _powerUpSpawner;
    Transform _spawnedTransform;

    [Header("Other")]
    WVDSoundManager _soundManager;
    WVDTutorialManager _tutorialManager;

    public Transform SpawnedTransform 
    { 
        get => _spawnedTransform; 
        set => _spawnedTransform = value; 
    }

    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<WVDTutorialManager>();
        switch (_selectedPowerUpType) // Play tutorial for corresponding power up
        {
            case WVDPlayerPowerUpManager.PowerUpType.Attack:
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.AttackPowerUp, 1.0f));
                break;
            case WVDPlayerPowerUpManager.PowerUpType.Shield:
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.ShieldPowerUp, 1.0f));
                break;
            case WVDPlayerPowerUpManager.PowerUpType.Heal:
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.HealPowerUp, 1.0f));
                break;
            case WVDPlayerPowerUpManager.PowerUpType.Trap:
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.TrapPowerUp, 1.0f));
                break;
        }
    }
    public void SetSpawnerParameters(WVDPowerUpSpawner spawner, Transform spawnedPosition)
    {
        _powerUpSpawner = spawner;
        _spawnedTransform = spawnedPosition;
    }
    void DestroyEveryActiveDrone(Collider other)
    {
        List<IWVDDamageable> drones = other.gameObject.GetComponent<WVDPlayer>().Drones; // Getting drone list off of Player script
        foreach (IWVDDamageable drone in drones.ToList())
        {
            drone.TakeDamage(100, false); // don't want to deafen the player with every out drone playing damage effect, hence set to false here
        }
        _powerUpSpawner.TomeSpawned = false;
        _soundManager.PlaySFXAtPlayer(_soundManager.TomePowerUpSFX);
        WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.Tome, 1.0f));
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_selectedPowerUpType == WVDPlayerPowerUpManager.PowerUpType.Tome)
            {
                DestroyEveryActiveDrone(other);
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