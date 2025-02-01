using UnityEngine;

public class WVDBossFireStreamElement : MonoBehaviour
{
    [Header("General")]
    Vector3 _direction;
    float _timeIntervalToSpawnNextElement;
    int _elementNumber;
    [SerializeField]
    float _distance;
    [SerializeField]
    float _maxNumberElements;
    [SerializeField]
    float _lifeTime;
    [SerializeField]
    GameObject _fireStreamElementPrefab;

    [Header("Damage")]
    bool _canDamage;
    [SerializeField]
    float _canDamageInterval;
    [SerializeField]
    int _damage;

    void Start()
    {
        gameObject.name = "BossFireStreamElement (Clone)"; // So we don't get stupidly long names in the hierarchy
        _canDamage = true;
        Destroy(gameObject, _lifeTime);
        if (_elementNumber < _maxNumberElements)
        {
            Invoke("SpawnNextElement", _timeIntervalToSpawnNextElement);
        }
    }
    public void SetParameters(Vector3 direction, float timeInterval, int elementNumber)
    {
        _direction = direction.normalized;
        _timeIntervalToSpawnNextElement = timeInterval;
        _elementNumber = elementNumber;
    }
    void SpawnNextElement()
    {
        WVDBossFireStreamElement fireStreamElement = Instantiate(_fireStreamElementPrefab, transform.position + _direction * _distance, Quaternion.identity).GetComponent<WVDBossFireStreamElement>();
        fireStreamElement.SetParameters(_direction, _timeIntervalToSpawnNextElement, _elementNumber++);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _canDamage)
        {
            other.GetComponent<WVDPlayer>().TakeDamage(_damage, true);
            _canDamage = false;
            Invoke("CanDamageAgain", _canDamageInterval);
        }
        else if (other.gameObject.CompareTag("Flammable"))
        {
            other.gameObject.GetComponent<WVDFlammable>().BurnObject(transform.position);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _canDamage)
        {
            other.GetComponent<WVDPlayer>().TakeDamage(_damage, true);
            _canDamage = false;
            Invoke("CanDamageAgain", _canDamageInterval);
        }
    }
    void CanDamageAgain()
    {
        _canDamage = true;
    }
}