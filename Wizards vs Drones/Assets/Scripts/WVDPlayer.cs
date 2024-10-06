using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WVDPlayer : WVDBaseEntity, IWVDDamageable
{
    [Header("Model - Player")]
    [SerializeField]
    GameObject _playerModel;

    [Header("Shield - Player")]
    [SerializeField]
    float _currentShield;
    [SerializeField]
    float _maxShield;
    [SerializeField]
    float _rechargeShieldInterval;
    float _rechargeShieldIntervalTimer;
    [SerializeField]
    float _rechargeShieldRate;
    bool _activateShield;
    [SerializeField]
    ShieldState _currentShieldState;
    [SerializeField]
    GameObject _shieldFX;
    [SerializeField]
    Slider _shieldUI;

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

    public float CurrentShield
    {
        get => _currentShield;
        set
        {
            if (value > _maxShield)
            {
                _currentShield = _maxShield;
            }
            else if (value <= 0)
            {
                _currentShield = 0;
            }
            else
            {
                _currentShield = value;
            }
            _shieldUI.value = _currentShield / _maxShield;
        }
    }

    public ShieldState CurrentShieldState 
    { 
        get => _currentShieldState;
        set 
        {
            _currentShieldState = value;
            print($"Shield state now set to: {_currentShieldState}");
        }
    }

    public bool ActivateShield 
    { 
        get => _activateShield;
        set
        {
            if (_activateShield != value)
            {
                if (value)
                {
                    print("Activate shield");
                }

                else
                {
                    print("Deactivate shield");
                }
            }
            _activateShield = value;
        }
    }

    public bool ShieldFXOn 
    { 
        get => _shieldFX.activeSelf;
        set
        {
            _shieldFX.SetActive(value);
            Invulnerable = value;
        }
    }
    public bool PlayerModelOn
    {
        get => _playerModel.activeSelf;
        set => _playerModel.SetActive(value);
    }

    public override void Start()
    {
        base.Start();
        CurrentPlayingAnimation = WVDAnimationStrings.PlayerIdleAnimation;
        InitialShieldSetup();
    }

    private void InitialShieldSetup()
    {
        CurrentShield = _maxShield;
        _rechargeShieldIntervalTimer = _rechargeShieldInterval;
        _currentShieldState = ShieldState.FullyCharged;
        _shieldFX.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (IsFullyDamaged())
        //{
        //    DestroyFullyDamaged();
        //}
        //else
        {
            HandleShieldState();
        }
    }

    private void HandleShieldState()
    {
        switch (_currentShieldState)
        {
            case ShieldState.FullyCharged:
                if (_activateShield)
                {
                    _currentShieldState = ShieldState.Using;
                    ShieldFXOn = true;
                }
                break;
            case ShieldState.Using:
                if (!_activateShield || CurrentShield <= 0.0f)
                {
                    _currentShieldState = ShieldState.WaitingToRecharge;
                    ShieldFXOn = false;
                }
                else
                {
                    CurrentShield -= Time.deltaTime;
                }
                break;
            case ShieldState.WaitingToRecharge:
                if (_activateShield)
                {
                    if (CurrentShield > 0.0f)
                    {
                        _rechargeShieldIntervalTimer = _rechargeShieldInterval;
                        _currentShieldState = ShieldState.Using;
                        ShieldFXOn = true;
                    }
                    else
                    {
                        _rechargeShieldIntervalTimer = _rechargeShieldInterval;
                    }
                }
                else if (_rechargeShieldIntervalTimer <= 0.0f)
                {
                    _rechargeShieldIntervalTimer = _rechargeShieldInterval;
                    _currentShieldState = ShieldState.Recharging;
                }
                else
                {
                    _rechargeShieldIntervalTimer -= Time.deltaTime;
                }
                break;
            case ShieldState.Recharging:
                if (_activateShield && CurrentShield > 0.0f)
                {
                    _rechargeShieldIntervalTimer = _rechargeShieldInterval; // this line may not be needed
                    _currentShieldState = ShieldState.Using;
                    ShieldFXOn = true;
                }
                else if (CurrentShield >= _maxShield)
                {
                    CurrentShield = _maxShield;
                    _currentShieldState = ShieldState.FullyCharged;
                }
                else
                {
                    CurrentShield += Time.deltaTime * _rechargeShieldRate;
                }
                break;
        }
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
        print("Destroyed!");
        // Do other things
    }

    // todo implement this
    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }



    public enum ShieldState
    {
        Using,
        WaitingToRecharge,
        Recharging,
        FullyCharged
    }

}
