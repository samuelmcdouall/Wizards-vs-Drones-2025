using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDHomingProjectile : WVDBaseProjectile
{
    IWVDDamageable _currentLockedOnDrone;
    List<IWVDDamageable> _droneTargets = new List<IWVDDamageable>(); // On creation give the homing projectile a reference to the drone list from the player script

    public List<IWVDDamageable> DroneTargets 
    { 
        get => _droneTargets; 
        set => _droneTargets = value; 
    }

    public override void Start()
    {
        base.Start();
        _currentLockedOnDrone = SelectClosestTarget();
    }

    private void FixedUpdate()
    {
        // If the currently locked on drone has yet to be hit, then move towards it
        if (_currentLockedOnDrone as Object)
        {
            Vector3 directionToDrone = _currentLockedOnDrone.GetModelTransform().position - transform.position;
            SetProjectileDirection(directionToDrone);
        }
        // Otherwise find the next closest target and move towards that (if there are no others, it will keep searching an empty list but continue on same trajectory)
        else
        {
            _currentLockedOnDrone = SelectClosestTarget();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<IWVDDamageable>() != null)
        {
            other.gameObject.GetComponent<IWVDDamageable>().ResolveAttack(Damage, Effects);
            print("hit enemy");
        }
    }

    IWVDDamageable SelectClosestTarget()
    {
        IWVDDamageable closestDrone = null;
        float closestDistance = 1000000.0f;
        foreach (IWVDDamageable drone in _droneTargets)
        {
            float distanceToDrone = Vector3.Distance(drone.GetTransform().position, transform.position);
            if (distanceToDrone < closestDistance)
            {
                closestDrone = drone;
                closestDistance = distanceToDrone;
            }
        }
        return closestDrone;
    }
}
