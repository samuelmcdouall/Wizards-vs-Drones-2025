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
    [SerializeField]
    protected WVDAttackEffects Effects;
    public bool CannotDamageAgain; // this is in case of hitting again before being fully destroyed

    [Header("SFX/FX - General")]
    [SerializeField]
    protected GameObject ImpactFX;
    protected WVDSoundManager SoundManager;

    public Vector3 Direction
    {
        get => _direction;
        set => _direction = value;
    }

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = _direction * _speed;
        SoundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
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

    public void SetProjectileEffects(WVDAttackEffects effects)
    {
        Effects = effects;
    }
}
