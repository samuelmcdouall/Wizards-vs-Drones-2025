using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDDroneSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject _testDrone;
    void Start()
    {
        InvokeRepeating("TestSpawn", 5, 5);
    }

    void TestSpawn()
    {
        Instantiate(_testDrone, transform.position, _testDrone.transform.rotation);
    }
}
