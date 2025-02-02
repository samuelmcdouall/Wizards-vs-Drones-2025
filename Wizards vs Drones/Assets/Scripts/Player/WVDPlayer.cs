using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WVDPlayer : WVDBaseEntity, IWVDDamageable
{
    [Header("General - Player")]
    [SerializeField]
    WVDGameOverManager _gameOverManagerScript;
    [SerializeField]
    WVDBoss _bossScript;
    List<IWVDDamageable> _drones = new List<IWVDDamageable>(); // List of all active drones, to be updated as drones are created/destroyed

    [Header("Model - Player")]
    [SerializeField]
    GameObject _playerModel;

    [Header("Shield - Player")]
    [SerializeField]
    GameObject _shieldRegularFX;
    [SerializeField]
    GameObject _shieldReflectFX;
    [SerializeField]
    GameObject _shieldElectricFX;
    [SerializeField]
    float _shieldElectricDamageThreshold;
    [SerializeField]
    GameObject _shieldElectricAttackFXPrefab;
    readonly float _shieldAttackOffset = 2.8f;

    [Header("Heal - Player")]
    [SerializeField]
    GameObject _lifeStealFX;
    float _lifeStealTimer;
    bool _lifeSteal;

    [Header("Speed - Player")]
    [SerializeField]
    float _dashSpeed;

    [Header("Upgrades")]
    [SerializeField] int _batteryCount;
    [SerializeField] TMP_Text _batteryCountUI;
    public WVDPlayerUpgrades PurchasedUpgrades;

    public bool LifeSteal 
    { 
        get => _lifeSteal;
        set 
        {
            print($"Lifesteal: {value}");
            _lifeStealFX.SetActive(value);
            _lifeSteal = value;
        }
    }
    public float DashSpeed
    {
        get => _dashSpeed;
        set => _dashSpeed = value;
    }
    public bool PlayerModelOn
    {
        get => _playerModel.activeSelf;
        set => _playerModel.SetActive(value);
    }
    public List<IWVDDamageable> Drones 
    { 
        get => _drones; 
        set => _drones = value; 
    }
    public int BatteryCount 
    { 
        get => _batteryCount;
        set 
        {
            _batteryCount = value;
            _batteryCountUI.text = "" + _batteryCount;
        }
    }
    public GameObject PlayerModel 
    { 
        get => _playerModel; 
        set => _playerModel = value; 
    }

    public override void Start()
    {
        base.Start();
        CurrentPlayingAnimation = WVDAnimationStrings.PlayerIdleAnimation;
        _shieldRegularFX.SetActive(false);
        _shieldReflectFX.SetActive(false);
        _shieldElectricFX.SetActive(false);
        _batteryCountUI.text = "" + 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public override void Update()
    {
        base.Update();

        SetAnimationIfWonOrLost();

        if (LifeSteal)
        {
            if (_lifeStealTimer <= 0.0f)
            {
                print("Player no longer has lifesteal");
                LifeSteal = false;
            }
            else
            {
                _lifeStealTimer -= Time.deltaTime;
            }
        }
        if (_shieldElectricFX.activeSelf)
        {
            DestroyNearbyDrones();
        }
    }
    void SetAnimationIfWonOrLost()
    {
        if (WVDFunctionsCheck.HasWon)
        {
            SwitchToAnimation(WVDAnimationStrings.PlayerIdleAnimation);
        }
        if (WVDFunctionsCheck.IsDead)
        {
            SwitchToAnimation(WVDAnimationStrings.PlayerDieAnimation);
        }
    }
    void DestroyNearbyDrones()
    {
        foreach (IWVDDamageable drone in _drones.ToList())
        {
            if (Vector3.Distance(drone.GetTransform().position, transform.position) <= _shieldElectricDamageThreshold)
            {
                drone.TakeDamage(1000, true); // Insta kill
                WVDShieldElectricAttackFX attackFX = Instantiate(_shieldElectricAttackFXPrefab, transform.position, Quaternion.identity).GetComponent<WVDShieldElectricAttackFX>();
                Vector3 directionToDrone = (drone.GetModelTransform().position - transform.position).normalized;
                Vector3 startPos = transform.position + directionToDrone * _shieldAttackOffset;
                attackFX.SetPositions(startPos, drone.GetModelTransform().position);
            }
        }
    }
    public void AddDroneToPlayerList(IWVDDamageable drone)
    {
        _drones.Add(drone);
        print($"Drone added, currently keeping track of {_drones.Count}");
    }
    public void RemoveDroneFromPlayerList(IWVDDamageable drone)
    {
        _drones.Remove(drone);
        print($"Drone removed, currently keeping track of {_drones.Count}");
    }
    public async void SwitchOnShieldForSeconds(ShieldVersion version, float seconds) // todo maybe stuff like this should be put in the power up manager
    {
        switch (version)
        {
            case ShieldVersion.Regular:
                _shieldRegularFX.SetActive(true);
                break;
            case ShieldVersion.Reflect:
                _shieldReflectFX.SetActive(true);
                break;
            case ShieldVersion.Electric:
                _shieldElectricFX.SetActive(true);
                break;
        }
        Invulnerable = true;
        float endTime = Time.time + seconds;
        while (Time.time < endTime)
        {
            await Task.Yield();
        }
        _shieldRegularFX.SetActive(false);
        _shieldReflectFX.SetActive(false);
        _shieldElectricFX.SetActive(false);
        Invulnerable = false;
    }
    public void ApplyLifeStealForSeconds(float time)
    {
        // If a time is applied that would be larger than the time remaining then apply new time
        if (time > _lifeStealTimer)
        {
            print("Now has lifesteal!");
            LifeSteal = true;
            _lifeStealTimer = time;
        }
    }
    public void UpgradeAttackSpeedAnimation()
    {
        Animator.SetFloat("AttackSpeedAnimation", 1.33f);
    }
    public void ResolveAttack(int damage, WVDAttackEffects effects)
    {
        TakeDamage(damage, true);
        ApplyEffects(effects);
    }
    public void TakeDamage(int damage, bool playDamageSFX) // playDamageSFX is for the interface signature, only really needed for the drones
    {
        if (!WVDFunctionsCheck.IsDead)
        {
            print($"Player took {damage} damage");
            if (!Invulnerable)
            {
                CurrentHealth -= damage;
            }
            print($"Player on {CurrentHealth} health");
            if (IsFullyDamaged())
            {
                SoundManager.PlayRandomSFXAtPlayer(new AudioClip[] { SoundManager.PlayerDeathSFX1, SoundManager.PlayerDeathSFX2, SoundManager.PlayerDeathSFX3 });
                PlayerDies();
            }
            else if (!Invulnerable)
            {
                SoundManager.PlayRandomSFXAtPlayer(new AudioClip[] { SoundManager.PlayerHitSFX1, SoundManager.PlayerHitSFX2, SoundManager.PlayerHitSFX3 });
            }
        }
    }
    public void PlayerDies()
    {
        print("PLAYER DEAD!");
        SwitchToAnimation(WVDAnimationStrings.PlayerDieAnimation);
        if (_lifeStealFX.activeSelf)
        {
            _lifeStealFX.SetActive(false);
        }
        foreach (IWVDDamageable drone in _drones)
        {
            drone.GetTransform().gameObject.GetComponent<WVDBaseDrone>().CurrentDroneState = WVDBaseDrone.DroneState.Stopped;
        }
        if (_bossScript.BossInBattle)
        {
            _bossScript.CurrentBossState = WVDBoss.BossState.Victory;
        }
        _gameOverManagerScript.TriggerGameOver();
    }
    public override void ApplyEffects(WVDAttackEffects effects)
    {
        base.ApplyEffects(effects);
        if (effects.DOT)
        {
            ApplyDOT(effects.DOTDamage, effects.DOTInterval, effects.DOTDuration);
        }
    }
    public async void ApplyDOT(int damage, float interval, float duration)
    {
        float endTime = Time.time + duration;
        float intervalTime = Time.time + interval;
        while (Time.time < endTime)
        {
            if (Time.time > intervalTime)
            {
                TakeDamage(damage, true);
                intervalTime = Time.time + interval;
            }
            await Task.Yield();
        }
        TakeDamage(damage, true); // Final damage to make the last damaging tick of damage
    }
    public Transform GetTransform()
    {
        return gameObject.transform;
    }
    public Transform GetModelTransform()
    {
        return _playerModel.transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boss") || other.gameObject.CompareTag("BossShield"))
        {
            TakeDamage(100, true); // insta kill if touch boss
        }
    }
    public enum ShieldVersion
    {
        Regular,
        Reflect,
        Electric
    }
}