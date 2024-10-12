using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDLaserDroneProjectile : WVDBaseProjectile // todo maybe see if theres some common code in the different enemy projectile types in future
{
    public override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ShieldDeflect"))
        {
            print("REBOUND PROJECTILE");
            SetProjectileDirection(-Direction);
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<WVDPlayer>().TakeDamage(Damage);
                print("hit player");
            }
            else if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<IWVDDamageable>() != null)
            {
                other.gameObject.GetComponent<IWVDDamageable>().TakeDamage(Damage);
                print("hit enemy");
            }
            Destroy(gameObject);
        }

    }
}
