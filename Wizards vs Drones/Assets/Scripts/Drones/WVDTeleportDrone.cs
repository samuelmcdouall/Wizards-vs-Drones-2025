using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class WVDTeleportDrone : WVDBaseDrone, IWVDDamageable
{
    [Header("General - Teleport Drone")]

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

    public override void Start()
    {
        base.Start();
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
                if (DroneNMA.isOnNavMesh)
                {
                    DroneNMA.SetDestination(Player.transform.position);
                }
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
    Vector3 RandomTeleportPosition()
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