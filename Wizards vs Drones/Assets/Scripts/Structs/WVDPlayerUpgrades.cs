[System.Serializable]
public struct WVDPlayerUpgrades
{
    public bool ShootThreeArc;

    public bool StunAttacks;
    public float StunAttackDuration;

    public float DropRateIncrease;

    public bool SlowAttacks;
    public float SlowAttackPercentage;
    public float SlowAttackDuration;

    public bool DOTAttacks;
    public int DOTAttackDamage;
    public float DOTAttackInterval;
    public float DOTAttackDuration;

    public bool Pierce;

    public float ExplodeOnDeathChance;

    public float CriticalChance;

    public bool HealthIncrease;

    public float AttackSpeedModifier;

    public float DashRechargeModifier;

    public int LowHealthDamageBonus;

    public void SetToDefault()
    {
        ShootThreeArc = false;

        StunAttacks = false;
        StunAttackDuration = 0.0f;

        DropRateIncrease = 0.0f;

        SlowAttacks = false;
        SlowAttackPercentage = 0.0f;
        SlowAttackDuration = 0.0f;

        DOTAttacks = false;
        DOTAttackDamage = 0;
        DOTAttackInterval = 0.0f;
        DOTAttackDuration = 0.0f;

        Pierce = false;

        ExplodeOnDeathChance = 0.0f;

        CriticalChance = 0.0f;

        HealthIncrease = false;

        AttackSpeedModifier = 1.0f;

        DashRechargeModifier = 1.0f;

        LowHealthDamageBonus = 0;
    }
}