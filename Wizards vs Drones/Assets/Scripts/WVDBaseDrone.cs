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
    [SerializeField] 
    WVDDroneSpawner _droneSpawner; // todo just to see if getting ref
    [SerializeField]
    protected DroneType SelectedDroneType;
    protected WVDStatsManager StatsManager;

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
    GameObject _spawnDronedFromBuff;
    [SerializeField]
    GameObject _spawnDroneBuffIndicator;
    [SerializeField]
    bool _isSpawnedFromBuff;
    [SerializeField]
    float _spawnDroneRangeMin;
    [SerializeField]
    float _spawnDroneRangeMax;

    [SerializeField]
    GameObject _shieldObject;
    [SerializeField]
    float _shieldRechargeDelay; // As soon as it goes off, this begins counting
    bool _shieldOn;

    [SerializeField]
    GameObject _slowObject;
    [SerializeField]
    float _slowBuffPercent;
    [SerializeField]
    float _slowBuffDuration;
    [SerializeField]
    float _slowBuffCheckInterval;
    float _slowBuffCheckTimer;
    [SerializeField]
    float _slowBuffThreshold;


    public DroneState CurrentDroneState 
    { 
        get => _currentDroneState;
        set
        {
            _currentDroneState = value;
            print($"{gameObject.name} state now set to: {_currentDroneState}");
        }
    }

    public bool ShieldOn 
    { 
        get => _shieldOn;
        set 
        {
            _shieldOn = value;
            if (!_shieldOn)
            {
                _shieldObject.SetActive(false);
                Invoke("SwitchShieldBackOn", _shieldRechargeDelay);
            }
        }
    }

    void SwitchShieldBackOn()
    {
        _shieldOn = true;
        _shieldObject.SetActive(true);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        CurrentDroneState = DroneState.Chasing;
        DroneNMA = GetComponent<NavMeshAgent>();
        DroneNMA.speed = MaxNormalSpeed;

        _levelManagerScript = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<WVDLevelManager>(); 
        SoundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        StatsManager = GameObject.FindGameObjectWithTag("StatsManager").GetComponent<WVDStatsManager>();
        SoundManager.PlaySFXAtPoint(SoundManager.DroneSpawnSFX, transform.position);


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
        float slowChance = currentRoundStats.SlowChance;
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < spawnChance)
        {
            _selectedDroneBuff = DroneBuff.SpawnOnDeath;
            _spawnDroneBuffIndicator.SetActive(true);
        }
        else if (rand < spawnChance + shieldChance)
        {
            _selectedDroneBuff = DroneBuff.Shield;
            _shieldOn = true;
            _shieldObject.SetActive(true);
        }
        else if (rand < spawnChance + shieldChance + slowChance)
        {
            _selectedDroneBuff = DroneBuff.Slow;
            _slowObject.SetActive(true);
            _slowBuffCheckTimer = _slowBuffCheckInterval;
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

        if (Stunned || CurrentDroneState == DroneState.Stopped)
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
        if (_selectedDroneBuff == DroneBuff.Slow)
        {
            if (_slowBuffCheckTimer < 0.0f)
            {
                _slowBuffCheckTimer = _slowBuffCheckInterval;
                if (Vector3.Distance(transform.position, Player.transform.position) <= _slowBuffThreshold)
                {
                    PlayerScript.ApplySlow(_slowBuffPercent, _slowBuffDuration);
                }
            }
            else
            {
                _slowBuffCheckTimer -= Time.deltaTime;
            }
        }
    }

    public virtual void DestroyFullyDamaged()
    {
        // todo add in fx
        print($"{gameObject.name} drone destroyed");
        Instantiate(DestroyPrefab, transform.position + ExplodeOffset, DestroyPrefab.transform.rotation);
        SoundManager.PlaySFXAtPoint(SoundManager.DroneBlowUpSFX, transform.position);
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

        switch (SelectedDroneType)
        {
            case DroneType.Electric:
                StatsManager.ElectricDronesDestroyed++;
                break;
            case DroneType.Laser:
                StatsManager.LaserDronesDestroyed++;
                break;
            case DroneType.Fast:
                StatsManager.FastDronesDestroyed++;
                break;
            case DroneType.Teleport:
                StatsManager.TeleportDronesDestroyed++;
                break;
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
        Discharge, // stand still just after attack
        Stopped // this is when the player is dead, the drones will stop
    }

    public enum DroneBuff
    {
        None,
        SpawnOnDeath,
        Shield,
        Slow

    }

    public enum DroneType
    {
        Electric,
        Laser,
        Fast,
        Teleport
    }
}
