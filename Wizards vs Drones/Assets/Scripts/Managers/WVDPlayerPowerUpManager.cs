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
    float _lifestealDuration;
    [SerializeField]
    float _invulnerableDuration;

    [Header("Shield")]
    [SerializeField]
    float _shieldDuration;
    [Header("Attack")]
    [SerializeField]
    GameObject _grenadePrefab;
    [SerializeField]
    Transform _redPowerUpFirePoint;
    [SerializeField]
    GameObject _circleAttackPrefab;
    [SerializeField]
    int _numAttacksInCircle;
    [SerializeField]
    float _circleSpawnOffset;
    [SerializeField]
    GameObject _homingAttackPrefab;

    [Header("Traps")]
    [SerializeField]
    GameObject _trapSlowPrefab;
    [SerializeField]
    GameObject _trapDamagePrefab;
    [SerializeField]
    GameObject _trapExplosivePrefab;
    readonly float _trapDeploymentOffset = 2.0f;


    [Header("UI")]
    [SerializeField]
    Image _primaryPowerUpIcon;
    [SerializeField]
    Image[] _secondaryPowerUpIcons;
    WVDSoundManager _soundManager;

    public PowerUpType PrimaryPowerUpHeld
    {
        get => _primaryPowerUpHeld;
        set 
        {
            _primaryPowerUpIcon.gameObject.SetActive(true);
            _primaryPowerUpHeld = value;
            switch (value)
            {
                case PowerUpType.Heal:
                    _primaryPowerUpIcon.color = Color.green;
                    break;
                case PowerUpType.Shield:
                    _primaryPowerUpIcon.color = new Color(0.35f, 0.72f, 1.0f);
                    break;
                case PowerUpType.Trap:
                    _primaryPowerUpIcon.color = Color.yellow;
                    break;
                case PowerUpType.Attack:
                    _primaryPowerUpIcon.color = Color.red;
                    break;
                case PowerUpType.Upgrade:
                case PowerUpType.None:
                    Debug.LogError($"ERROR: Should not have been given the {value} power up type here");
                    _primaryPowerUpIcon.color = Color.white;
                    break;
            }
        }
    }
    public int SecondaryPowerUpCountHeld // Can only hold up to 2
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
                _secondaryPowerUpIcons[_secondaryPowerUpCountHeld].gameObject.SetActive(true);
                _secondaryPowerUpCountHeld = value;
            }
        }
    }

    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        ResetPowerUps();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && WVDFunctionsCheck.PlayerInputsAllowed())
        {
            if (_primaryPowerUpHeld == PowerUpType.Heal)
            {
                switch (_secondaryPowerUpCountHeld)
                {
                    case 0:
                        _playerScript.CurrentHealth += _healValueBase;
                        _soundManager.PlaySFXAtPlayer(_soundManager.HealPowerUpSFX);
                        break;
                    case 1:
                        _playerScript.ApplyLifeStealForSeconds(_lifestealDuration);
                        _soundManager.PlaySFXAtPlayer(_soundManager.LifestealPowerUpSFX);
                        break;
                    case 2:
                        _playerScript.ApplyInvulnerable(_invulnerableDuration); // invul for time
                        _soundManager.PlaySFXAtPlayer(_soundManager.InvulnerablePowerUpSFX);
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
                _soundManager.PlaySFXAtPlayer(_soundManager.ShieldPowerUpSFX);
            }
            else if (_primaryPowerUpHeld == PowerUpType.Trap)
            {
                switch (_secondaryPowerUpCountHeld)
                {
                    case 0:
                        DeployTrap(TrapVersion.Slow);
                        break;
                    case 1:
                        DeployTrap(TrapVersion.Damage);
                        break;
                    case 2:
                        DeployTrap(TrapVersion.Explosive);
                        break;
                }
                _soundManager.PlaySFXAtPlayer(_soundManager.TrapPowerUpSFX);
            }
            else if (_primaryPowerUpHeld == PowerUpType.Attack)
            {
                switch (_secondaryPowerUpCountHeld)
                {
                    case 0:
                        DeployGrenade();
                        _soundManager.PlaySFXAtPlayer(_soundManager.GrenadeThrowPowerUpSFX);
                        break;
                    case 1:
                        FullCircleAttack();
                        _soundManager.PlaySFXAtPlayer(_soundManager.CirclePowerUpSFX);
                        break;
                    case 2:
                        DeployHomingAttack();
                        _soundManager.PlaySFXAtPlayer(_soundManager.GhostPowerUpSFX);
                        break;
                }
            }
            ResetPowerUps();
        }
    }
    void DeployTrap(TrapVersion trap)
    {
        GameObject chosenTrap = _trapSlowPrefab; // set to default
        if (trap == TrapVersion.Damage)
        {
            chosenTrap = _trapDamagePrefab;
        }
        else if (trap == TrapVersion.Explosive)
        {
            chosenTrap = _trapExplosivePrefab;
        }

        Instantiate(chosenTrap, transform.position + _playerScript.PlayerModel.transform.forward * _trapDeploymentOffset + new Vector3(0.0f, -1.0f, 0.0f), _playerScript.PlayerModel.transform.rotation);
    }
    void DeployGrenade()
    {
        GameObject grenade = Instantiate(_grenadePrefab, _redPowerUpFirePoint.position, _grenadePrefab.transform.rotation);
        Vector3 arcDirection = new Vector3(_redPowerUpFirePoint.forward.x, 0.5f, _redPowerUpFirePoint.forward.z);
        grenade.GetComponent<WVDGrenadePowerUpProjectile>().SetProjectileDirection(arcDirection);
    }
    void FullCircleAttack()
    {
        float degreeIncrement = 360.0f / (float)_numAttacksInCircle;
        for (int i = 0; i < _numAttacksInCircle; i++)
        {
            Vector3 projectileDirection = Quaternion.Euler(0.0f, i * degreeIncrement, 0.0f) * _redPowerUpFirePoint.forward;
            Vector3 spawnPoint = transform.position + projectileDirection * _circleSpawnOffset;
            GameObject projectile = Instantiate(_circleAttackPrefab, spawnPoint, Quaternion.identity);
            projectile.GetComponent<WVDPlayerProjectile>().SetProjectileDirection(projectileDirection);
            projectile.transform.rotation = Quaternion.LookRotation(new Vector3(projectileDirection.x, 0.0f, projectileDirection.z));
        }
    }
    void DeployHomingAttack()
    {
        GameObject homingAttack = Instantiate(_homingAttackPrefab, _redPowerUpFirePoint.position, _homingAttackPrefab.transform.rotation);
        homingAttack.GetComponent<WVDHomingProjectile>().DroneTargets = _playerScript.Drones;
        homingAttack.GetComponent<WVDHomingProjectile>().SetProjectileDirection(_redPowerUpFirePoint.forward); // unless there are no enemies currently out, this will immediately change, but just gives it something so it doesn't sit there in this scenario
    }
    public void ResetPowerUps()
    {
        _primaryPowerUpHeld = PowerUpType.None;
        _secondaryPowerUpCountHeld = 0;
        _primaryPowerUpIcon.gameObject.SetActive(false);
        foreach (Image icon in _secondaryPowerUpIcons)
        {
            icon.gameObject?.SetActive(false);
        }
    }
    public enum PowerUpType
    {
        Heal, // Green
        Shield, // Blue
        Trap, // Yellow
        Attack, // Red
        //Explosion, // Purple, not used
        Upgrade, // black
        Tome, // One off nuke
        None,
    }
    public enum TrapVersion
    {
        Slow,
        Damage,
        Explosive
    }
}