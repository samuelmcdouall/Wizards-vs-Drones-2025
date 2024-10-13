using UnityEngine;

public abstract class WVDBaseProjectile : MonoBehaviour
{
    [Header("Movement - General")]
    [SerializeField]
    float _speed;
    Vector3 _direction;
    Rigidbody _rb;
    [SerializeField]
    float _lifeTime;

    [Header("Damage - General")]
    public int Damage;
    public WVDAttackEffects Effects;

    [Header("SFX/FX - General")]
    [SerializeField]
    GameObject _impactFX; // todo change this to public and access in other classes

    public Vector3 Direction 
    { 
        get => _direction; 
        set => _direction = value; 
    }

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = _direction * _speed;
        Effects.SetToDefault();
        Destroy(gameObject, _lifeTime);
    }

    public void SetProjectileDirection(Vector3 direction)
    {
        _direction = direction.normalized;
        // If it changes later on (when we've been given the RB) then change the velocity as well
        if (_rb)
        {
            _rb.velocity = _direction * _speed;
        }
    }
}
