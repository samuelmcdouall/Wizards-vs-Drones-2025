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
        if (other.gameObject.CompareTag("Fountain") ||
            other.gameObject.CompareTag("PowerUp"))
        {
            return;
        }
        if (other.gameObject.CompareTag("ShieldDeflect"))
        {
            print("REBOUND PROJECTILE");
            SetProjectileDirection(-Direction);
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<WVDPlayer>().ResolveAttack(Damage, Effects); // todo might just use IWVDDamageable here as well
                print("hit player");
            }
            else if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<IWVDDamageable>() != null)
            {
                other.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(Damage, Effects);
                print("hit enemy");
            }
            Instantiate(ImpactFX, transform.position, Quaternion.identity);
            SoundManager.PlaySFXAtPoint(SoundManager.DroneLaserCollideSFX, transform.position);
            Destroy(gameObject);
        }

    }
}
