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
    void Start()
    {
        
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

    public void WVDClickTryAgainButton()
    {
        FadeToWhiteAndLoadScene("GameScene");
    }
    public void WVDClickQuitToMainMenuButton()
    {
        FadeToWhiteAndLoadScene("MainMenuScene");
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
}
