using UnityEngine;

public interface IWVDDamageable
{
    void ResolveAttack(int damage, WVDAttackEffects effects);
    void TakeDamage(int damage); // todo may need additional inputs, which could put in a custom struct, like stun, DOT etc.
    bool IsFullyDamaged();
    void DestroyFullyDamaged();

    Transform GetTransform();
}
