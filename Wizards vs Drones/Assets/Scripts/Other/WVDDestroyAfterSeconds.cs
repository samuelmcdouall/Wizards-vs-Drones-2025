using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDDestroyAfterSeconds : MonoBehaviour
{
    [SerializeField]
    float _lifetime;
    void Start()
    {
        Destroy(gameObject, _lifetime);
    }
}
