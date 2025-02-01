using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDBossHealElement : MonoBehaviour
{
    Vector3 _direction;
    WVDBoss _bossScript;
    [SerializeField]
    GameObject _explodePrefab;


    [SerializeField]
    float _timePeriodHealElementsFlyOut;
    [SerializeField]
    float _targetDistance;
    Vector3 _targetPoint;
    [SerializeField]
    float _flyOutLerpParamater;
    bool _flyingOut;
    void Start()
    {
        _targetPoint = transform.position + _direction * _targetDistance;
        _flyingOut = true;
        Invoke("StopFlyingOut", _timePeriodHealElementsFlyOut);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_flyingOut)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPoint, _flyOutLerpParamater);
        }
    }

    void StopFlyingOut()
    {
        _flyingOut = false;
    }

    public void SetParameters(Vector3 direction, WVDBoss bossScript)
    {
        _direction = direction.normalized;
        _bossScript = bossScript;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            _bossScript.CurrentHealElementsActive--;
            Instantiate(_explodePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
