using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDEntity : MonoBehaviour
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
    int _currentSpeed;
    [SerializeField]
    int _maxSpeed;
    [SerializeField]
    int _minSpeed;
    public int CurrentSpeed 
    { 
        get => _currentSpeed;
        set
        {
            if (value > _maxSpeed)
            {
                _currentSpeed = _maxSpeed;
            }
            else if (value < _minSpeed)
            {
                _currentSpeed = _minSpeed;
            }
            else
            {
                _currentSpeed = value;
            }
        }
    }

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
