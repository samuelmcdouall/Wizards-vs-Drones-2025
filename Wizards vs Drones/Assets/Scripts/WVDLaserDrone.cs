using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WVDLaserDrone : WVDBaseDrone, IWVDDamageable
{
    [Header("General - Laser Drone")]
    //readonly Vector3 _explodeOffset = new Vector3(0.0f, 1.0f, 0.0f);
    //LaserDroneState _currentLaserDroneState;

    [Header("Movement - Laser Drone")]
    //[SerializeField]
    //float _attackRayCastDistance;
    //[SerializeField]
    //Transform[] _rayCastPoints;
    //NavMeshAgent _laserDroneNMA;
    //readonly float _chargingTurnFactor = 2.5f;
    //[SerializeField]
    //GameObject _droneModel;

    [Header("Attacking - Laser Drone")]
    //[SerializeField]
    //float _attackChargeUpDuration;
    //[SerializeField]
    //float _attackDischargeDuration;
    [SerializeField]
    GameObject _laserProjectilePrefab;
    [SerializeField]
    Transform _projectileFirePoint;
    //readonly int _layerMask = 1 << 2;

    //public LaserDroneState CurrentLaserDroneState
    //{
    //    get => _currentLaserDroneState;
    //    set
    //    {
    //        _currentLaserDroneState = value;
    //        print($"Laser Drone State now set to: {_currentLaserDroneState}");
    //    }
    //}

    public void DestroyFullyDamaged()
    {
        // todo add in fx
        print("Laser drone destroyed");
        Instantiate(ExplodePrefab, transform.position + ExplodeOffset, ExplodePrefab.transform.rotation);
        Instantiate(BatteryPickUp, transform.position + ExplodeOffset, BatteryPickUp.transform.rotation);
        Player.GetComponent<WVDPlayer>().RemoveDroneFromPlayerList(this);
        Destroy(gameObject);
    }
    public void TakeDamage(int damage)
    {
        print($"Laser drone took {damage} damage");
        CurrentHealth -= damage;
        if (IsFullyDamaged())
        {
            if (!DestroySequenceCompleted)
            {
                DestroyFullyDamaged();
                DestroySequenceCompleted = true;
            }
        }
    }

    //public bool IsFullyDamaged()
    //{
    //    if (CurrentHealth <= 0.0f)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //_currentLaserDroneState = LaserDroneState.Chasing;
        //_laserDroneNMA = GetComponent<NavMeshAgent>();
        //_laserDroneNMA.speed = MaxNormalSpeed;
        Player.GetComponent<WVDPlayer>().AddDroneToPlayerList(this);   
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //_laserDroneNMA.speed = MaxNormalSpeed;
        //for (int i = 0; i < _rayCastPoints.Length; i++)
        //{
        //    Debug.DrawRay(_rayCastPoints[i].position, _rayCastPoints[i].forward * _attackRayCastDistance, Color.magenta);
        //}

        //if (Stunned)
        //{
        //    _laserDroneNMA.isStopped = true;
        //    return;
        //}
        //else
        //{
        //    _laserDroneNMA.isStopped = false;
        //}
        if (CurrentDroneState == DroneState.Chasing)
        {
            bool hitPlayer = false;
            for (int i = 0; i < RayCastPoints.Length; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(RayCastPoints[i].position, RayCastPoints[i].forward, out hit, AttackRayCastDistance, ~LayerMask))
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
                CurrentDroneState = DroneState.ChargingUp;
                DroneNMA.isStopped = true;
                StartCoroutine(TransitionToStateAfterDelay(AttackChargeUpDuration));
            }
            else
            {
                DroneNMA.SetDestination(Player.transform.position);
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
            if (CurrentDroneState == DroneState.ChargingUp)
            {
                // this turns the drone gradually over time, looks more natural
                Vector3 yIndepPlayerPos = new Vector3(Player.transform.position.x, 0.0f, Player.transform.position.z);
                Vector3 yIndepDronePos = new Vector3(transform.position.x, 0.0f, transform.position.z);
                Vector3 direction = (yIndepPlayerPos - yIndepDronePos).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * ChargingTurnFactor);


                // this snaps immediately to face the player and doesn't look natural
                //transform.LookAt(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z));
            }
            yield return null;
        }
        switch (CurrentDroneState)
        {
            case DroneState.ChargingUp:
                CurrentDroneState = DroneState.Discharge;

                // Fire projectile here
                if (!Stunned)
                {
                    GameObject laserDrone = Instantiate(_laserProjectilePrefab, _projectileFirePoint.position, _laserProjectilePrefab.transform.rotation);
                    laserDrone.GetComponent<WVDLaserDroneProjectile>().SetProjectileDirection(_projectileFirePoint.forward);
                }

                StartCoroutine(TransitionToStateAfterDelay(AttackDischargeDuration));
                break;
            case DroneState.Discharge:
                CurrentDroneState = DroneState.Chasing;
                DroneNMA.isStopped = false;
                break;
            default:
                Debug.LogError("ERROR: Invalid state for Laser Drone");
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
        return DroneModel.transform;
    }

    //public enum LaserDroneState
    //{
    //    Chasing,
    //    ChargingUp, // Just before going into Discharge state, do the attack
    //    Discharge // stand still just after attack
    //}
}
