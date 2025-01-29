using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WVDLaserDroneProjectile : WVDBaseProjectile // todo maybe see if theres some common code in the different enemy projectile types in future
{
    bool _reflected;
    GameObject _parentDrone;


    public override void Start()
    {
        base.Start();
        _reflected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Laser drone projectile hit: " + other.name);
        if (other.gameObject.CompareTag("Fountain") ||
            other.gameObject.CompareTag("PowerUp"))
        {
            return;
        }
        if (other.gameObject.CompareTag("ShieldDeflect"))
        {
            print("REBOUND PROJECTILE");
            SetProjectileDirection(-Direction);
            _reflected = true;
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
                }
            }
            else if ((other.gameObject.CompareTag("Enemy") && other.transform.root.gameObject != _parentDrone) || // either hit another enemy or hit yourself and the laser has been reflected
                     (other.gameObject.CompareTag("Enemy") && other.transform.root.gameObject == _parentDrone && _reflected))
            {
                if (other.transform.root.gameObject.GetComponent<IWVDDamageable>() != null)
                {
                    if (!CannotDamageAgain)
                    {
                        other.transform.root.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(Damage, Effects);
                        print("hit enemy");
                        CannotDamageAgain = true;
                    }
                }
            }
            if (!(other.gameObject.CompareTag("Enemy") && other.transform.root.gameObject == _parentDrone && !_reflected))
            {
                Instantiate(ImpactFX, transform.position, Quaternion.identity);
                SoundManager.PlaySFXAtPoint(SoundManager.DroneLaserCollideSFX, transform.position);
                Destroy(gameObject);
            }
        }

    }

    public void SetParentDrone(GameObject parentDrone)
    {
        _parentDrone = parentDrone;
    }
}
