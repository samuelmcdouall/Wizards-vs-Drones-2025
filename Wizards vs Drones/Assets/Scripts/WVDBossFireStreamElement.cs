using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDBossFireStreamElement : MonoBehaviour
{
    [System.NonSerialized]
    public Vector3 Direction;
    [System.NonSerialized]
    public float TimeIntervalToSpawnNextElement;
    [System.NonSerialized]
    public int ElementNumber;
    [SerializeField]
    float _distance;
    [SerializeField]
    float _maxNumberElements;
    [SerializeField]
    float _lifeTime;
    [SerializeField]
    GameObject _fireStreamElementPrefab;

    bool _canDamage;
    [SerializeField]
    float _canDamageInterval;
    [SerializeField]
    int _damage;

    void Start()
    {
        _canDamage = true;
        Destroy(gameObject, _lifeTime);
        if (ElementNumber < _maxNumberElements)
        {
            Invoke("SpawnNextElement", TimeIntervalToSpawnNextElement);
        }
    }

    public void SetParameters(Vector3 direction, float timeInterval, int elementNumber)
    {
        Direction = direction.normalized;
        TimeIntervalToSpawnNextElement = timeInterval;
        ElementNumber = elementNumber;
    }
    
    void SpawnNextElement()
    {
        WVDBossFireStreamElement fireStreamElement = Instantiate(_fireStreamElementPrefab, transform.position + Direction * _distance, Quaternion.identity).GetComponent<WVDBossFireStreamElement>();
        fireStreamElement.SetParameters(Direction, TimeIntervalToSpawnNextElement, ElementNumber++);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _canDamage)
        {
            other.GetComponent<WVDPlayer>().TakeDamage(_damage);
            _canDamage = false;
            Invoke("CanDamageAgain", _canDamageInterval);
        }   
    }

    void CanDamageAgain()
    {
        _canDamage = true;
    }
}
