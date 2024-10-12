using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Speed - Player")]
    [SerializeField]
    float _maxSideBackSpeed;
    public float MaxSideBackSpeed
    {
        get => _maxSideBackSpeed;
        set => _maxSideBackSpeed = value;
    }
    [SerializeField]
    float _dashSpeed;
    public float DashSpeed
    {
        get => _dashSpeed;
        set => _dashSpeed = value;
    }

    [Header("General - Player")]
    List<IWVDDamageable> _drones = new List<IWVDDamageable>();
    

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

    public override void Start()
    {
        base.Start();
        CurrentPlayingAnimation = WVDAnimationStrings.PlayerIdleAnimation;
        _shieldRegularFX.SetActive(false);
        _shieldReflectFX.SetActive(false);
        _shieldElectricFX.SetActive(false);
        //_currentShieldVersion = CurrentShieldVersion.None;
        //InitialShieldSetup();
    }

    //private void InitialShieldSetup()
    //{
    //    CurrentShield = _maxShield;
    //    _rechargeShieldIntervalTimer = _rechargeShieldInterval;
    //    _currentShieldState = ShieldState.FullyCharged;
    //    _shieldFX.SetActive(false);
    //}

    // Update is called once per frame
    void Update()
    {
        if (_shieldElectricFX.activeSelf)
        {
            foreach (IWVDDamageable drone in _drones.ToList()) // copy value to a separate list, so if something disappears from the list mid forloop shouldn't be an issue. Possible issue if drone gets destroyed before its checked in the list in same update cycle
            {
                if (Vector3.Distance(drone.GetTransform().position, transform.position) <= _shieldElectricDamageThreshold)
                {
                    drone.TakeDamage(1000); // Insta-kill, something stupidly high
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

    public bool IsFullyDamaged()
    {
        if (CurrentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    public void DestroyFullyDamaged()
    {
        print("PLAYER DEAD!");
        // Do other things
    }

    // todo implement this, this is next
    public void TakeDamage(int damage)
    {
        print($"Player took {damage} damage");
        CurrentHealth -= damage;
        print($"Player on {CurrentHealth} health");
        if (IsFullyDamaged())
        {
            DestroyFullyDamaged();
        }
    }

    //public enum ShieldState // todo as moving this to a power up, this may not be needed
    //{
    //    Using,
    //    WaitingToRecharge,
    //    Recharging,
    //    FullyCharged
    //}

    public async void SwitchOnShieldForSeconds(ShieldVersion version, float seconds)
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

    public enum ShieldVersion
    {
        Regular,
        Reflect,
        Electric
    }
}