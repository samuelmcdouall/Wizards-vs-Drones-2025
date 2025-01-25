using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WVDMainMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject _mainMenuScreen;
    [SerializeField]
    GameObject _gameModeScreen;
    [SerializeField]
    GameObject _optionsModeScreen;
    [SerializeField]
    Image _whiteFadeScreen;
    [SerializeField]
    float _whiteFadeDuration;
    [SerializeField]
    AudioSource _musicAS;
    [SerializeField]
    float _musicFadePeriod;


    public void WVDClickPlayButton()
    {
        _mainMenuScreen.SetActive(false);
        _gameModeScreen.SetActive(true);
    }

    public void WVDClickNormalModeButton()
    {
        _whiteFadeScreen.gameObject.SetActive(true);
        FadeToWhite();
        FadeMusicOut();
    }

    public void WVDClickBackButton()
    {
        _mainMenuScreen.SetActive(true);
        _gameModeScreen.SetActive(false);
        _optionsModeScreen.SetActive(false);
    }

    // todo music controls next + put in place for mouse sensitivity, then put in a system for swapping out between the different game/shop/boss music

    async void FadeToWhite()
    {
        float fadeInTimer = 0.0f;
        while (fadeInTimer < _whiteFadeDuration)
        {
            float opacity = Mathf.Lerp(0.0f, 1.0f, fadeInTimer / _whiteFadeDuration);
            print("op: " + opacity);
            _whiteFadeScreen.color = new Color(1.0f,1.0f,1.0f, opacity);
            fadeInTimer += Time.deltaTime;
            await Task.Yield();
        }
        _whiteFadeScreen.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        SceneManager.LoadScene("GameScene");
    }

    async void FadeMusicOut()
    {
        float fadeOutTimer = 0.0f;
        float fadeRate = _musicAS.volume * (1.0f / _musicFadePeriod);
        while (fadeOutTimer < _musicFadePeriod)
        {
            _musicAS.volume -= fadeRate * Time.deltaTime;
            fadeOutTimer += Time.deltaTime;
            await Task.Yield();
        }
        _musicAS.volume = 0.0f;
    }




}
