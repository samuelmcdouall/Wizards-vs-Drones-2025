using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WVDPlayerUpgrades
{
    public bool ShootThreeArc;

    public bool StunAttacks;
    public float StunAttackDuration;

    public float DropRateIncrease;

    public bool SlowAttacks; // how much, how long
    public float SlowAttackPercentage;
    public float SlowAttackDuration;

    public bool DOTAttacks; // how much, how often, how long
    public int DOTAttackDamage;
    public float DOTAttackInterval;
    public float DOTAttackDuration;

    public bool Pierce;

    //public bool DamageNearby; // how many other targets, range
    //public float DamageNearbyDistance;
    //public int DamageNearbyTargetsNumber;

    public float ExplodeOnDeathChance;

    public float CriticalChance;

    //public bool InstaKill; // %chance
    //public float InstaKillChance;

    public bool HealthIncrease; // one off max increase

    public float AttackSpeedModifier;

    public float DashRechargeModifier;

    public int LowHealthDamageBonus;

    public void SetToDefault()
    {
        ShootThreeArc = false;
        StunAttacks = false;
        StunAttackDuration = 0.0f;
        
        DropRateIncrease = 0.0f;//
        
        SlowAttacks = false;
        SlowAttackPercentage = 0.0f;
        SlowAttackDuration = 0.0f;
        
        DOTAttacks = false;
        DOTAttackDamage = 0;
        DOTAttackInterval = 0.0f;
        DOTAttackDuration = 0.0f;
        
        Pierce = false;

        //DamageNearby = false;//
        //DamageNearbyDistance = 0.0f;
        //DamageNearbyTargetsNumber = 0;
        
        ExplodeOnDeathChance = 0.0f;//
        
        CriticalChance = 0.0f;
        
        HealthIncrease = false; // todo do this when make shop itself
        
        AttackSpeedModifier = 1.0f;

        DashRechargeModifier = 1.0f;
        
        LowHealthDamageBonus = 0;
    }

}
