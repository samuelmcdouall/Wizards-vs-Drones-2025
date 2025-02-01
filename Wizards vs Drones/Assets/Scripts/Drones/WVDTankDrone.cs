using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDTankDrone : WVDElectricDrone, IWVDDamageable
{
    [Header("Enrage - Tank Drone")]
    [SerializeField]
    bool _enraged;
    [SerializeField]
    int _healthenrageThreshold;
    [SerializeField]
    float _enrageMaxSpeed;
    [SerializeField]
    float _enrageTurnFactor;
    [SerializeField]
    float _enrageAttackChargeUpDuration;
    [SerializeField]
    float _enrageAttackDuration;
    [SerializeField]
    float _enrageAttackDischargeDuration;
    public override void TakeDamage(int damage, bool playDamageSFX) // for tank, do an override of this + need a go enrage function to increase stats + (maybe this not needed)do a start/update with just the base function
    {
        print($"Tank drone took {damage} damage");
        CurrentHealth -= damage;

        if (playDamageSFX)
        {
            SoundManager.PlayRandomSFXAtPlayer(new AudioClip[] { SoundManager.DroneTakeDamageSFX1, SoundManager.DroneTakeDamageSFX2 });
        }
        if (IsFullyDamaged())
        {
            if (!DestroySequenceCompleted)
            {
                DestroySequenceCompleted = true;
                DestroyFullyDamaged();
            }
        }
        else if (!_enraged && CurrentHealth <= _healthenrageThreshold) // if you want a charge up to show going rage then would have to have it as a separate state and this class would need to be rejigged a little, the TransitionToStateAfterDelay would have to be its own version to get that "enraging" state
        {
            DroneEnrages();
        }
    }

    void DroneEnrages()
    {
        _enraged = true;
        MaxNormalSpeed = _enrageMaxSpeed;
        ChargingTurnFactor = _enrageTurnFactor;
        AttackChargeUpDuration = _enrageAttackChargeUpDuration;
        AttackDuration = _enrageAttackDuration;
        AttackDischargeDuration = _enrageAttackDischargeDuration;
    }
}