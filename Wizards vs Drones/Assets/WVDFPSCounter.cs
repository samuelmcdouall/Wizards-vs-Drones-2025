using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WVDFPSCounter : MonoBehaviour
{
    [SerializeField]
    float _interval;
    float _counter;
    TMP_Text _text;
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TMP_Text>();
        _counter = _interval;
    }

    // Update is called once per frame
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
