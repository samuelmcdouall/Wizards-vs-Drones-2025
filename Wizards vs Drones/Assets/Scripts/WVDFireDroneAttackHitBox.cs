using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDFireDroneAttackHitBox : MonoBehaviour
{
    WVDPlayer _playerScript;
    [SerializeField]
    int _flameDamagePerSecond;


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        _playerScript = other.GetComponent<WVDPlayer>().StartBurning(_flameDamagePerSecond);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        other.GetComponent<WVDPlayer>().StopBurning();
    //    }
    //}

    //private void OnEnable()
    //{
        
    //}
    //private void OnDisable()
    //{
        
    //}
    //void Update()
    //{
        
    //}
}
