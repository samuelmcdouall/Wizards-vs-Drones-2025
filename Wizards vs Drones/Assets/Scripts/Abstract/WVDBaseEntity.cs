using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WVDBaseEntity : MonoBehaviour
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
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SwitchToAnimation(string animation)
    {
        if (animation != _currentPlayingAnimation)
        {
            _currentPlayingAnimation = animation;
            _animator.Play(animation);
        }
    }
}
