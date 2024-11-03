using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class WVDBaseDrone : WVDBaseEntity
{
    [Header("General - Base Drone")]
    [SerializeField]
    protected GameObject DestroyPrefab;
    [SerializeField]
    protected GameObject BatteryPickUp;
    protected bool DestroySequenceCompleted;
    protected Vector3 ExplodeOffset = new Vector3(0.0f, 1.0f, 0.0f);
    DroneState _currentDroneState;
    [SerializeField]
    protected float PickUpChance;
    protected float BonusPickUpChanceFromLastHit;
    protected float ExplodeOnDeathChanceFromLastHit;
    [SerializeField]
    protected GameObject ExplodePrefab;

    [Header("Movement - Base Drone")]
    [SerializeField]
    protected float AttackRayCastDistance;
    [SerializeField]
    protected Transform[] RayCastPoints;
    protected NavMeshAgent DroneNMA;
    protected readonly float ChargingTurnFactor = 2.5f;
    [SerializeField]
    protected GameObject DroneModel;

    [Header("Attacking - Base Drone")]
    [SerializeField]
    protected float AttackChargeUpDuration;
    [SerializeField]
    protected float AttackDuration;
    [SerializeField]
    protected float AttackDischargeDuration;
    protected readonly int LayerMask = 1 << 2;

    protected DroneState CurrentDroneState 
    { 
        get => _currentDroneState;
        set
        {
            _currentDroneState = value;
            print($"{gameObject.name} state now set to: {_currentDroneState}");
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        CurrentDroneState = DroneState.Chasing;
        DroneNMA = GetComponent<NavMeshAgent>();
        DroneNMA.speed = MaxNormalSpeed;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        DroneNMA.speed = MaxNormalSpeed;
        for (int i = 0; i < RayCastPoints.Length; i++)
        {
            Debug.DrawRay(RayCastPoints[i].position, RayCastPoints[i].forward * AttackRayCastDistance, Color.magenta);
        }

        if (Stunned)
        {
            DroneNMA.isStopped = true;
            return;
        }
        else
        {
            if (CurrentDroneState == DroneState.Chasing) // If it is chasing, reenable the movement, if it isn't then it'll be stopped because its charging up/attacking etc. todo this should be for all movement states, so if theres another one that involves movement other than chasing then put it here as well
            {
                DroneNMA.isStopped = false;
            }
        }
    }

    public enum DroneState
    {
        Chasing,
        ChargingUp,
        Attacking,
        Discharge // stand still just after attack
    }
}
