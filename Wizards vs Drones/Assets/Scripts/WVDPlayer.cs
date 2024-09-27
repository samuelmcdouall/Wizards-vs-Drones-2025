using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPlayer : WVDEntity
{
    [Header("Shield")]
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
            _activateShield = value;
            if (_activateShield)
            {
                print("Shield on");
            }
            else
            {
                print("Shield off");
            }
        }
    }

    public override void Start()
    {
        base.Start();
        CurrentPlayingAnimation = WVDAnimationStrings.PlayerIdleAnimation;
        InitialShieldSetup();
    }

    private void InitialShieldSetup()
    {
        _currentShield = _maxShield;
        _rechargeShieldIntervalTimer = _rechargeShieldInterval;
        _currentShieldState = ShieldState.FullyCharged;
        _shieldFX.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleShieldState();
    }

    private void HandleShieldState()
    {
        switch (_currentShieldState)
        {
            case ShieldState.FullyCharged:
                if (_activateShield)
                {
                    _currentShieldState = ShieldState.Using;
                    _shieldFX.SetActive(true);
                }
                break;
            case ShieldState.Using:
                if (!_activateShield || _currentShield <= 0.0f)
                {
                    _currentShield = 0.0f;
                    _currentShieldState = ShieldState.WaitingToRecharge;
                    _shieldFX.SetActive(false);
                }
                else
                {
                    _currentShield -= Time.deltaTime;
                }
                break;
            case ShieldState.WaitingToRecharge:
                if (_activateShield)
                {
                    if (_currentShield > 0.0f)
                    {
                        _rechargeShieldIntervalTimer = _rechargeShieldInterval;
                        _currentShieldState = ShieldState.Using;
                        _shieldFX.SetActive(true);
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
                if (_activateShield && _currentShield > 0.0f)
                {
                    _rechargeShieldIntervalTimer = _rechargeShieldInterval; // this line may not be needed
                    _currentShieldState = ShieldState.Using;
                    _shieldFX.SetActive(true);
                }
                else if (_currentShield >= _maxShield)
                {
                    _currentShield = _maxShield;
                    _currentShieldState = ShieldState.FullyCharged;
                }
                else
                {
                    _currentShield += Time.deltaTime * _rechargeShieldRate;
                }
                break;
        }
    }

    public bool IsShieldOn()
    {
        return _shieldFX.activeSelf;
    }

    public enum ShieldState
    {
        Using,
        WaitingToRecharge,
        Recharging,
        FullyCharged
    }

}
