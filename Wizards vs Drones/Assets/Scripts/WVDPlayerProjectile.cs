using UnityEngine;

public class WVDPlayerProjectile : WVDBaseProjectile
{
    bool _canPierce;
    [System.NonSerialized]
    public WVDPlayer PlayerScript;
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
        if (other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Fountain"))
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
                return;
            }
        }
        Destroy(gameObject);
    }
}