using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDTrailFX : MonoBehaviour
{
    [SerializeField]
    float _lifetime;
    [SerializeField]
    ParticleSystem _ps;
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        _ps.time = 0.0f;
        Invoke("DisableObject", _lifetime);
    }
    void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
