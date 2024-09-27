using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WVDEntity : MonoBehaviour
{
    [Header("Health")]
    int _currentHealth;
    [SerializeField]
    int _maxHealth;

    public int CurrentHealth 
    { 
        get => _currentHealth; 
        set 
        { 
            if (value > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
            else if (value <= 0)
            {
                _currentHealth = 0;
                OnKilled();
                // todo trigger is dead code here later on
            }
            else
            {
                _currentHealth = value;
            }
        } 
    }


    [Header("Speed")]
    [SerializeField]
    float _maxNormalSpeed;
    public float MaxNormalSpeed 
    { 
        get => _maxNormalSpeed; 
        set => _maxNormalSpeed = value; 
    }

    [Header("Animations")]
    [SerializeField]
    Animator _animator;
    string _currentPlayingAnimation;
    public string CurrentPlayingAnimation 
    { 
        get => _currentPlayingAnimation; 
        set => _currentPlayingAnimation = value; 
    }

    public virtual void Start()
    {
        _currentHealth = _maxHealth;
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

    public virtual void OnKilled()
    {

    }
}
