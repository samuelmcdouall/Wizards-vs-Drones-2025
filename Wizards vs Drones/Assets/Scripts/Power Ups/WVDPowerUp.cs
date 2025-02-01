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
    WVDTutorialManager _tutorialManager;

    public Transform SpawnedTransform { get => _spawnedTransform; set => _spawnedTransform = value; }

    private void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<WVDTutorialManager>();
        switch (_selectedPowerUpType)
        {
            case WVDPlayerPowerUpManager.PowerUpType.Attack:
                //_tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.AttackPowerUp, 1.0f);
                WVDEventBus.Raise(new WVDDisplayTutorialEventData(WVDTutorialManager.TutorialPart.AttackPowerUp, 1.0f));
                break;
            case WVDPlayerPowerUpManager.PowerUpType.Shield:
                //_tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.ShieldPowerUp, 1.0f);
                WVDEventBus.Raise(new WVDDisplayTutorialEventData(WVDTutorialManager.TutorialPart.ShieldPowerUp, 1.0f));
                break;
            case WVDPlayerPowerUpManager.PowerUpType.Heal:
                //_tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.HealPowerUp, 1.0f);
                WVDEventBus.Raise(new WVDDisplayTutorialEventData(WVDTutorialManager.TutorialPart.HealPowerUp, 1.0f));
                break;
            case WVDPlayerPowerUpManager.PowerUpType.Trap:
                //_tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.TrapPowerUp, 1.0f);
                WVDEventBus.Raise(new WVDDisplayTutorialEventData(WVDTutorialManager.TutorialPart.TrapPowerUp, 1.0f));
                break;
        }
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
                    drone.TakeDamage(100, false); // don't want to deafen the player with every out drone playing damage effect;
                }
                _powerUpSpawner.TomeSpawned = false;
                _soundManager.PlaySFXAtPlayer(_soundManager.TomePowerUpSFX);
                WVDEventBus.Raise(new WVDDisplayTutorialEventData(WVDTutorialManager.TutorialPart.Tome, 1.0f));
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