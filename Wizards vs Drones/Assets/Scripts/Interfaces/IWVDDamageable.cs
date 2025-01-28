using UnityEngine;

public interface IWVDDamageable
{
    void ResolveAttack(int damage, WVDAttackEffects effects);
    void TakeDamage(int damage, bool playDamageSFX); // the playDamageSFX is really just for not playing it when the tome is picked up, as that will be very loud
    //void DestroyFullyDamaged(); todo probably won't need in this interface

    Transform GetTransform();

    Transform GetModelTransform();
}
