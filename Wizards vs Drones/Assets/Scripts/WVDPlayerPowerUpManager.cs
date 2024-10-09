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

    [Header("Shield")]
    [SerializeField]
    float _shieldDuration;
    [Header("Traps")]
    [Header("Attack")]
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
            ResetPowerUps();
        }    
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
