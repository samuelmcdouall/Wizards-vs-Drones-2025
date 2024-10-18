using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WVDAttackEffects
{
    //public bool NoEffects; // true if all other variables are false/0.0f/0
    public bool Slow; // how much, how long
    public float SlowPercentage;
    public float SlowDuration;
    
    public bool DOT; // how much, how often, how long
    public int DOTDamage;
    public float DOTInterval;
    public float DOTDuration;
    
    public bool Stun; // how long
    public float StunDuration;
    
    public bool Pierce;
    
    public bool DamageNearby; // how many other targets
    public int DamageNearbyTargetsNumber;
    
    public float ExplodeOnDeathChance;
    
    public bool Critical; // %chance
    public float CriticalChance;
    
    //public bool InstaKill; // %chance
    //public float InstaKillChance;
    
    public float DropRateIncrease;

    public void SetToDefault()
    {
        //NoEffects = true;
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
        
        DamageNearby = false;
        DamageNearbyTargetsNumber = 0;
        
        ExplodeOnDeathChance = 0.0f;
        
        CriticalChance = 0.0f;
        
        //InstaKill = false;
        //InstaKillChance = 0.0f;
        
        DropRateIncrease = 0.0f;
    }


}
