using UnityEngine;

public class WVDLaserDroneProjectile : WVDBaseProjectile
{
    [Header("General")]
    public bool Reflected;
    GameObject _parentDrone;

    public override void Start()
    {
        base.Start();
        Reflected = false;
    }
    public void SetParentDrone(GameObject parentDrone)
    {
        _parentDrone = parentDrone;
    }
    void OnTriggerEnter(Collider other)
    {
        print("Laser drone projectile hit: " + other.name);

        // Ignore these
        if (other.gameObject.CompareTag("Fountain") ||
            other.gameObject.CompareTag("PowerUp") ||
            (other.gameObject.CompareTag("DroneShield") && !Reflected))
        {
            return;
        }

        // Reflect in opposite direction
        if (other.gameObject.CompareTag("ShieldDeflect"))
        {
            print("REBOUND PROJECTILE");
            SetProjectileDirection(-Direction);
            Reflected = true;
        }
        else
        {
            // Hit player
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PickUpTrigger"))
            {
                if (!CannotDamageAgain)
                {
                    other.transform.root.gameObject.GetComponent<WVDPlayer>().ResolveAttack(Damage, Effects);
                    CannotDamageAgain = true;
                    DestroyProjectile();
                }
            }
            // Hit other drone
            else if ((other.gameObject.CompareTag("Enemy")))
            {
                if (other.transform.root.gameObject.GetComponent<IWVDDamageable>() != null)
                {
                    if (!CannotDamageAgain && Reflected)
                    {
                        other.transform.root.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(Damage, Effects);
                        CannotDamageAgain = true;
                        DestroyProjectile();
                    }
                }
            }
            // Hit another object
            else
            {
                DestroyProjectile();
            }
        }
    }
    void DestroyProjectile()
    {
        Instantiate(ImpactFX, transform.position, Quaternion.identity);
        SoundManager.PlaySFXAtPoint(SoundManager.DroneLaserCollideSFX, transform.position);
        Destroy(gameObject);
    }
}