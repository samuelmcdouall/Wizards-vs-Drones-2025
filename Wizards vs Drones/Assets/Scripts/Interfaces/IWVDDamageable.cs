using UnityEngine;

public interface IWVDDamageable
{
    void ResolveAttack(int damage, WVDAttackEffects effects);
    void TakeDamage(int damage, bool playDamageSFX); // the playDamageSFX is really just for not playing it when the tome is picked up, as that will be very loud

    Transform GetTransform();

    Transform GetModelTransform();
}