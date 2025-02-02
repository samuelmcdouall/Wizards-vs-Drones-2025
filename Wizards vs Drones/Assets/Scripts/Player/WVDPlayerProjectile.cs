using UnityEngine;

public class WVDPlayerProjectile : WVDBaseProjectile
{
    [Header("General")]
    bool _canPierce;
    [System.NonSerialized]
    public WVDPlayer PlayerScript;
    GameObject _hitEnemy;

    public override void Start()
    {
        base.Start();
        _canPierce = Effects.Pierce;
        float randCrit = Random.Range(0.0f, 1.0f);
        if (randCrit < Effects.CriticalChance)
        {
            Damage *= 2;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        print("PROJECTILE HIT: " + other.gameObject.name);
        if (CannotDamageAgain)
        {
            return;
        }
        if (other.gameObject.CompareTag("DroneShield"))
        {
            DestroyProjectile();
        }
        // Ignore any of these
        else if (other.gameObject.CompareTag("Player") ||
            other.gameObject.CompareTag("PlayerProjectile") ||
            other.gameObject.CompareTag("PickUpTrigger") ||
            other.gameObject.CompareTag("PowerUp") ||
            other.gameObject.CompareTag("Fountain") || 
            other.gameObject.CompareTag("EnemyHitBox") ||
            other.gameObject.CompareTag("BossFireStream") ||
            other.gameObject.CompareTag("ShieldRegular") ||
            other.gameObject.CompareTag("ShieldDeflect") ||
            other.gameObject.CompareTag("ShieldElectric"))
        {
            return;
        }
        else if (other.gameObject.CompareTag("Enemy") && 
                 other.transform.root.gameObject.GetComponent<IWVDDamageable>() != null
                 )
        {
            if (other.transform.root.gameObject != _hitEnemy)
            {
                print("hit enemy");
                if (Effects.LifeSteal)
                {
                    PlayerScript.CurrentHealth++;
                }

                if (!CannotDamageAgain)
                {
                    other.transform.root.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(Damage, Effects);
                }

                if (_hitEnemy == null)
                {
                    _hitEnemy = other.transform.root.gameObject;
                }

                if (_canPierce)
                {
                    print("Pierced enemy");
                    _canPierce = false;
                }
                else
                {
                    DestroyProjectile();
                }
            }
        }
        else if (other.gameObject.CompareTag("Boss"))
        {
            if (!CannotDamageAgain)
            {
                other.gameObject.GetComponent<WVDBoss>().TakeDamage(Damage);
            }
            DestroyProjectile();

        }
        else if (!other.gameObject.CompareTag("InvisibleWall"))
        {
            DestroyProjectile();
        }
    }
    void DestroyProjectile()
    {
        CannotDamageAgain = true;
        Instantiate(ImpactFX, transform.position, Quaternion.identity);
        SoundManager.PlaySFXAtPoint(SoundManager.PlayerProjectileImpactSFX, transform.position);
        Destroy(gameObject);
    }
}