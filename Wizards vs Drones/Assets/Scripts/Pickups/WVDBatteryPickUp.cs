using System.Collections;
using UnityEngine;

public class WVDBatteryPickUp : MonoBehaviour
{
    [Header("Value")]
    [SerializeField]
    int _value;

    [Header("Flashing Animation")]
    [SerializeField]
    bool _dontTimeOut;
    [SerializeField]
    float _lifeTime;
    [SerializeField]
    float _startFlashingThreshold;
    [SerializeField]
    float _flashPeriod;
    float _timer;
    [SerializeField]
    GameObject _batteryModel;
    Coroutine _flashCoroutine;

    [Header("Other")]
    WVDSoundManager _soundManager;
    WVDStatsManager _statsManager;
    WVDTutorialManager _tutorialManager;
    Rigidbody _rb;

    void Start()
    {
        _timer = _lifeTime;
        if (!_dontTimeOut)
        {
            Destroy(gameObject, _lifeTime);
        }
        _rb = GetComponent<Rigidbody>();
        AddRandomForceAndTorque();
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        _statsManager = GameObject.FindGameObjectWithTag("StatsManager").GetComponent<WVDStatsManager>();
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<WVDTutorialManager>();
    }
    void Update()
    {
        if (!_dontTimeOut)
        {
            if (_timer < _startFlashingThreshold)
            {
                if (_flashCoroutine == null)
                {
                    _flashCoroutine = StartCoroutine(ChangeAfterFlashPeriod());
                }
            }
            _timer -= Time.deltaTime;
        }
    }
    void AddRandomForceAndTorque()
    {
        float randX = Random.Range(-200.0f, 200.0f);
        float randY = Random.Range(200.0f, 300.0f);
        float randZ = Random.Range(-200.0f, 200.0f);
        _rb.AddForce(new Vector3(randX, randY, randZ));
        randX = Random.Range(-100.0f, 100.0f);
        randY = Random.Range(-100.0f, 100.0f);
        randZ = Random.Range(-100.0f, 100.0f);
        _rb.AddTorque(new Vector3(randX, randY, randZ));
    }
    IEnumerator ChangeAfterFlashPeriod()
    {
        yield return new WaitForSeconds(_flashPeriod);
        _batteryModel.SetActive(!_batteryModel.activeSelf);
        _flashCoroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUpTrigger"))
        {
            WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.Battery, 1.0f));
            other.gameObject.transform.parent.gameObject.GetComponent<WVDPlayer>().BatteryCount += _value;
            _soundManager.PlaySFXAtPlayer(_soundManager.PickupBatterySFX);
            _statsManager.BatteriesCollected += _value;
            Destroy(gameObject);
        }
    }
}