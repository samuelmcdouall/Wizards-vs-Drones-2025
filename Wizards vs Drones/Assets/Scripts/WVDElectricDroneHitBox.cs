using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDElectricDroneHitBox : MonoBehaviour
{
    [SerializeField]
    WVDElectricDrone _droneScript;
    public bool CanDamage;
    private void OnTriggerEnter(Collider other)
    {
        if (CanDamage && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<WVDPlayer>().TakeDamage(_droneScript.ZapDamage, true);
            CanDamage = false;
        }
    }
}
