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

    public override void Start()
    {
        base.Start();
        _attackHitBox.SetActive(false);
        PlayerScript.AddDroneToPlayerList(this);
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
                StartCoroutine(TransitionToStateAfterDelay(AttackChargeUpDuration)); // TransitionToStateAfterDelay is different for all the drones so have to do this chunk in each derived class
            }
            else
            {
                DroneNMA.SetDestination(Player.transform.position);
            }
        }
    }
    IEnumerator TransitionToStateAfterDelay(float delay)
    {
        // If charging up, turn to face player for delay
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
                    SoundManager.PlaySFXAtPoint(SoundManager.DroneZapSFX, transform.position);
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
            case DroneState.Stopped:
                _attackHitBox.SetActive(false);
                break;
            case DroneState.Dead:
                // Do nothing absorbing state
                break;
            default:
                Debug.LogError("ERROR: Invalid state for Electric Drone");
                break;
        }
    }
    public override void ResolveAttack(int damage, WVDAttackEffects effects)
    {
        base.ResolveAttack(damage, effects);
        TakeDamage(damage, true);
        ApplyEffects(effects);
    }
    public override void TakeDamage(int damage, bool playDamageSFX)
    {
        base.TakeDamage(damage, playDamageSFX);
        if (IsFullyDamaged() && CurrentDroneState != DroneState.Dead)
        {
            if (!DestroySequenceCompleted)
            {
                DestroySequenceCompleted = true;
                DestroyFullyDamaged();
            }
        }
    }
    public override void DestroyFullyDamaged()
    {
        base.DestroyFullyDamaged();
        PlayerScript.RemoveDroneFromPlayerList(this);
        Destroy(gameObject);
    }
    public override void ApplyEffects(WVDAttackEffects effects)
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
                TakeDamage(damage, true);
                intervalTime = Time.time + interval;
            }
            await Task.Yield();
        }
        TakeDamage(damage, true); // Final damage to make the last damaging tick of damage
    }
    public Transform GetTransform()
    {
        return gameObject.transform;
    }
    public Transform GetModelTransform()
    {
        return DroneModel.transform;
    }
}