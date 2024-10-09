using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDLaserDroneProjectile : WVDBaseProjectile
{
    public override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<WVDPlayer>().TakeDamage(Damage);
            print("hit player");
        }
        Destroy(gameObject);
    }
}
