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
    DroneBuff _selectedDroneBuff;
    WVDLevelManager _levelManagerScript;
    [SerializeField] WVDDroneSpawner _droneSpawner; // todo just to see if getting ref

    [Header("Movement - Base Drone")]
    [SerializeField]
    protected float AttackRayCastDistance;
    [SerializeField]
    protected Transform[] RayCastPoints;
    protected NavMeshAgent DroneNMA;
    [SerializeField]
    protected float ChargingTurnFactor = 2.5f;
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

    [Header("Buffs - Base Drone")]
    [SerializeField]
    GameObject _spawnDronedFromBuff; // should be electric one
    [SerializeField]
    GameObject _spawnDroneBuffIndicator;
    [SerializeField]
    bool _isSpawnedFromBuff; // i.e. shouldn't be added to tally or roll for having a drone buff
    [SerializeField]
    float _spawnDroneRangeMin;
    [SerializeField]
    float _spawnDroneRangeMax;


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

        _levelManagerScript = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<WVDLevelManager>();

        if (!_isSpawnedFromBuff)
        {
            DetermineDroneBuff();
        }
        else
        {
            _selectedDroneBuff = DroneBuff.None;
        }
    }

    void DetermineDroneBuff()
    {
        WVDDroneSpawnRound currentRoundStats = _droneSpawner.DronesPerRound[_levelManagerScript.Level];

        float spawnChance = currentRoundStats.SpawnOnDeathChance;
        float shieldChance = currentRoundStats.ShieldChance;
        float radiationChance = currentRoundStats.RadiationChance;
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < spawnChance)
        {
            _selectedDroneBuff = DroneBuff.SpawnOnDeath;
            _spawnDroneBuffIndicator.SetActive(true);
        }
        else if (rand < spawnChance + shieldChance)
        {
            _selectedDroneBuff = DroneBuff.Shield;
        }
        else if (rand < spawnChance + shieldChance + radiationChance)
        {
            _selectedDroneBuff = DroneBuff.Radiation;
        }
        else
        {
            _selectedDroneBuff = DroneBuff.None;
        }
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

    public virtual void DestroyFullyDamaged()
    {
        // todo add in fx
        print($"{gameObject.name} drone destroyed");
        Instantiate(DestroyPrefab, transform.position + ExplodeOffset, DestroyPrefab.transform.rotation);
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < PickUpChance + BonusPickUpChanceFromLastHit)
        {
            Instantiate(BatteryPickUp, transform.position + ExplodeOffset, BatteryPickUp.transform.rotation);
        }
        rand = Random.Range(0.0f, 1.0f);
        if (rand < ExplodeOnDeathChanceFromLastHit)
        {
            Instantiate(ExplodePrefab, transform.position + ExplodeOffset, ExplodePrefab.transform.rotation);
        }


        if (_selectedDroneBuff == DroneBuff.SpawnOnDeath)
        {
            SpawnDroneFromBuff();
            SpawnDroneFromBuff();
        }

        _droneSpawner.CurrentDronesSpawned--;
        _droneSpawner.LevelDronesRemaining--;
    }

    private void SpawnDroneFromBuff()
    {
        Vector3 pos = RandomTeleportPosition();
        NavMeshHit hit;
        int i = 0;
        while (!NavMesh.SamplePosition(pos, out hit, 1.0f, NavMesh.AllAreas))
        {
            pos = RandomTeleportPosition();
            if (i == 1000)
            {
                break;
            }
            i++;
        }
        if (i == 1000) // this is to stop infintie loop if can't make it out
        {
            Debug.LogError("Could not find a place to spawn the extra drone");
            return;
        }
        pos += transform.position;

        WVDBaseDrone drone = Instantiate(_spawnDronedFromBuff, pos, _spawnDronedFromBuff.transform.rotation).GetComponent<WVDBaseDrone>();
        drone.SetSpawnerParameters(_droneSpawner);
        _droneSpawner.CurrentDronesSpawned++;
        _droneSpawner.LevelDronesRemaining++; // todo adding this back to the total may cause issues but will see
    }

    private Vector3 RandomTeleportPosition()
    {
        float randX = Random.Range(_spawnDroneRangeMin, _spawnDroneRangeMax);
        float randZ = Random.Range(_spawnDroneRangeMin, _spawnDroneRangeMax);
        if (Random.Range(0.0f, 1.0f) < 0.5f)
        {
            randX = -randX;
        }
        if (Random.Range(0.0f, 1.0f) < 0.5f)
        {
            randZ = -randZ;
        }

        return new Vector3(randX, 0.0f, randZ);
    }


    public void SetSpawnerParameters(WVDDroneSpawner spawner)
    {
        _droneSpawner = spawner;
    }

    public enum DroneState
    {
        Chasing,
        ChargingUp,
        Attacking,
        Discharge // stand still just after attack
    }

    public enum DroneBuff
    {
        None,
        SpawnOnDeath,
        Shield,
        Radiation

    }
}
