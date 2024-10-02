using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WVDElectricDrone : WVDEntity
{
    ElectricDroneState _currentElectricDroneState;
    NavMeshAgent _electricDroneNMA;

    public ElectricDroneState CurrentElectricDroneState 
    { 
        get => _currentElectricDroneState;
        set
        {
            _currentElectricDroneState = value;
            print($"Electric Drone State now set to: {_currentElectricDroneState}");
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _currentElectricDroneState = ElectricDroneState.Chasing;
        _electricDroneNMA = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentElectricDroneState == ElectricDroneState.Chasing)
        {
            _electricDroneNMA.SetDestination(Player.transform.position);
        }

    }

    public enum ElectricDroneState
    {
        Chasing,
        ChargingUp,
        Attacking,
        Decharge // stand still just after attack
    }
}
