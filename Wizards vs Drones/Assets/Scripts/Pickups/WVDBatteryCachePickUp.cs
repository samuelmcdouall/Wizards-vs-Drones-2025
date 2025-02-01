using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDBatteryCachePickUp : MonoBehaviour
{
    WVDSoundManager _soundManager;
    WVDStatsManager _statsManager;
    WVDTutorialManager _tutorialManager;
    [SerializeField]
    int _minValue;
    [SerializeField]
    int _maxValue;
    int _value;

    // Start is called before the first frame update
    void Start()
    {
        _value = Random.Range(_minValue, _maxValue + 1);
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        _statsManager = GameObject.FindGameObjectWithTag("StatsManager").GetComponent<WVDStatsManager>();
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<WVDTutorialManager>();
    }

    private void OnTriggerEnter(Collider other)
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
