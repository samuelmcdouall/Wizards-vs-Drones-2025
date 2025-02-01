using UnityEngine;

public class WVDBossHealElement : MonoBehaviour
{
    [Header("General")]
    Vector3 _direction;
    WVDBoss _bossScript;
    [SerializeField]
    GameObject _explodePrefab;

    [Header("Flying to position")]
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
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            _bossScript.CurrentHealElementsActive--;
            Instantiate(_explodePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}