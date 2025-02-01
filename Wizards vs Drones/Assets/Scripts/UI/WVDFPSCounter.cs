using TMPro;
using UnityEngine;

public class WVDFPSCounter : MonoBehaviour // Used for Debug purposes
{
    [Header("General")]
    [SerializeField]
    float _interval;
    float _counter;
    TMP_Text _text;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
        _counter = _interval;
    }
    void Update()
    {
        if (_counter < 0.0f)
        {
            _counter = _interval;
            _text.text = "FPS: " + 1.0f / Time.deltaTime;
        }
        else
        {
            _counter -= Time.deltaTime;
        }
    }
}