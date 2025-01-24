using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WVDFadeFromWhiteScreen : MonoBehaviour
{
    [SerializeField]
    Image _whiteFadeScreen;
    [SerializeField]
    float _whiteFadeDuration;
    [SerializeField]
    bool _cursorVisibleAtStart;
    void Start()
    {
        if (_cursorVisibleAtStart)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        WVDFunctionsCheck.IsDead = false;
        Invoke("FadeFromWhite", 1.0f);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void FadeFromWhite()
    {
        float fadeInTimer = 0.0f;
        while (fadeInTimer < _whiteFadeDuration)
        {
            float opacity = Mathf.Lerp(1.0f, 0.0f, fadeInTimer / _whiteFadeDuration);
            _whiteFadeScreen.color = new Color(1.0f, 1.0f, 1.0f, opacity);
            fadeInTimer += Time.deltaTime;
            await Task.Yield();
        }
        _whiteFadeScreen.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _whiteFadeScreen.gameObject.SetActive(false);
    }
}
