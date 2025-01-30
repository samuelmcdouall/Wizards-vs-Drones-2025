using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WVDLaserDroneProjectile : WVDBaseProjectile // todo maybe see if theres some common code in the different enemy projectile types in future
{
    public bool Reflected;
    GameObject _parentDrone;


    public override void Start()
    {
        base.Start();
        Reflected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Laser drone projectile hit: " + other.name);
        if (other.gameObject.CompareTag("Fountain") ||
            other.gameObject.CompareTag("PowerUp") ||
            (other.gameObject.CompareTag("DroneShield") && !Reflected))
        {
            return;
        }
        if (other.gameObject.CompareTag("ShieldDeflect"))
        {
            print("REBOUND PROJECTILE");
            SetProjectileDirection(-Direction);
            Reflected = true;
        }
        else
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PickUpTrigger"))
            {
                if (!CannotDamageAgain)
                {
                    other.transform.root.gameObject.GetComponent<WVDPlayer>().ResolveAttack(Damage, Effects); // todo might just use IWVDDamageable here as well
                    print("hit player");
                    CannotDamageAgain = true;
                    DestroyProjectile();
                }
            }
            else if ((other.gameObject.CompareTag("Enemy")))
            {
                if (other.transform.root.gameObject.GetComponent<IWVDDamageable>() != null)
                {
                    if (!CannotDamageAgain && Reflected)
                    {
                        other.transform.root.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(Damage, Effects);
                        print("hit enemy");
                        CannotDamageAgain = true;
                        DestroyProjectile();
                    }
                }
            }
            else
            {
                DestroyProjectile();
            }
            //if (!(other.gameObject.CompareTag("Enemy") && !Reflected))// || !(other.gameObject.CompareTag("DroneShield") && !Reflected))
            //{

            //}
        }

    }

    public void SetParentDrone(GameObject parentDrone)
    {
        _parentDrone = parentDrone;
    }

    void DestroyProjectile()
    {
        Instantiate(ImpactFX, transform.position, Quaternion.identity);
        SoundManager.PlaySFXAtPoint(SoundManager.DroneLaserCollideSFX, transform.position);
        Destroy(gameObject);
    }
}
