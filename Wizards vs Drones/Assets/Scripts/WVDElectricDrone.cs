using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WVDElectricDrone : WVDBaseEntity, IWVDDamageable
{
    [Header("General - Electric Drone")]
    [SerializeField]
    GameObject _explodePrefab;
    readonly Vector3 _explodeOffset = new Vector3(0.0f, 1.0f, 0.0f); 
    ElectricDroneState _currentElectricDroneState;
    [SerializeField]
    GameObject _batteryPickUp;
    bool _destroySequenceCompleted;

    [Header("Movement - Electric Drone")]
    [SerializeField]
    float _attackRayCastDistance;
    [SerializeField]
    Transform[] _rayCastPoints;
    NavMeshAgent _electricDroneNMA;
    readonly float _chargingTurnFactor = 2.5f;
    [SerializeField]
    GameObject _droneModel;

    [Header("Attacking - Electric Drone")]
    [SerializeField]
    float _attackChargeUpDuration;
    [SerializeField]
    float _attackDuration;
    [SerializeField]
    float _attackDischargeDuration;
    [SerializeField]
    GameObject _attackHitBox;
    [SerializeField]
    WVDElectricDroneHitBox _attackHitBoxScript;
    [SerializeField]
    int _zapDamage;
    readonly int _layerMask = 1 << 2;

    public ElectricDroneState CurrentElectricDroneState 
    { 
        get => _currentElectricDroneState;
        set
        {
            _currentElectricDroneState = value;
            print($"Electric Drone State now set to: {_currentElectricDroneState}");
        }
    }

    public int ZapDamage 
    { 
        get => _zapDamage; 
        set => _zapDamage = value; 
    }

    public void DestroyFullyDamaged()
    {
        // todo add in fx
        print("Electric drone destroyed");
        Instantiate(_explodePrefab, transform.position + _explodeOffset, _explodePrefab.transform.rotation);
        Instantiate(_batteryPickUp, transform.position + _explodeOffset, _batteryPickUp.transform.rotation);
        Player.GetComponent<WVDPlayer>().RemoveDroneFromPlayerList(this);
        Destroy(gameObject);
    }
    public void TakeDamage(int damage)
    {
        print($"Electric drone took {damage} damage");
        CurrentHealth -= damage;
        if (IsFullyDamaged())
        {
            if (!_destroySequenceCompleted)
            {
                DestroyFullyDamaged();
                _destroySequenceCompleted = true;
            }
        }
    }

    public bool IsFullyDamaged()
    {
        if (CurrentHealth <= 0.0f)
        {
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _currentElectricDroneState = ElectricDroneState.Chasing;
        _electricDroneNMA = GetComponent<NavMeshAgent>();
        _electricDroneNMA.speed = MaxNormalSpeed;
        _attackHitBox.SetActive(false);
        Player.GetComponent<WVDPlayer>().AddDroneToPlayerList(this);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        _electricDroneNMA.speed = MaxNormalSpeed;
        for (int i = 0; i < _rayCastPoints.Length; i++)
        {
            Debug.DrawRay(_rayCastPoints[i].position, _rayCastPoints[i].forward * _attackRayCastDistance, Color.magenta);
        }

        if (CurrentElectricDroneState == ElectricDroneState.Chasing)
        {
            bool hitPlayer = false;
            for (int i = 0; i < _rayCastPoints.Length; i++) 
            {
                RaycastHit hit;
                if (Physics.Raycast(_rayCastPoints[i].position, _rayCastPoints[i].forward, out hit, _attackRayCastDistance, ~_layerMask))
                {
                    if (hit.transform.gameObject.CompareTag("Player"))
                    {
                        hitPlayer = true;
                        break;
                    }
                }
            }
            if (hitPlayer)
            {
                CurrentElectricDroneState = ElectricDroneState.ChargingUp;
                _electricDroneNMA.isStopped = true;
                StartCoroutine(TransitionToStateAfterDelay(_attackChargeUpDuration));
            }
            else
            {
                _electricDroneNMA.SetDestination(Player.transform.position);
            }
        }
    }

    IEnumerator TransitionToStateAfterDelay(float delay) // Coroutine over async here because coroutine handles destroyed object easier
    {
        //yield return new WaitForSeconds(delay);

        // Delay (if charging up, turn to face player)
        float endTime = Time.time + delay;
        while (Time.time < endTime)
        {
            if (CurrentElectricDroneState == ElectricDroneState.ChargingUp)
            {
                // this turns the drone gradually over time, looks more natural
                Vector3 yIndepPlayerPos = new Vector3(Player.transform.position.x, 0.0f, Player.transform.position.z);
                Vector3 yIndepDronePos = new Vector3(transform.position.x, 0.0f, transform.position.z);
                Vector3 direction = (yIndepPlayerPos - yIndepDronePos).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _chargingTurnFactor);


                // this snaps immediately to face the player and doesn't look natural
                //transform.LookAt(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z));
            }
            yield return null;
        }
        switch (CurrentElectricDroneState)
        {
            case ElectricDroneState.ChargingUp:
                CurrentElectricDroneState = ElectricDroneState.Attacking;
                _attackHitBox.SetActive(true);
                _attackHitBoxScript.CanDamage = true;
                StartCoroutine(TransitionToStateAfterDelay(_attackDuration));
                break;
            case ElectricDroneState.Attacking:
                CurrentElectricDroneState = ElectricDroneState.Discharge;
                _attackHitBox.SetActive(false);
                StartCoroutine(TransitionToStateAfterDelay(_attackDischargeDuration));
                break;
            case ElectricDroneState.Discharge:
                CurrentElectricDroneState = ElectricDroneState.Chasing;
                _electricDroneNMA.isStopped = false;
                break;
            default:
                Debug.LogError("ERROR: Should not be broken");
                break;
        }

    }

    public Transform GetTransform()
    {
        return gameObject.transform;
    }

    public void ResolveAttack(int damage, WVDAttackEffects effects)
    {
        TakeDamage(damage);
        ApplyEffects(effects);
    }

    public Transform GetModelTransform()
    {
        return _droneModel.transform;
    }

    public enum ElectricDroneState
    {
        Chasing,
        ChargingUp,
        Attacking,
        Discharge // stand still just after attack
    }
}
