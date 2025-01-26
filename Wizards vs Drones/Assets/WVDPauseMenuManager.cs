using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WVDPauseMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject _pauseMenuScreen;
    [SerializeField]
    GameObject _pauseOptionsMenuScreen;
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
    WVDSoundManager _soundManager;

    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_pauseOptionsMenuScreen.activeSelf)
            {
                _pauseMenuScreen.SetActive(true);
                _pauseOptionsMenuScreen.SetActive(false);
            }
            else if (_pauseMenuScreen.activeSelf)
            {
                _pauseMenuScreen.SetActive(false);
                WVDFunctionsCheck.InPauseMenu = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1.0f;
            }
            else
            {
                ShowOptionsMenu();
            }
        }
    }

    void ShowOptionsMenu()
    {
        _pauseMenuScreen.SetActive(true);
        WVDFunctionsCheck.InPauseMenu = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0.0f;
    }

    public void WVDClickResumeButton()
    {
        _pauseMenuScreen.SetActive(false);
        WVDFunctionsCheck.InPauseMenu = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
    }

    public void WVDClickOptionsButton()
    {
        _pauseMenuScreen.SetActive(false);
        _pauseOptionsMenuScreen.SetActive(true);
        Time.timeScale = 1.0f;
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
        Time.timeScale = 0.0f;
    }

    public void WVDClickOptionsBackButton()
    {
        _pauseMenuScreen.SetActive(true);
        _pauseOptionsMenuScreen.SetActive(false);
        Time.timeScale = 1.0f;
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
        Time.timeScale = 0.0f;
    }

    public void WVDClickQuitToMenuButton()
    {
        _whiteFadeScreen.gameObject.SetActive(true);
        FadeToWhite();
        FadeMusicOut();
        Time.timeScale = 1.0f;
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
        Time.timeScale = 0.0f;
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
            _whiteFadeScreen.color = new Color(1.0f, 1.0f, 1.0f, opacity);
            fadeInTimer += Time.unscaledDeltaTime;
            await Task.Yield();
        }
        _whiteFadeScreen.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        SceneManager.LoadScene("MainMenuScene");
    }

    async void FadeMusicOut()
    {
        float fadeOutTimer = 0.0f;
        float fadeRate = _optionsManagerScript.MusicVolume * (1.0f / _musicFadePeriod);
        while (fadeOutTimer < _musicFadePeriod)
        {
            _musicAS.volume -= fadeRate * Time.unscaledDeltaTime;
            fadeOutTimer += Time.unscaledDeltaTime;
            await Task.Yield();
        }
        _musicAS.volume = 0.0f;
    }
}
