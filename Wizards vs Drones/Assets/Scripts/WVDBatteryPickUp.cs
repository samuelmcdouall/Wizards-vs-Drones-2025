using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDBatteryPickUp : MonoBehaviour
{
    [SerializeField]
    int _value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUpTrigger"))
        {
            other.gameObject.transform.parent.gameObject.GetComponent<WVDPlayer>().BatteryCount += _value;
            Destroy(gameObject);
        }
    }
}
