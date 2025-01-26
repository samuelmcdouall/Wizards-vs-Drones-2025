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
    GameObject _optionsScreen;
    [SerializeField]
    Image _whiteFadeScreen;
    [SerializeField]
    float _whiteFadeDuration;
    [SerializeField]
    AudioSource _musicAS;
    [SerializeField]
    float _musicFadePeriod;
    [SerializeField]
    WVDOptionsManager _optionsManagerScript;


    void Start()
    {

    }
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
        _optionsScreen.SetActive(false);
    }

    public void WVDClickOptionsButton()
    {
        _mainMenuScreen.SetActive(false);
        _optionsScreen.SetActive(true);
    }

    public void WVDChangeMusicSlider()
    {
        _optionsManagerScript.MusicVolume = _optionsManagerScript.MusicSlider.value;
        _musicAS.volume = _optionsManagerScript.MusicVolume;
        PlayerPrefs.SetFloat(WVDOptionsStrings.MusicVolume, _optionsManagerScript.MusicVolume);
        PlayerPrefs.Save();
    }

    public void WVDChangeSFXSlider()
    {
        _optionsManagerScript.SFXVolume = _optionsManagerScript.SFXSlider.value;
        PlayerPrefs.SetFloat(WVDOptionsStrings.SFXVolume, _optionsManagerScript.SFXVolume);
        PlayerPrefs.Save();
    }
    public void WVDChangeMouseSensitivitySlider()
    {
        _optionsManagerScript.MouseSensitivity = _optionsManagerScript.MouseSlider.value;
        PlayerPrefs.SetFloat(WVDOptionsStrings.MouseSensitivity, _optionsManagerScript.MouseSensitivity);
        PlayerPrefs.Save();
    }

    async void FadeToWhite()
    {
        float fadeInTimer = 0.0f;
        while (fadeInTimer < _whiteFadeDuration)
        {
            float opacity = Mathf.Lerp(0.0f, 1.0f, fadeInTimer / _whiteFadeDuration);
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
        float fadeRate = _optionsManagerScript.MusicVolume * (1.0f / _musicFadePeriod);
        while (fadeOutTimer < _musicFadePeriod)
        {
            _musicAS.volume -= fadeRate * Time.deltaTime;
            fadeOutTimer += Time.deltaTime;
            await Task.Yield();
        }
        _musicAS.volume = 0.0f;
    }
    public void FadeMusicIn() // keep the rest here in case want to fade in, but just having immediately play sounds better really for the main menu (if want it back, need to make this function async)
    {
        //_musicAS.volume = 0.0f;
        //float fadeInTimer = 0.0f;
        //float fadeRate = _optionsManagerScript.MusicVolume * (1.0f / _musicFadePeriod);
        //while (fadeInTimer < _musicFadePeriod)
        //{
        //    _musicAS.volume += fadeRate * Time.deltaTime;
        //    fadeInTimer += Time.deltaTime;
        //    await Task.Yield();
        //}
        _musicAS.volume = _optionsManagerScript.MusicVolume; 

    }




}
