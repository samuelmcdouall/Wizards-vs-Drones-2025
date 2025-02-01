using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDDroneShieldBuff : MonoBehaviour
{
    [SerializeField]
    WVDBaseDrone _baseDroneScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile") || (other.gameObject.CompareTag("Projectile") && other.gameObject.GetComponent<WVDLaserDroneProjectile>().Reflected))
        {
            _baseDroneScript.ShieldOn = false;
            WVDLaserDroneProjectile projectile = other.gameObject.GetComponent<WVDLaserDroneProjectile>();
            if (projectile)
            {
                projectile.CannotDamageAgain = true;
            }
            Destroy(other.gameObject);
        }
    }
}
