using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WVDPlayerPowerUpManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    WVDPlayer _playerScript;
    PowerUpType _primaryPowerUpHeld;
    int _secondaryPowerUpCountHeld;

    [Header("Heal")]
    [SerializeField]
    int _healValueBase;
    [SerializeField]
    int _healValueUpgrade1;
    [SerializeField]
    int _healValueUpgrade2;

    [Header("Shield")] // todo should probably move the shield + other power up deploying here
    [SerializeField]
    float _shieldDuration;
    [Header("Traps")]
    [Header("Attack")]
    [SerializeField]
    GameObject _grenadePrefab;
    [SerializeField]
    Transform _grenadeFirePoint;
    [SerializeField]
    GameObject _circleAttackPrefab;
    [SerializeField]
    int _numAttacksInCircle;
    [SerializeField]
    float _circleSpawnOffset;
    [SerializeField]
    GameObject _homingAttackPrefab;
    [Header("Explosion")]

    [Header("UI")]
    [SerializeField]
    GameObject _primaryPowerUpIcon;
    [SerializeField]
    GameObject[] _secondaryPowerUpIcons;
    public PowerUpType PrimaryPowerUpHeld
    {
        get => _primaryPowerUpHeld;
        set 
        {
            _primaryPowerUpIcon.SetActive(true);
            _primaryPowerUpHeld = value;
            switch (value)
            {
                case PowerUpType.Heal:
                    _primaryPowerUpIcon.GetComponent<Image>().color = Color.green;
                    break;
                case PowerUpType.Shield:
                    _primaryPowerUpIcon.GetComponent<Image>().color = Color.blue;
                    break;
                case PowerUpType.Trap:
                    _primaryPowerUpIcon.GetComponent<Image>().color = Color.yellow;
                    break;
                case PowerUpType.Attack:
                    _primaryPowerUpIcon.GetComponent<Image>().color = Color.red;
                    break;
                case PowerUpType.Explosion:
                    _primaryPowerUpIcon.GetComponent<Image>().color = new Color(0.5f, 0.0f, 1.0f);
                    break;
                case PowerUpType.Upgrade:
                case PowerUpType.None:
                    Debug.LogError($"ERROR: Should not have been given the {value} power up type here");
                    _primaryPowerUpIcon.GetComponent<Image>().color = Color.white;
                    break;
            }
        }
    }
    public int SecondaryPowerUpCountHeld
    {
        get => _secondaryPowerUpCountHeld;
        set
        {
            if (value > 2)
            {
                _secondaryPowerUpCountHeld = 2;
            }
            else if (value < 0)
            {
                _secondaryPowerUpCountHeld = 0;
            }
            else
            {
                _secondaryPowerUpIcons[_secondaryPowerUpCountHeld].SetActive(true);
                _secondaryPowerUpCountHeld = value;
            }
        }
    }

    void Start()
    {
        
        ResetPowerUps();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_primaryPowerUpHeld == PowerUpType.Heal)
            {
                switch (_secondaryPowerUpCountHeld)
                {
                    case 0:
                        _playerScript.CurrentHealth += _healValueBase;
                        break;
                    case 1:
                        _playerScript.CurrentHealth += _healValueUpgrade1;
                        break;
                    case 2:
                        _playerScript.CurrentHealth += _healValueUpgrade2;
                        break;
                }
            }
            else if (_primaryPowerUpHeld == PowerUpType.Shield)
            {
                switch (_secondaryPowerUpCountHeld)
                {
                    case 0:
                        _playerScript.SwitchOnShieldForSeconds(WVDPlayer.ShieldVersion.Regular, _shieldDuration);
                        break;
                    case 1:
                        _playerScript.SwitchOnShieldForSeconds(WVDPlayer.ShieldVersion.Reflect, _shieldDuration);
                        break;
                    case 2:
                        _playerScript.SwitchOnShieldForSeconds(WVDPlayer.ShieldVersion.Electric, _shieldDuration);
                        break;
                }
            }
            else if (_primaryPowerUpHeld == PowerUpType.Trap)
            {
                switch (_secondaryPowerUpCountHeld)
                {
                    case 0:
                        _playerScript.DeployTrap(WVDPlayer.TrapVersion.Slow);
                        break;
                    case 1:
                        _playerScript.DeployTrap(WVDPlayer.TrapVersion.Damage);
                        break;
                    case 2:
                        _playerScript.DeployTrap(WVDPlayer.TrapVersion.Explosive);
                        break;
                }
            }
            else if (_primaryPowerUpHeld == PowerUpType.Attack)
            {
                switch (_secondaryPowerUpCountHeld)
                {
                    case 0:
                        DeployGrenade();
                        break;
                    case 1:
                        FullCircleAttack();
                        break;
                    case 2:
                        DeployHomingAttack();
                        break;
                }
            }
            ResetPowerUps();
        }    
    }

    void DeployGrenade()
    {
        GameObject grenade = Instantiate(_grenadePrefab, _grenadeFirePoint.position, _grenadePrefab.transform.rotation);
        Vector3 arcDirection = new Vector3(_grenadeFirePoint.forward.x, 0.5f, _grenadeFirePoint.forward.z);
        grenade.GetComponent<WVDGrenadePowerUpProjectile>().SetProjectileDirection(arcDirection);
        // model transform forwards, slight elevation, gravity on to give arc
    }

    void FullCircleAttack()
    {
        float degreeIncrement = 360.0f / (float)_numAttacksInCircle;
        for (int i = 0; i < _numAttacksInCircle; i++)
        {
            Vector3 projectileDirection = Quaternion.Euler(0.0f, i * degreeIncrement, 0.0f) * _grenadeFirePoint.forward;
            Vector3 spawnPoint = transform.position + projectileDirection * _circleSpawnOffset;
            GameObject projectile = Instantiate(_circleAttackPrefab, spawnPoint, Quaternion.identity);
            projectile.GetComponent<WVDPlayerProjectile>().SetProjectileDirection(projectileDirection);
        }
    }

    void DeployHomingAttack()
    {
        GameObject homingAttack = Instantiate(_homingAttackPrefab, _grenadeFirePoint.position, _homingAttackPrefab.transform.rotation);
        homingAttack.GetComponent<WVDHomingProjectile>().DroneTargets = _playerScript.Drones;
        homingAttack.GetComponent<WVDHomingProjectile>().SetProjectileDirection(_grenadeFirePoint.forward); // unless there are no enemies currently out, this will immediately change, but just gives it something so it doesn't sit there in this scenario
    }

    void ResetPowerUps()
    {
        _primaryPowerUpHeld = PowerUpType.None;
        _secondaryPowerUpCountHeld = 0;
        _primaryPowerUpIcon.SetActive(false);
        foreach (GameObject icon in _secondaryPowerUpIcons)
        {
            icon.SetActive(false);
        }
    }



    public enum PowerUpType
    {
        Heal, // Green
        Shield, // Blue
        Trap, // Yellow
        Attack, // Red
        Explosion, // Purple
        Upgrade, // white/black/silver?
        None,
    }
}
