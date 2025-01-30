using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class WVDTeleportDrone : WVDBaseDrone, IWVDDamageable // a lot of this is similar to the laser drone, possible to combine?
{
    [Header("General - Teleport Drone")]
    [SerializeField]
    GameObject _damageMarker;

    [Header("Movement - Teleport Drone")]
    [SerializeField]
    float _teleportRangeMin;
    [SerializeField]
    float _teleportRangeMax;
    [SerializeField]
    GameObject _teleportChargingFX;
    [SerializeField]
    GameObject _teleportActivateFXPrefab;


    [Header("Attacking - Teleport Drone")]
    [SerializeField]
    GameObject _teleportProjectilePrefab;
    [SerializeField]
    Transform _projectileFirePoint;

    public override void DestroyFullyDamaged()
    {
        base.DestroyFullyDamaged();
        PlayerScript.RemoveDroneFromPlayerList(this);
        Destroy(gameObject);
    }
    public void TakeDamage(int damage, bool playDamageSFX)
    {
        if (!IsFullyDamaged())
        {
            print($"Teleport drone took {damage} damage");
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
            // otherwise insta kill and leave as "X"
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
                CurrentDroneState = DroneState.Attacking;

                // Fire projectile here
                if (!Stunned)
                {
                    WVDLaserDroneProjectile projectile = Instantiate(_teleportProjectilePrefab, _projectileFirePoint.position, _teleportProjectilePrefab.transform.rotation).GetComponent<WVDLaserDroneProjectile>();
                    projectile.SetProjectileDirection(_projectileFirePoint.forward);
                    projectile.SetParentDrone(gameObject);
                    SoundManager.PlaySFXAtPoint(SoundManager.DroneLaserLauchSFX, transform.position);
                }
                StartCoroutine(TransitionToStateAfterDelay(AttackDuration));
                break;

            case DroneState.Attacking:
                CurrentDroneState = DroneState.Discharge;
                _teleportChargingFX.SetActive(true);
                StartCoroutine(TransitionToStateAfterDelay(AttackDischargeDuration));

                break;
            case DroneState.Discharge:
                Instantiate(_teleportActivateFXPrefab, GetModelTransform().position, _teleportActivateFXPrefab.transform.rotation);

                Vector3 pos = RandomTeleportPosition();
                NavMeshHit hit;
                while (!NavMesh.SamplePosition(pos, out hit, 1.0f, NavMesh.AllAreas))
                {
                    pos = RandomTeleportPosition();
                }
                SoundManager.PlaySFXAtPoint(SoundManager.DroneTeleportSFX, transform.position);
                pos += transform.position;
                DroneNMA.Warp(pos);
                _teleportChargingFX.SetActive(false);
                Instantiate(_teleportActivateFXPrefab, GetModelTransform().position, _teleportActivateFXPrefab.transform.rotation);


                CurrentDroneState = DroneState.Chasing;
                DroneNMA.isStopped = false;
                break;
            case DroneState.Stopped:
                break;
            default:
                Debug.LogError("ERROR: Invalid state for Teleport Drone");
                break;
        }

    }

    private Vector3 RandomTeleportPosition()
    {
        float randX = Random.Range(_teleportRangeMin, _teleportRangeMax);
        float randZ = Random.Range(_teleportRangeMin, _teleportRangeMax);
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
