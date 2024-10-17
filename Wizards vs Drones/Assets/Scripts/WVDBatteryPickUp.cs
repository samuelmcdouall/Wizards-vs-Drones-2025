using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDBatteryPickUp : MonoBehaviour
{
    [SerializeField]
    int _value;
    Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        float randX = Random.Range(-200.0f, 200.0f);
        float randY = Random.Range(200.0f, 300.0f);
        float randZ = Random.Range(-200.0f, 200.0f);
        _rb.AddForce(new Vector3(randX, randY, randZ));
        randX = Random.Range(-100.0f, 100.0f);
        randY = Random.Range(-100.0f, 100.0f);
        randZ = Random.Range(-100.0f, 100.0f);
        _rb.AddTorque(new Vector3(randX, randY, randZ));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUpTrigger"))
        {
            other.gameObject.transform.parent.gameObject.GetComponent<WVDPlayer>().BatteryCount += _value;
            Destroy(gameObject);
        }
    }
}
