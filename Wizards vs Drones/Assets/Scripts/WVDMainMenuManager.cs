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
    GameObject _difficultyScreen;
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
    WVDSoundManager _soundManager;
    [SerializeField]
    WVDSaveDataManager _saveDataManager;
    [SerializeField]
    GameObject _challengeModeLocked;
    [SerializeField]
    GameObject _challengeModeUnlocked;
    WVDChallengeModeManager _challengeModeManager;


    void Start()
    {
        Time.timeScale = 1.0f;
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
        _challengeModeManager = GameObject.FindGameObjectWithTag("ChallengeModeManager").GetComponent<WVDChallengeModeManager>();
        if (_saveDataManager.SaveData.ChallengeModeUnlocked)
        {
            _challengeModeUnlocked.SetActive(true);
        }
        else
        {
            _challengeModeLocked.SetActive(true);
        }
    }
    public void WVDClickPlayButton()
    {
        _mainMenuScreen.SetActive(false);
        _difficultyScreen.SetActive(true);
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
    }
    public void WVDClickEasyButton()
    {
        ToGameModeScreen();
        _challengeModeManager.SelectedDifficulty = WVDChallengeModeManager.Difficulty.Easy;
    }

    public void WVDClickMediumButton()
    {
        ToGameModeScreen();
        _challengeModeManager.SelectedDifficulty = WVDChallengeModeManager.Difficulty.Medium;
    }
    public void WVDClickHardButton()
    {
        ToGameModeScreen();
        _challengeModeManager.SelectedDifficulty = WVDChallengeModeManager.Difficulty.Hard;
    }

    private void ToGameModeScreen()
    {
        _difficultyScreen.SetActive(false);
        _gameModeScreen.SetActive(true);
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
    }

    public void WVDClickNormalModeButton()
    {
        _whiteFadeScreen.gameObject.SetActive(true);
        _challengeModeManager.ChallengeModeActive = false;
        FadeToWhite(true);
        FadeMusicOut();
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
    }
    public void WVDClickChallengeModeButton() // todo add challenge mode going into
    {
        _whiteFadeScreen.gameObject.SetActive(true);
        _challengeModeManager.ChallengeModeActive = true;
        FadeToWhite(true);
        FadeMusicOut();
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
    }

    public void WVDClickBackButton()
    {
        _mainMenuScreen.SetActive(true);
        _difficultyScreen.SetActive(false);
        _gameModeScreen.SetActive(false);
        _optionsScreen.SetActive(false);
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
    }

    public void WVDClickOptionsButton()
    {
        _mainMenuScreen.SetActive(false);
        _optionsScreen.SetActive(true);
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
    }

    public void WVDClickQuitButton()
    {
        _whiteFadeScreen.gameObject.SetActive(true);
        FadeToWhite(false);
        FadeMusicOut();
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
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

    public void WVDResetTutorialTipsButton()
    {
        WVDSaveData saveData = new WVDSaveData(_saveDataManager.SaveData.ChallengeModeUnlocked);
        _saveDataManager.SaveNewData(saveData);
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
    }

    async void FadeToWhite(bool loadGame)
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
        if (loadGame)
        {
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Application.Quit();
        }
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
