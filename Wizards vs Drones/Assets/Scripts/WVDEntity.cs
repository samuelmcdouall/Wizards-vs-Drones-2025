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
    float _maxSpeed;
    public float MaxSpeed 
    { 
        get => _maxSpeed; 
        set => _maxSpeed = value; 
    }

    public virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
