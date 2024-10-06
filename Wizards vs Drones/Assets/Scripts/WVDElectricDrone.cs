using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WVDElectricDrone : WVDBaseEntity, IWVDDamageable
{
    [Header("General - Electric Drone")]
    [SerializeField]
    GameObject _explodePrefab;
    ElectricDroneState _currentElectricDroneState;

    [Header("Movement - Electric Drone")]
    [SerializeField]
    float _attackRayCastDistance;
    [SerializeField]
    Transform[] _rayCastPoints;
    NavMeshAgent _electricDroneNMA;
    readonly float _chargingTurnFactor = 2.5f;

    [Header("Attacking - Electric Drone")]
    [SerializeField]
    float _attackChargeUpDuration;
    [SerializeField]
    float _attackDuration;
    [SerializeField]
    float _attackDischargeDuration;
    [SerializeField]
    GameObject _attackHitBox;

    public ElectricDroneState CurrentElectricDroneState 
    { 
        get => _currentElectricDroneState;
        set
        {
            _currentElectricDroneState = value;
            print($"Electric Drone State now set to: {_currentElectricDroneState}");
        }
    }

    public void DestroyFullyDamaged()
    {
        // todo add in fx
        print("Electric drone destroyed");
        Destroy(gameObject);
    }
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (IsFullyDamaged())
        {
            DestroyFullyDamaged();
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
    }

    // Update is called once per frame
    void Update()
    {
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
                if (Physics.Raycast(_rayCastPoints[i].position, _rayCastPoints[i].forward, out hit, _attackRayCastDistance))
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


    public enum ElectricDroneState
    {
        Chasing,
        ChargingUp,
        Attacking,
        Discharge // stand still just after attack
    }
}
