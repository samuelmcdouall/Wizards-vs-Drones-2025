using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public abstract class WVDBaseEntity : MonoBehaviour, IWVDAffectable
{
    [Header("Health - General")]
    int _currentHealth;
    [SerializeField]
    int _maxHealth;
    [SerializeField]
    Slider _healthUI;

    public int CurrentHealth 
    { 
        get => _currentHealth; 
        set 
        {
            if (_invulnerable) // Cannot go down, but go up, staying within max bound
            {
                if (value > _currentHealth)
                {
                    _currentHealth = value;
                }
                if (value > _maxHealth)
                {
                    _currentHealth = _maxHealth;
                }
            }
            else // Otherwise can go up or down, but stays within bounds
            {
                if (value > _maxHealth)
                {
                    _currentHealth = _maxHealth;
                }
                else if (value <= 0)
                {
                    _currentHealth = 0;
                }
                else
                {
                    _currentHealth = value;
                }
            }
            if (_healthUI)
            {
                _healthUI.value = (float)_currentHealth / (float)_maxHealth;
            }
        } 
    }
    bool _invulnerable;
    [SerializeField] public bool Invulnerable 
    { 
        get => _invulnerable;
        set
        {
            print($"Invulnerable: {value}");
            _invulnerable = value;
        }
    }


    [Header("Speed - General")]
    [SerializeField]
    float _maxNormalSpeed;
    float _initialMaxNormalSpeed;
    float _slowedTimer;
    bool _slowed;
    public float MaxNormalSpeed 
    { 
        get => _maxNormalSpeed; 
        set => _maxNormalSpeed = value; 
    }

    [Header("Animations - General")]
    [SerializeField]
    Animator _animator;
    string _currentPlayingAnimation;
    public string CurrentPlayingAnimation 
    { 
        get => _currentPlayingAnimation; 
        set => _currentPlayingAnimation = value; 
    }

    [Header("Other - General")]
    [System.NonSerialized]
    public GameObject Player;

    public virtual void Start()
    {
        _currentHealth = _maxHealth;
        _invulnerable = false;
        _initialMaxNormalSpeed = _maxNormalSpeed;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public virtual void Update()
    {
        if (_slowed)
        {
            if (_slowedTimer <= 0.0f)
            {
                print("Entity no longer slowed");
                _slowed = false;
                _maxNormalSpeed = _initialMaxNormalSpeed;
            }
            else
            {
                _slowedTimer -= Time.deltaTime;
            }
        }
    }

    public void SwitchToAnimation(string animation)
    {
        if (animation != _currentPlayingAnimation)
        {
            _currentPlayingAnimation = animation;
            _animator.Play(animation);
        }
    }

    public void ApplyEffects(WVDAttackEffects effects)
    {
        if (effects.Slow)
        {
            ApplySlow(effects.SlowPercentage, effects.SlowDuration);
        }
    }

    public void ApplySlow(float percentage, float time)
    {
        // If a slow is applied that would reduce the speed to lower (accounting for multiple slow sources) then apply new slow
        if (_maxNormalSpeed > _initialMaxNormalSpeed * percentage)
        {
            print($"Entity set to {percentage} speed");
            _maxNormalSpeed = _initialMaxNormalSpeed * percentage;
            _slowed = true;
        }

        // If a time is applied that would be larger than the time remaining then apply new time
        if (time > _slowedTimer)
        {
            _slowedTimer = time;
        }
    }
}
