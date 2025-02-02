using UnityEngine;

public class WVDDroneShieldBuff : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    WVDBaseDrone _baseDroneScript;

    void OnTriggerEnter(Collider other)
    {
        // Should switch off if its a player projectile or a reflected laser attack
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