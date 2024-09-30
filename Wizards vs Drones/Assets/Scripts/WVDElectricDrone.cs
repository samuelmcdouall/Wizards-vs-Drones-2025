using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDElectricDrone : WVDEntity
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public enum EnemyState
    {
        Chasing,
        ChargingUp,
        Attacking,
        Decharge // stand still just after attack
    }
}
