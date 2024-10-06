using UnityEngine;

public class WVDPlayerProjectile : WVDBaseProjectile
{
    public override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<IWVDDamageable>() != null)
        {
            other.gameObject.GetComponent<IWVDDamageable>().TakeDamage(Damage);
            print("hit enemy");
        }
        Destroy(gameObject);
    }
}