using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDDroneShieldBuff : MonoBehaviour
{
    [SerializeField]
    WVDBaseDrone _baseDroneScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            _baseDroneScript.ShieldOn = false;
        }
    }
}
