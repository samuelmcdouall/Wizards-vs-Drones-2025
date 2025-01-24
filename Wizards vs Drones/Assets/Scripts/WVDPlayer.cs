using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WVDPlayer : WVDBaseEntity, IWVDDamageable
{
    [Header("Model - Player")]
    [SerializeField]
    GameObject _playerModel;

    [Header("Shield - Player")]
    //[SerializeField]
    //float _currentShield;
    //[SerializeField]
    //float _maxShield;
    //[SerializeField]
    //float _rechargeShieldInterval;
    //float _rechargeShieldIntervalTimer;
    //[SerializeField]
    //float _rechargeShieldRate;
    //bool _activateShield;
    //[SerializeField]
    //ShieldState _currentShieldState;
    //[SerializeField]
    //GameObject _shieldFX;
    //[SerializeField]
    //Slider _shieldUI;
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

    [Header("Traps - Player")]
    [SerializeField]
    GameObject _trapSlowPrefab;
    [SerializeField]
    GameObject _trapDamagePrefab;
    [SerializeField]
    GameObject _trapExplosivePrefab;
    readonly float _trapDeploymentOffset = 2.0f;

    [Header("Heal - Player")]
    [SerializeField]
    GameObject _lifeStealFX;
    float _lifeStealTimer;
    bool _lifeSteal;
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

    [Header("Speed - Player")]
    [SerializeField]
    float _dashSpeed;
    public float DashSpeed
    {
        get => _dashSpeed;
        set => _dashSpeed = value;
    }

    [Header("General - Player")]
    List<IWVDDamageable> _drones = new List<IWVDDamageable>();
    [SerializeField]
    WVDGameOverManager _gameOverManagerScript;
    [SerializeField]
    WVDBoss _bossScript;


    [Header("Upgrades")]
    [SerializeField] int _batteryCount;
    [SerializeField] TMP_Text _batteryCountUI;
    public WVDPlayerUpgrades PurchasedUpgrades;

    //[Header("Burning")] // maybe list of enemies in flamethrower range, deal damage * number or list of coroutines?
    //[SerializeField]
    //GameObject _onFireFX;
    //Coroutine _burnCoroutine; // 7:41 in video
    //bool _stoppingBurn;


    //public float CurrentShield
    //{
    //    get => _currentShield;
    //    set
    //    {
    //        if (value > _maxShield)
    //        {
    //            _currentShield = _maxShield;
    //        }
    //        else if (value <= 0)
    //        {
    //            _currentShield = 0;
    //        }
    //        else
    //        {
    //            _currentShield = value;
    //        }
    //        _shieldUI.value = _currentShield / _maxShield;
    //    }
    //}

    //public ShieldState CurrentShieldState
    //{
    //    get => _currentShieldState;
    //    set
    //    {
    //        _currentShieldState = value;
    //        print($"Shield state now set to: {_currentShieldState}");
    //    }
    //}

    //public bool ActivateShield
    //{
    //    get => _activateShield;
    //    set
    //    {
    //        if (_activateShield != value)
    //        {
    //            if (value)
    //            {
    //                print("Activate shield");
    //            }

    //            else
    //            {
    //                print("Deactivate shield");
    //            }
    //        }
    //        _activateShield = value;
    //    }
    //}

    //public bool ShieldFXOn
    //{
    //    get => _shieldFX.activeSelf;
    //    set
    //    {
    //        _shieldFX.SetActive(value);
    //        Invulnerable = value;
    //    }
    //}
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

    public override void Start()
    {
        base.Start();
        CurrentPlayingAnimation = WVDAnimationStrings.PlayerIdleAnimation;
        _shieldRegularFX.SetActive(false);
        _shieldReflectFX.SetActive(false);
        _shieldElectricFX.SetActive(false);
        //PurchasedUpgrades.SetToDefault(); todo put back in here
        //_currentShieldVersion = CurrentShieldVersion.None;
        //InitialShieldSetup();
        _batteryCountUI.text = "" + 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    //private void InitialShieldSetup()
    //{
    //    CurrentShield = _maxShield;
    //    _rechargeShieldIntervalTimer = _rechargeShieldInterval;
    //    _currentShieldState = ShieldState.FullyCharged;
    //    _shieldFX.SetActive(false);
    //}

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
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
            foreach (IWVDDamageable drone in _drones.ToList()) // copy value to a separate list, so if something disappears from the list mid forloop shouldn't be an issue. Possible issue if drone gets destroyed before its checked in the list in same update cycle
            {
                if (Vector3.Distance(drone.GetTransform().position, transform.position) <= _shieldElectricDamageThreshold)
                {
                    drone.TakeDamage(1000); // Insta-kill, something stupidly high
                    WVDShieldElectricAttackFX attackFX = Instantiate(_shieldElectricAttackFXPrefab, transform.position, Quaternion.identity).GetComponent<WVDShieldElectricAttackFX>();
                    Vector3 directionToDrone = (drone.GetModelTransform().position - transform.position).normalized;
                    Vector3 startPos = transform.position + directionToDrone * _shieldAttackOffset;
                    attackFX.SetPositions(startPos, drone.GetModelTransform().position);
                }
            }
        }
        //if (IsFullyDamaged())
        //{
        //    DestroyFullyDamaged();
        //}
        //else
        //{
        //    HandleShieldState();
        //}
    }

    //private void HandleShieldState()
    //{
    //    switch (_currentShieldState)
    //    {
    //        case ShieldState.FullyCharged:
    //            if (_activateShield)
    //            {
    //                _currentShieldState = ShieldState.Using;
    //                ShieldFXOn = true;
    //            }
    //            break;
    //        case ShieldState.Using:
    //            if (!_activateShield || CurrentShield <= 0.0f)
    //            {
    //                _currentShieldState = ShieldState.WaitingToRecharge;
    //                ShieldFXOn = false;
    //            }
    //            else
    //            {
    //                CurrentShield -= Time.deltaTime;
    //            }
    //            break;
    //        case ShieldState.WaitingToRecharge:
    //            if (_activateShield)
    //            {
    //                if (CurrentShield > 0.0f)
    //                {
    //                    _rechargeShieldIntervalTimer = _rechargeShieldInterval;
    //                    _currentShieldState = ShieldState.Using;
    //                    ShieldFXOn = true;
    //                }
    //                else
    //                {
    //                    _rechargeShieldIntervalTimer = _rechargeShieldInterval;
    //                }
    //            }
    //            else if (_rechargeShieldIntervalTimer <= 0.0f)
    //            {
    //                _rechargeShieldIntervalTimer = _rechargeShieldInterval;
    //                _currentShieldState = ShieldState.Recharging;
    //            }
    //            else
    //            {
    //                _rechargeShieldIntervalTimer -= Time.deltaTime;
    //            }
    //            break;
    //        case ShieldState.Recharging:
    //            if (_activateShield && CurrentShield > 0.0f)
    //            {
    //                _rechargeShieldIntervalTimer = _rechargeShieldInterval; // this line may not be needed
    //                _currentShieldState = ShieldState.Using;
    //                ShieldFXOn = true;
    //            }
    //            else if (CurrentShield >= _maxShield)
    //            {
    //                CurrentShield = _maxShield;
    //                _currentShieldState = ShieldState.FullyCharged;
    //            }
    //            else
    //            {
    //                CurrentShield += Time.deltaTime * _rechargeShieldRate;
    //            }
    //            break;
    //    }
    //}

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

    public void PlayerDies()
    {
        print("PLAYER DEAD!");
        SwitchToAnimation(WVDAnimationStrings.PlayerDieAnimation);
        foreach(IWVDDamageable drone in _drones)
        {
            drone.GetTransform().gameObject.GetComponent<WVDBaseDrone>().CurrentDroneState = WVDBaseDrone.DroneState.Stopped;
        }
        if (_bossScript.BossInBattle)
        {
            _bossScript.CurrentBossState = WVDBoss.BossState.Victory;
        }
        _gameOverManagerScript.TriggerGameOver();

    }

    // todo implement this, this is next
    public void TakeDamage(int damage)
    {
        print($"Player took {damage} damage");
        if (!Invulnerable)
        {
            CurrentHealth -= damage;
        }
        print($"Player on {CurrentHealth} health");
        if (IsFullyDamaged())
        {
            PlayerDies();
        }
    }

    //public enum ShieldState // todo as moving this to a power up, this may not be needed
    //{
    //    Using,
    //    WaitingToRecharge,
    //    Recharging,
    //    FullyCharged
    //}

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

    public Transform GetTransform()
    {
        return gameObject.transform;
    }

    public void DeployTrap(TrapVersion trap)
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

        Instantiate(chosenTrap, transform.position + _playerModel.transform.forward * _trapDeploymentOffset + new Vector3(0.0f, -1.0f, 0.0f), _playerModel.transform.rotation);
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

    public void ResolveAttack(int damage, WVDAttackEffects effects)
    {
        TakeDamage(damage);
        ApplyEffects(effects);
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
                TakeDamage(damage);
                intervalTime = Time.time + interval;
            }
            await Task.Yield();
        }
        TakeDamage(damage); // Final damage to make the last damaging tick of damage
    }

    //public void StartBurning(int damagePerSecond)
    //{
    //    _onFireFX.SetActive(true);
    //    //_stoppingBurn = false;  // If hit again whilst still burning, cancel the stop burn order
    //    if (_burnCoroutine != null)
    //    {
    //        StopCoroutine(_burnCoroutine);
    //    }
    //    _burnCoroutine = StartCoroutine(Burn(damagePerSecond));
    //}

    //IEnumerator Burn(int damagePerSecond)
    //{
    //    TakeDamage(damagePerSecond);
    //    WaitForSeconds waitTime = new WaitForSeconds(1.0f);
    //    while (_onFireFX.activeSelf)
    //    {
    //        yield return waitTime;
    //        TakeDamage(damagePerSecond);
    //    }
    //}


    //public IEnumerator StopBurningAfter(float seconds)
    //{
    //    _stoppingBurn = true;
    //    yield return new WaitForSeconds(seconds);
    //    if (_stoppingBurn)
    //    {
    //        StopBurning();
    //    }
    //    else
    //    {
    //        print("BURN ORDER CANCELLED");
    //    }
    //}
    //public void StopBurning()
    //{
    //    _onFireFX.SetActive(false);
    //    //_stoppingBurn = false;
    //    if (_burnCoroutine != null)
    //    {
    //        StopCoroutine(_burnCoroutine);
    //    }
    //}

    public Transform GetModelTransform()
    {
        return _playerModel.transform;
    }

    public enum ShieldVersion
    {
        Regular,
        Reflect,
        Electric
    }

    public enum TrapVersion
    {
        Slow,
        Damage,
        Explosive
    }
}