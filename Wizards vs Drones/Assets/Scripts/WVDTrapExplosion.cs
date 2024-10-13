using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDTrapExplosion : MonoBehaviour
{
    [SerializeField]
    float _lifetime;
    [SerializeField]
    int _damage;
    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<IWVDDamageable>() != null)
        {
            other.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(_damage, new WVDAttackEffects());
            print("hit enemy with explosive trap");
        }
    }
}
