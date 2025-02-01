using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDGrenadePowerUpProjectile : WVDBaseProjectile
{
    [Header("Grenade Explosion")]
    [SerializeField]
    GameObject _explosionPrefab;

    public override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        SoundManager.PlaySFXAtPlayer(SoundManager.GrenadeImpactPowerUpSFX);
        Destroy(gameObject);
    }
}
