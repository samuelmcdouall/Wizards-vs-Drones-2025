using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WVDLaserDrone : WVDBaseDrone, IWVDDamageable
{
    [Header("General - Laser Drone")]
    [SerializeField]
    GameObject _damageMarker;

    [Header("Movement - Laser Drone")]

    [Header("Attacking - Laser Drone")]
    [SerializeField]
    GameObject _laserProjectilePrefab;
    [SerializeField]
    Transform _projectileFirePoint;

    public override void DestroyFullyDamaged()
    {
        //// todo add in fx
        //print("Laser drone destroyed");
        //Instantiate(DestroyPrefab, transform.position + ExplodeOffset, DestroyPrefab.transform.rotation);
        //float rand = Random.Range(0.0f, 1.0f);
        //if (rand < PickUpChance + BonusPickUpChanceFromLastHit)
        //{
        //    Instantiate(BatteryPickUp, transform.position + ExplodeOffset, BatteryPickUp.transform.rotation);
        //}
        //rand = Random.Range(0.0f, 1.0f);
        //if (rand < ExplodeOnDeathChanceFromLastHit)
        //{
        //    Instantiate(ExplodePrefab, transform.position + ExplodeOffset, ExplodePrefab.transform.rotation);
        //}
        base.DestroyFullyDamaged();
        PlayerScript.RemoveDroneFromPlayerList(this);
        Destroy(gameObject);
    }
    public void TakeDamage(int damage, bool playDamageSFX)
    {
        print($"Laser drone took {damage} damage");
        CurrentHealth -= damage;
        Vector3 randomSpawnOffset = new Vector3(Random.Range(-0.4f, 0.4f), 0.0f, Random.Range(-0.4f, 0.4f));
        TMP_Text text = Instantiate(_damageMarker, transform.position + Vector3.up * 2.0f + randomSpawnOffset, Quaternion.identity).GetComponent<TMP_Text>();
        if (damage > 10)
        {
            text.text = "X"; // i.e. insta kill
        }
        else
        {
            text.text = "" + damage;
        }

        ResetRemainingStuckTimer();
        if (playDamageSFX)
        {
            SoundManager.PlayRandomSFXAtPlayer(new AudioClip[] { SoundManager.DroneTakeDamageSFX1, SoundManager.DroneTakeDamageSFX2 });
        }
        if (IsFullyDamaged())
        {
            if (!DestroySequenceCompleted)
            {
                DestroySequenceCompleted = true;
                DestroyFullyDamaged();
            }
        }
    }
    public override void Start()
    {
        base.Start();
        PlayerScript.AddDroneToPlayerList(this);   
    }

    // Update is called once per frame
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
                CurrentDroneState = DroneState.Discharge;

                // Fire projectile here
                if (!Stunned)
                {
                    WVDLaserDroneProjectile laserDrone = Instantiate(_laserProjectilePrefab, _projectileFirePoint.position, _laserProjectilePrefab.transform.rotation).GetComponent<WVDLaserDroneProjectile>();
                    laserDrone.SetProjectileDirection(_projectileFirePoint.forward);
                    laserDrone.SetParentDrone(gameObject);
                    SoundManager.PlaySFXAtPoint(SoundManager.DroneLaserLauchSFX, transform.position);
                }

                StartCoroutine(TransitionToStateAfterDelay(AttackDischargeDuration));
                break;
            case DroneState.Discharge:
                CurrentDroneState = DroneState.Chasing;
                DroneNMA.isStopped = false;
                break;
            case DroneState.Stopped:
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
        BonusPickUpChanceFromLastHit = effects.DropRateIncrease;
        ExplodeOnDeathChanceFromLastHit = effects.ExplodeOnDeathChance;
        TakeDamage(damage, true);
        ApplyEffects(effects);
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

    public Transform GetModelTransform()
    {
        return DroneModel.transform;
    }
}