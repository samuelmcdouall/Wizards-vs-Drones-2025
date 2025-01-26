using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WVDGameOverManager : MonoBehaviour
{
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
    AudioSource _musicAS;
    [SerializeField]
    float _musicFadePeriod;
    [SerializeField]
    WVDOptionsManager _optionsManagerScript;
    [SerializeField]
    GameObject _victoryScreen;
    [SerializeField]
    GameObject _challengeModeText;
    [SerializeField]
    bool _ifBeatenGameBefore;
    WVDSoundManager _soundManager;

    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
    }

    // Update is called once per frame
    void Update()
    {

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
        if (!_ifBeatenGameBefore) // will get this from the save file
        {
            _challengeModeText.SetActive(true);
            // set the above boolean to true
        }
        _victoryScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }


    async void FadeToWhiteAndLoadScene(string sceneToLoad)
    {
        _whiteFadeScreen.gameObject.SetActive(true);
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
