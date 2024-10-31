using System.Collections;
using UnityEngine;

public class WVDElectricDrone : WVDBaseDrone, IWVDDamageable
{
    [Header("General - Electric Drone")]

    [Header("Movement - Electric Drone")]

    [Header("Attacking - Electric Drone")]
    [SerializeField]
    GameObject _attackHitBox;
    [SerializeField]
    WVDElectricDroneHitBox _attackHitBoxScript;
    [SerializeField]
    int _zapDamage;

    public int ZapDamage 
    { 
        get => _zapDamage; 
        set => _zapDamage = value; 
    }

    public void DestroyFullyDamaged()
    {
        // todo add in fx
        print("Electric drone destroyed");
        Instantiate(ExplodePrefab, transform.position + ExplodeOffset, ExplodePrefab.transform.rotation);
        Instantiate(BatteryPickUp, transform.position + ExplodeOffset, BatteryPickUp.transform.rotation);
        Player.GetComponent<WVDPlayer>().RemoveDroneFromPlayerList(this);
        Destroy(gameObject);
    }
    public void TakeDamage(int damage)
    {
        print($"Electric drone took {damage} damage");
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
    public override void Start()
    {
        base.Start();
        _attackHitBox.SetActive(false);
        Player.GetComponent<WVDPlayer>().AddDroneToPlayerList(this);
    }

    public override void Update()
    {
        base.Update();
        if (CurrentDroneState == DroneState.Chasing)
        {
            bool hitPlayer = false;
            for (int i = 0; i < RayCastPoints.Length; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(RayCastPoints[i].position, RayCastPoints[i].forward, out hit, AttackRayCastDistance, ~LayerMask))
                {
                    print(hit.transform.gameObject.tag);
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
                CurrentDroneState = DroneState.Attacking;
                if (!Stunned)
                {
                    _attackHitBox.SetActive(true);
                    _attackHitBoxScript.CanDamage = true;
                }
                StartCoroutine(TransitionToStateAfterDelay(AttackDuration));
                break;
            case DroneState.Attacking:
                CurrentDroneState = DroneState.Discharge;
                _attackHitBox.SetActive(false);
                StartCoroutine(TransitionToStateAfterDelay(AttackDischargeDuration));
                break;
            case DroneState.Discharge:
                CurrentDroneState = DroneState.Chasing;
                DroneNMA.isStopped = false;
                break;
            default:
                Debug.LogError("ERROR: Invalid state for Electric Drone");
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
}