using TMPro;
using UnityEngine;
using UnityEngine.AI;

public abstract class WVDBaseDrone : WVDBaseEntity
{
    [Header("General - Base Drone")]
    DroneState _currentDroneState;
    WVDLevelManager _levelManagerScript;
    WVDDroneSpawner _droneSpawner;
    [SerializeField]
    protected DroneType SelectedDroneType;
    protected WVDStatsManager StatsManager;
    WVDTutorialManager _tutorialManager;
    WVDDifficultySettingsManager _challengeModeManager;

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
    DroneBuff _selectedDroneBuff;
    [Header("Spawn On Death Buff")]
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

    [Header("Shield Buff")]
    [SerializeField]
    GameObject _shieldObject;
    [SerializeField]
    float _shieldRechargeDelay; // As soon as it goes off, this begins counting
    bool _shieldOn;

    [Header("Slow Buff")]
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
    bool _slowTutorialBeenChecked; // To make sure we don't have multiple tutorial tips of this type be queued before any can be played 

    [Header("Destroyed - Base Drone")]
    [SerializeField]
    protected GameObject DestroyPrefab;
    [SerializeField]
    protected GameObject BatteryPickUp;
    protected bool DestroySequenceCompleted;
    protected Vector3 ExplodeOffset = new Vector3(0.0f, 1.0f, 0.0f);
    [SerializeField]
    protected float PickUpChance;
    protected float BonusPickUpChanceFromLastHit;
    protected float ExplodeOnDeathChanceFromLastHit;
    [SerializeField]
    protected GameObject ExplodePrefab;

    [Header("UI - Base Drone")]
    [SerializeField]
    GameObject _droneRemainingHelpUIPrefab;
    GameObject _droneRemainingHelpUIInstance;
    float _remainingDroneStuckTime = 30.0f;
    float _remainingDroneStuckTimer;
    [SerializeField]
    GameObject _damageMarker;

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
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.ShieldBuff, 1.0f));
                Invoke("SwitchShieldBackOn", _shieldRechargeDelay);
            }
        }
    }

    public override void Start()
    {
        base.Start();
        CurrentDroneState = DroneState.Chasing;
        DroneNMA = GetComponent<NavMeshAgent>();
        DroneNMA.speed = MaxNormalSpeed;

        _levelManagerScript = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<WVDLevelManager>(); 
        SoundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        StatsManager = GameObject.FindGameObjectWithTag("StatsManager").GetComponent<WVDStatsManager>();
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<WVDTutorialManager>();
        _challengeModeManager = GameObject.FindGameObjectWithTag("DifficultyModeManager").GetComponent<WVDDifficultySettingsManager>();
        SoundManager.PlaySFXAtPoint(SoundManager.DroneSpawnSFX, transform.position);

        if (_challengeModeManager.ChallengeModeActive)
        {
            MaxHealth *= 2;
            CurrentHealth *= 2;
        }

        if (!_isSpawnedFromBuff)
        {
            DetermineDroneBuff();
        }
        else
        {
            _selectedDroneBuff = DroneBuff.None;
        }

        switch (SelectedDroneType)
        {
            case DroneType.Electric:
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.ElectricDrone, 1.0f));
                break;
            case DroneType.Laser:
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.LaserDrone, 1.0f));
                break;
            case DroneType.Fast:
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.FastDrone, 1.0f));
                break;
            case DroneType.Teleport:
                WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.TeleportDrone, 1.0f));
                break;
        }

        // If a new drone is spawned and we are below threshold then this one must show its helper UI
        if (_droneSpawner.LevelDronesRemaining <= _droneSpawner.DronesRemainingHelpUIThreshold)
        {
            SpawnDroneRemainingHelpUI();
        }
        _remainingDroneStuckTimer = _remainingDroneStuckTime;
    }
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
            if (DroneNMA.isOnNavMesh)
            {
                DroneNMA.isStopped = true;
            }
            return;
        }
        else
        {
            // If it is in chasing state and is no longer stunned, reenable the movement, if it isn't then it'll be stopped because its charging up/attacking etc. anyway
            if (CurrentDroneState == DroneState.Chasing) 
            {
                if (DroneNMA.isOnNavMesh)
                {
                    DroneNMA.isStopped = false;
                }
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
                    if (!_slowTutorialBeenChecked)
                    {
                        WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.SlowBuff, 1.0f));
                        _slowTutorialBeenChecked = true;
                    }
                }
            }
            else
            {
                _slowBuffCheckTimer -= Time.deltaTime;
            }
        }

        // As a failsafe if a drone is stuck somewhere and its one of the last ones (i.e. it has spawned its UI helper), it will be teleported to a
        // random spawn point after a timer to stop the game locking
        if (_droneRemainingHelpUIInstance)
        {
            _remainingDroneStuckTimer -= Time.deltaTime;
            if (_remainingDroneStuckTimer < 0.0f)
            {
                _remainingDroneStuckTimer = _remainingDroneStuckTime;
                Vector3 randomAvailablePosition = _droneSpawner.AvailableSpawnPositions[Random.Range(0, _droneSpawner.AvailableSpawnPositions.Count)].position;
                DroneNMA.Warp(randomAvailablePosition);
                print($"Drone is stuck, teleporting to: {randomAvailablePosition}");

            }
        }
    }
    void SwitchShieldBackOn()
    {
        _shieldOn = true;
        _shieldObject.SetActive(true);
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
    public void SpawnDroneRemainingHelpUI()
    {
        if (!_droneRemainingHelpUIInstance)
        {
            print("One of the last few drones, spawning UI helper for: " + gameObject.name);
            _droneRemainingHelpUIInstance = Instantiate(_droneRemainingHelpUIPrefab, transform.position, Quaternion.identity);
            _droneRemainingHelpUIInstance.transform.SetParent(GameObject.FindGameObjectWithTag("HelpDroneUIParent").transform, false);
            _droneRemainingHelpUIInstance.GetComponent<WVDDroneRemainingHelpUI>().SetDroneTransform(transform);
        }
    }
    // If the player damages a drone then they're in combat with it, so the timer is reset
    void ResetRemainingStuckTimer()
    {
        _remainingDroneStuckTimer = _remainingDroneStuckTime;
    }
    public virtual void DestroyFullyDamaged()
    {
        CurrentDroneState = DroneState.Dead;
        print($"{gameObject.name} drone destroyed");
        Instantiate(DestroyPrefab, transform.position + ExplodeOffset, DestroyPrefab.transform.rotation);
        
        DropBattery();
        if (_challengeModeManager.ChallengeModeActive)
        {
            DropBattery(); // possibly drop another if in challenge mode
        }
        
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < ExplodeOnDeathChanceFromLastHit)
        {
            Instantiate(ExplodePrefab, transform.position + ExplodeOffset, ExplodePrefab.transform.rotation);
        }


        if (_selectedDroneBuff == DroneBuff.SpawnOnDeath)
        {
            SpawnDroneFromBuff();
            SpawnDroneFromBuff();
            WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.SpawnOnDeathBuff, 1.0f));
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

        if (_droneRemainingHelpUIInstance)
        {
            Destroy(_droneRemainingHelpUIInstance);
        }
    }
    void DropBattery()
    {
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < PickUpChance + BonusPickUpChanceFromLastHit)
        {
            Instantiate(BatteryPickUp, transform.position + ExplodeOffset, BatteryPickUp.transform.rotation);
        }
    }
    void SpawnDroneFromBuff()
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
        if (i == 1000) // this is to stop infintie loop if can't find place to put drone
        {
            Debug.LogError("Could not find a place to spawn the extra drone");
            return;
        }
        pos += transform.position;

        WVDBaseDrone drone = Instantiate(_spawnDronedFromBuff, pos, _spawnDronedFromBuff.transform.rotation).GetComponent<WVDBaseDrone>();
        drone.SetSpawnerParameters(_droneSpawner);
        _droneSpawner.CurrentDronesSpawned++;
        _droneSpawner.LevelDronesRemaining++;
    }
    Vector3 RandomTeleportPosition()
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
    public virtual void ResolveAttack(int damage, WVDAttackEffects effects)
    {
        BonusPickUpChanceFromLastHit = effects.DropRateIncrease;
        ExplodeOnDeathChanceFromLastHit = effects.ExplodeOnDeathChance;
    }
    public virtual void TakeDamage(int damage, bool playDamageSFX)
    {
        if (!IsFullyDamaged())
        {
            print($"{gameObject.name} took {damage} damage");
            CurrentHealth -= damage;
            Vector3 randomSpawnOffset = new Vector3(Random.Range(-0.4f, 0.4f), 0.0f, Random.Range(-0.4f, 0.4f));
            TMP_Text text = Instantiate(_damageMarker, transform.position + Vector3.up * 2.0f + randomSpawnOffset, Quaternion.identity).GetComponent<TMP_Text>();
            if (damage <= 0)
            {
                text.text = ""; // i.e. no damage from attack
            }
            else if (damage <= 10)
            {
                text.text = "" + damage;
            }
            // Otherwise insta kill and leave as "X"
            ResetRemainingStuckTimer();
            if (playDamageSFX)
            {
                SoundManager.PlayRandomSFXAtPlayer(new AudioClip[] { SoundManager.DroneTakeDamageSFX1, SoundManager.DroneTakeDamageSFX2 });
            }
        }
    }
    public enum DroneState
    {
        Chasing,
        ChargingUp,
        Attacking,
        Discharge, // stand still just after attack
        Stopped, // this is when the player is dead, the drones will stop
        Dead
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