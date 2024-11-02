using System.Collections;
using System.Threading.Tasks;
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
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < PickUpChance + BonusPickUpChance)
        {
            Instantiate(BatteryPickUp, transform.position + ExplodeOffset, BatteryPickUp.transform.rotation);
        }
        Player.GetComponent<WVDPlayer>().RemoveDroneFromPlayerList(this); // todo apart from this line, could probably put the base function of this into the base drone function. Still have each drone implementing the Damageable interface, and an override function here
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
        BonusPickUpChance = effects.DropRateIncrease; // todo this is drone specific, maybe later on combine this into the base drone class, or possibly put into the apply effects
        TakeDamage(damage);
        ApplyEffects(effects);
    }

    public override void ApplyEffects(WVDAttackEffects effects) // todo look at again but because of TakeDamage, this might have to remain in each drone class
    {
        base.ApplyEffects(effects);
        if (effects.DOT)
        {
            ApplyDOT(effects.DOTDamage, effects.DOTInterval, effects.DOTDuration);
        }
    }

    public async void ApplyDOT(int damage, float interval, float duration)
    {
        float endTime = Time.time + duration;
        float intervalTime = Time.time + interval;
        while (Time.time < endTime)
        {
            if (Time.time > intervalTime)
            {
                TakeDamage(damage);
                intervalTime = Time.time + interval;
            }
            await Task.Yield();
        }
        TakeDamage(damage); // Final damage to make the last damaging tick of damage
    }

    public Transform GetModelTransform()
    {
        return DroneModel.transform;
    }
}