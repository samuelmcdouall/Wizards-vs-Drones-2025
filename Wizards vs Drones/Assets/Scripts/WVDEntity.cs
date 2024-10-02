using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WVDEntity : MonoBehaviour
{
    [Header("Health - General")]
    int _currentHealth;
    [SerializeField]
    int _maxHealth;

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
                    // todo trigger is dead code here later on
                }
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
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
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
