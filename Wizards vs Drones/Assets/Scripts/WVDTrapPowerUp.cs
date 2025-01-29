using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDTrapPowerUp : MonoBehaviour
{
    WVDAttackEffects _trapEffects = new WVDAttackEffects();

    [Header("Trap Stats")]
    [SerializeField]
    TrapType _trapType;
    [SerializeField]
    float _slowPercentage;
    [SerializeField]
    float _slowDuration;
    [SerializeField]
    int _damage;
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

    private void OnTriggerEnter(Collider other)
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
