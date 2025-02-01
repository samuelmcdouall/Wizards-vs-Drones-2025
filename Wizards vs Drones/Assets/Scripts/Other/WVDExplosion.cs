using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDExplosion : MonoBehaviour
{
    [SerializeField]
    float _lifetime;
    [SerializeField]
    int _damage;
    [SerializeField]
    bool _canDamageEnemies;
    [SerializeField]
    bool _canDamagePlayer;
    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_canDamageEnemies && other.gameObject.CompareTag("Enemy") && other.transform.root.gameObject.GetComponent<IWVDDamageable>() != null)
        {
            other.transform.root.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(_damage, new WVDAttackEffects());
            print("hit enemy with explosive trap");
        }
        if (_canDamagePlayer && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<WVDPlayer>().ResolveAttack(_damage, new WVDAttackEffects());
        }
    }
}
