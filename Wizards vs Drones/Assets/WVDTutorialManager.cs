using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WVDTutorialManager : MonoBehaviour
{
    Dictionary<TutorialPart, WVDTutorialDetails> _tutorialDictionary = new Dictionary<TutorialPart, WVDTutorialDetails>();
    [SerializeField]
    GameObject _tutorialBackground;
    [SerializeField]
    TMP_Text _tutorialText;
    [SerializeField]
    float _delayPerCharacter;
    bool _canPressContinue;


    private void Start()
    {
        _tutorialDictionary.Add(TutorialPart.Intro, new WVDTutorialDetails("Oh no! Looks like almost everyone is away from the castle and these weird flying boxes called <color=#CC0000>drones</color> are invading! You'll need to defend the castle with that <color=#CC0000>fireball spell</color> you learnt recently (left click). <color=#CC0000>Navigate round the castle</color> (WASD) and don't forget you can make a quick escape with the <color=#CC0000>blink spell</color> (spacebar). Look out for those <color=#CC0000>power up gems</color> scattered around, they'll give you a nifty bonus (right click). I'll come and find you when things have died down a bit... good luck!", false)); // need to get the boolean values from JSON saved
        _tutorialDictionary.Add(TutorialPart.ElectricDrone, new WVDTutorialDetails("oh no!!!!", false));
    }
    public enum TutorialPart
    {
        Intro,
        ElectricDrone,
    }

    public async void DisplayTutorial(TutorialPart part, float delay)
    {
        float timer = 0.0f;
        while (timer < delay)
        {
            timer += Time.deltaTime;
            await Task.Yield();
        }
        Time.timeScale = 0.0f;
        _tutorialText.text = "";
        _tutorialBackground.SetActive(true);
        WVDFunctionsCheck.InTutorial = true;
        WVDTutorialDetails details = new WVDTutorialDetails("DEFAULT", false);
        _tutorialDictionary.TryGetValue(part, out details);
        if (details.TutorialInformation != "DEFAULT" && !details.BeenPlayedBefore)
        {
            string fullInfo = details.TutorialInformation;
            for (int i = 0; i < fullInfo.Length; i++)
            {
                if (fullInfo[i] == '<')
                {
                    while (fullInfo[i] != '>')
                    {
                        _tutorialText.text += fullInfo[i];
                        i++;
                    }
                    _tutorialText.text += ">";
                }
                else
                {
                    float letterTimer = 0.0f;
                    while (letterTimer < _delayPerCharacter)
                    {
                        letterTimer += Time.unscaledDeltaTime;
                        await Task.Yield();
                    }
                    _tutorialText.text += fullInfo[i];
                }
            }
            _tutorialText.text += "\nPress enter to continue";
            _canPressContinue = true;
            // set to true in JSON, the reset button in main menu will set all to false in JSON

        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _canPressContinue)
        {
            _tutorialBackground.SetActive(false);
            _canPressContinue = false;
            Time.timeScale = 1.0f;
            WVDFunctionsCheck.InTutorial = false;
        }
    }
}
