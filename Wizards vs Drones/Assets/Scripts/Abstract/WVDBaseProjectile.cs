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

    [Header("SFX/FX - General")]
    [SerializeField]
    GameObject _impactFX; // todo change this to public and access in other classes

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = _direction * _speed;
        Destroy(gameObject, _lifeTime);
    }

    public void SetProjectileDirection(Vector3 direction)
    {
        _direction = direction.normalized;
    }
}
