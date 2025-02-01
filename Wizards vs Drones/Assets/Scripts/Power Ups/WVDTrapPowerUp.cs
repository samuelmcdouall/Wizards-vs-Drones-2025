using UnityEngine;

public class WVDTrapPowerUp : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    TrapType _trapType;

    [Header("Slow Trap")]
    [SerializeField]
    float _slowPercentage;
    [SerializeField]
    float _slowDuration;
    WVDAttackEffects _trapEffects = new WVDAttackEffects();

    [Header("Damage Trap")]
    [SerializeField]
    int _damage;

    [Header("Explosive Trap")]
    [SerializeField]
    GameObject _explosionPrefab;

    void Start()
    {
        _trapEffects.SetToDefault();
        if (_trapType == TrapType.Slow)
        {
            _trapEffects.Slow = true;
            _trapEffects.SlowPercentage = _slowPercentage;
            _trapEffects.SlowDuration = _slowDuration;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.transform.root.gameObject.GetComponent<IWVDDamageable>() != null)
        {
            other.transform.root.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(_damage, _trapEffects);
            if (_trapType == TrapType.Explosive)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            }
            print($"Enemy ran into {_trapType}");
            Destroy(gameObject);
        }
    }
    public enum TrapType
    {
        Slow,
        Damage,
        Explosive
    }
}