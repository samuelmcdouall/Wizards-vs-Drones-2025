using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WVDGameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] 
    List<GameObject> _UIElementsToTurnOff;
    [SerializeField]
    Image _whiteFadeScreen;
    [SerializeField]
    float _whiteFadeDuration;
    [SerializeField]
    float _gameOverMenuDelay;
    [SerializeField]
    GameObject _gameOverMenu;
    [SerializeField]
    GameObject _victoryScreen;
    [SerializeField]
    TMP_Text _victoryText;
    [SerializeField]
    WVDStatsManager _statsManager;
    [SerializeField]
    TMP_Text _statsText;

    [Header("Music")]
    [SerializeField]
    AudioSource _musicAS;
    [SerializeField]
    float _musicFadePeriod;
    [SerializeField]
    WVDOptionsManager _optionsManagerScript;
    WVDSoundManager _soundManager;

    [Header("Other")]
    [SerializeField]
    WVDSaveDataManager _saveDataManager;

    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
    }
    public void WVDClickTryAgainButton() // Also play again if victory
    {
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
        FadeMusicOut();
        FadeToWhiteAndLoadScene("GameScene");
    }
    public void WVDClickQuitToMainMenuButton()
    {
        _soundManager.PlaySFXAtPlayer(_soundManager.UIButtonSFX);
        FadeMusicOut();
        FadeToWhiteAndLoadScene("MainMenuScene");
    }
    public void TriggerGameOver()
    {
        WVDFunctionsCheck.IsDead = true;
        foreach (GameObject ui in _UIElementsToTurnOff)
        {
            ui.SetActive(false);
        }
        Invoke("ShowGameOverMenu", _gameOverMenuDelay);
    }
    void ShowGameOverMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _gameOverMenu.SetActive(true);
    }
    public void ShowVictoryScreenAfterDelay(float delay)
    {
        Invoke("ShowVictoryScreen", delay);
    }
    void ShowVictoryScreen()
    {
        WVDFunctionsCheck.HasWon = true;
        foreach (GameObject ui in _UIElementsToTurnOff)
        {
            ui.SetActive(false);
        }
        if (!_saveDataManager.SaveData.ChallengeModeUnlocked) // will get this from the save file
        {
            _victoryText.text += "\nChallenge mode has now been unlocked!";
            _saveDataManager.SaveData.ChallengeModeUnlocked = true;
            _saveDataManager.SaveNewData();
        }

        _statsManager.TimerStopped = true;
        int totalSeconds = (int)_statsManager.TimeTaken;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        string secondsString;
        if (seconds < 10)
        {
            secondsString = "0" + seconds;
        }
        else
        {
            secondsString = "" + seconds;
        }
        _statsText.text = $"Electric drones destroyed: {_statsManager.ElectricDronesDestroyed}\n" +
                          $"Laser drones destroyed: {_statsManager.LaserDronesDestroyed}\n" +
                          $"Fast drones destroyed: {_statsManager.FastDronesDestroyed}\n" +
                          $"Teleport drones destroyed: {_statsManager.TeleportDronesDestroyed}\n" +
                          $"Batteries collected: {_statsManager.BatteriesCollected}\n" +
                          $"Time taken: {minutes}:{secondsString}";


        _victoryScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }
    async void FadeToWhiteAndLoadScene(string sceneToLoad)
    {
        _whiteFadeScreen.gameObject.SetActive(true);
        WVDFunctionsCheck.WhiteScreenFading = true;
        float fadeInTimer = 0.0f;
        while (fadeInTimer < _whiteFadeDuration)
        {
            float opacity = Mathf.Lerp(0.0f, 1.0f, fadeInTimer / _whiteFadeDuration);
            _whiteFadeScreen.color = new Color(1.0f, 1.0f, 1.0f, opacity);
            fadeInTimer += Time.deltaTime;
            await Task.Yield();
        }
        _whiteFadeScreen.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        SceneManager.LoadScene(sceneToLoad);
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