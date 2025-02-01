[System.Serializable]
public struct WVDAttackEffects
{
    public bool Slow;
    public float SlowPercentage;
    public float SlowDuration;
    
    public bool DOT;
    public int DOTDamage;
    public float DOTInterval;
    public float DOTDuration;
    
    public bool Stun;
    public float StunDuration;
    
    public bool Pierce;
    
    public float ExplodeOnDeathChance;
    
    public float CriticalChance;
    
    public float DropRateIncrease;

    public bool LifeSteal; // Should be player only, heal 1 hp on hit

    public void SetToDefault()
    {
        Slow = false;
        SlowPercentage = 0.0f;
        SlowDuration = 0;

        DOT = false;
        DOTDamage = 0;
        DOTInterval = 0.0f;
        DOTDuration = 0.0f;

        Stun = false;
        StunDuration = 0.0f;

        Pierce = false;

        ExplodeOnDeathChance = 0.0f;

        CriticalChance = 0.0f;

        DropRateIncrease = 0.0f;

        LifeSteal = false;
    }
}