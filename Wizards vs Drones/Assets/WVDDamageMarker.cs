using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WVDDamageMarker : MonoBehaviour
{
    [SerializeField]
    float _lifetime;
    GameObject _camera;
    TMP_Text _text;
    [SerializeField]
    float _ascendRate;
    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _text = GetComponent<TMP_Text>();
        FadeOut();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_camera.transform.position);
        transform.position += Vector3.up * _ascendRate * Time.deltaTime;
    }

    async void FadeOut()
    {
        float timer = 0.0f;
        while (timer < _lifetime)
        {
            _text.color = Color.Lerp(new Color(_text.color.r, _text.color.g, _text.color.b, 1.0f), new Color(_text.color.r, _text.color.g, _text.color.b, 0.0f), timer / _lifetime);
            timer += Time.deltaTime;
            await Task.Yield();
        }
        Destroy(gameObject);
        
    }
}
