using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDBossProjectile : WVDBaseProjectile
{
    public override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        print("PROJECTILE HIT: " + other.gameObject.name);
        if (
            other.gameObject.CompareTag("PickUpTrigger") ||
            other.gameObject.CompareTag("Fountain") ||
            other.gameObject.CompareTag("Boss"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<IWVDDamageable>() != null)
        {
            other.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(Damage, Effects);
        }
        else if (other.gameObject.CompareTag("Flammable"))
        {
            other.gameObject.GetComponent<WVDFlammable>().BurnObject(transform.position);
        }
        if (!other.gameObject.CompareTag("InvisibleWall"))
        {
            Instantiate(ImpactFX, transform.position, Quaternion.identity);
            SoundManager.PlaySFXAtPoint(SoundManager.BossProjectileImpactSFX, transform.position);
            Destroy(gameObject);
        }
    }
}
