using UnityEngine;

public class WVDBatteryCachePickUp : MonoBehaviour
{
    [Header("Value")]
    [SerializeField]
    int _minValue;
    [SerializeField]
    int _maxValue;
    int _value;

    [Header("Other")]
    WVDSoundManager _soundManager;
    WVDStatsManager _statsManager;

    void Start()
    {
        _value = Random.Range(_minValue, _maxValue + 1);
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        _statsManager = GameObject.FindGameObjectWithTag("StatsManager").GetComponent<WVDStatsManager>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUpTrigger"))
        {
            other.gameObject.transform.parent.gameObject.GetComponent<WVDPlayer>().BatteryCount += _value;
            _soundManager.PlaySFXAtPlayer(_soundManager.PickupBatterySFX);
            _statsManager.BatteriesCollected += _value;
            Destroy(gameObject);
        }
    }
}