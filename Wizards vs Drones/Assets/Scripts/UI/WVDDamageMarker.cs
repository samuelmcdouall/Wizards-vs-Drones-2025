using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WVDDamageMarker : MonoBehaviour
{
    [Header("Values")]
    [SerializeField]
    float _lifetime;
    [SerializeField]
    float _ascendRate;

    [Header("Text")]
    GameObject _camera;
    TMP_Text _text;
    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _text = GetComponent<TMP_Text>();
        FadeOut();
    }
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
            _text.color = Color.Lerp(
                new Color(_text.color.r, _text.color.g, _text.color.b, 1.0f), 
                new Color(_text.color.r, _text.color.g, _text.color.b, 0.0f), 
                timer / _lifetime
                );
            timer += Time.deltaTime;
            await Task.Yield();
        }
        Destroy(gameObject);
    }
}