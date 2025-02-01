using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WVDFadeFromWhiteScreen : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    Image _whiteFadeScreen;
    [SerializeField]
    float _whiteFadeDuration;

    [Header("Other")]
    [SerializeField]
    bool _cursorVisibleAtStart;
    WVDTutorialManager _tutorialManager;

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
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager")?.GetComponent<WVDTutorialManager>(); // If we have a tutorial manager (i.e. in the game scene)
        WVDFunctionsCheck.WhiteScreenFading = true;
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
        if (_tutorialManager)
        {
            WVDEventBus.Raise(new WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart.Intro, 1.0f));
        }
        WVDFunctionsCheck.WhiteScreenFading = false;
    }
}