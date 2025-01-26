using UnityEngine;

public class WVDPlayerProjectile : WVDBaseProjectile
{
    bool _canPierce;
    [System.NonSerialized]
    public WVDPlayer PlayerScript;
    bool _noFurtherDamage;

    public override void Start()
    {
        base.Start();
        _canPierce = Effects.Pierce;
        float randCrit = Random.Range(0.0f, 1.0f); // todo will want some effects/sfx here to indicate crit
        if (randCrit < Effects.CriticalChance)
        {
            Damage *= 2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("PROJECTILE HIT: " + other.gameObject.name);
        if (_noFurtherDamage)
        {
            return;
        }
        if (other.gameObject.CompareTag("DroneShield"))
        {
            _noFurtherDamage = true; // stops player projectile from doing any more if it travels beyond the shield before being destroyed, shouldn't really be an issue most of the time but this is here just in case
        }
        if (other.gameObject.CompareTag("Player") ||
            other.gameObject.CompareTag("PlayerProjectile") ||
            other.gameObject.CompareTag("PickUpTrigger") ||
            other.gameObject.CompareTag("PowerUp") ||
            other.gameObject.CompareTag("Fountain") || 
            other.gameObject.CompareTag("EnemyHitBox") ||
            other.gameObject.CompareTag("BossFireStream"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<IWVDDamageable>() != null)
        {
            if (Effects.LifeSteal)
            {
                PlayerScript.CurrentHealth++;
            }
            other.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(Damage, Effects);
            print("hit enemy");
            if (_canPierce)
            {
                _canPierce = false;
                if (!other.gameObject.CompareTag("InvisibleWall"))
                {
                    Instantiate(ImpactFX, transform.position, Quaternion.identity);
                    SoundManager.PlaySFXAtPoint(SoundManager.PlayerProjectileImpactSFX, transform.position);
                    Destroy(gameObject);
                }
                return;
            }
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            other.gameObject.GetComponent<WVDBoss>().TakeDamage(Damage);

        }
        if (!other.gameObject.CompareTag("InvisibleWall"))
        {
            Instantiate(ImpactFX, transform.position, Quaternion.identity);
            SoundManager.PlaySFXAtPoint(SoundManager.PlayerProjectileImpactSFX, transform.position);
            Destroy(gameObject);
        }
    }
}