public interface IWVDDamageable
{
    void TakeDamage(int damage); // todo may need additional inputs, which could put in a custom struct, like stun, DOT etc.
    bool IsFullyDamaged();
    void DestroyFullyDamaged();
}
