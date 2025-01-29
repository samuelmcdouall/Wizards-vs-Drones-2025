using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
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
    [SerializeField]
    WVDSaveDataManager _saveDataManager;



    private void Start()
    {
        WVDSaveData saveData = _saveDataManager.SaveData;

        _tutorialDictionary.Add(TutorialPart.Intro, new WVDTutorialDetails("Oh no! Looks like almost everyone is away from the castle and these weird flying boxes called <color=#EE8322>drones</color> are invading! You'll need to defend the castle with that <color=#EE8322>fireball spell</color> you learnt recently (left click). <color=#EE8322>Navigate round the castle</color> (WASD) and don't forget you can make a quick escape with the <color=#EE8322>blink spell</color> (spacebar). Look out for those <color=#EE8322>power up gems</color> scattered around, they'll give you a nifty bonus (right click). I'll come and find you when things have died down a bit... good luck!", saveData.IntroBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.ElectricDrone, new WVDTutorialDetails("These <color=#EE8322>brown drones</color> will try to <color=#EE8322>zap</color> you. Should be easy enough to deal with, but there are a lot of them!", saveData.ElectricDroneBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.LaserDrone, new WVDTutorialDetails("The <color=#EE8322>red drones</color> are a bit different and will engage at range. They seem to be firing some sort of <color=#EE8322>laser</color> at you. ", saveData.LaserDroneBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.FastDrone, new WVDTutorialDetails("Things are heating up now, I've spotted some <color=#EE8322>green drones</color> lurking about that <color=#EE8322>move pretty quickly</color>", saveData.FastDroneBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.TeleportDrone, new WVDTutorialDetails("Thats a neat trick... these <color=#EE8322>orange drones</color> seem to be able to <color=#EE8322>teleport</color>. Are we sure they aren't magic?", saveData.TeleportDroneBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.Shop, new WVDTutorialDetails("Ah! You made it! Told you I'd be back... once things got a bit safer. Come find me at one of the <color=#EE8322>cauldrons</color> around the castle with <color=#EE8322>green exlir</color> in it. If you ever have a problem finding it, look for a <color=#EE8322>star trail</color>. That'll lead you to straight to it. If you bring me some of those <color=#EE8322>batteries</color> from destroyed drones I can show you some potions to <color=#EE8322>upgrade</color> your power (E). I can't stick around forever, I'm afraid. More drones will be on their way soon, so hurry! The castle is mostly locked down by <color=#EE8322>magical fire</color> but I'll work to best to dispel it as soon as I can.", saveData.ShopBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.AttackPowerUp, new WVDTutorialDetails("<color=#EE8322>Red power ups</color> give you an <color=#EE8322>explosive meteor to throw</color>. Combine it with <color=#EE8322>one black power up</color> and you can fling <color=#EE8322>fireballs in all directions</color>. Combine it <color=#EE8322>two black power ups</color> and you'll <color=#EE8322>summon a ghost</color> to attack for you.", saveData.AttackPowerUpBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.ShieldPowerUp, new WVDTutorialDetails("<color=#EE8322>Blue power ups</color> give you a <color=#EE8322>shield</color> that'll protect from <color=#EE8322>ranged attacks</color>. Combine it with <color=#EE8322>one black power up</color> and the shield will <color=#EE8322>reflect any ranged attacks</color>. Combine it with <color=#EE8322>two black power ups</color> and it'll <color=#EE8322>zap any drones close by</color>. These shields are temporary.", saveData.ShieldPowerUpBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.HealPowerUp, new WVDTutorialDetails("<color=#EE8322>Green power ups</color> will <color=#EE8322>heal</color> you. Combine it with <color=#EE8322>one black power up</color> and each <color=#EE8322>hit on an enemy will heal</color> you. Combine it with <color=#EE8322>two black power ups</color> and you'll be <color=#EE8322>immune to damage</color>. The healing from enemy damage and immune to damage effects are temporary.", saveData.HealPowerUpBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.TrapPowerUp, new WVDTutorialDetails("<color=#EE8322>Yellow power ups</color> will deploy a <color=#EE8322>trap that slows drones</color> that cross over it. Combine it with <color=#EE8322>one black power up</color> and the trap will <color=#EE8322>damage drones</color> instead. Combine it with <color=#EE8322>two black power ups</color> and the trap will <color=#EE8322>explode damaging nearby drones</color> as well.", saveData.TrapPowerUpBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.Tome, new WVDTutorialDetails("Excellent! You found one of them! Those <color=#EE8322>Tomes</color> are rare but powerful things. They have the power to <color=#EE8322>destroy every drone</color> in the castle at once!", saveData.TomeBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.GreatHall, new WVDTutorialDetails("I've managed to dispel the <color=#EE8322>orange flames</color>. The <color=#EE8322>Great Hall</color> is open again but don't go in there to eat, you still have drones to defeat!", saveData.GreatHallBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.Library, new WVDTutorialDetails("And... yes! That should be the <color=#EE8322>blue flames</color> gone now. The <color=#EE8322>Library</color> is now available. That reminds me, I still have to return that book on spotted dragons!", saveData.LibraryBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.Dungeon, new WVDTutorialDetails("Eye of newt, toe of... ferret? Oh whatever. The <color=#EE8322>green flames</color> are no more. The <color=#EE8322>Dungeon</color> can be entered once more. I hope someone remembered to feed whatever is down there. ", saveData.DungeonBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.Boss, new WVDTutorialDetails("Oh no this is bad. So the good news is that was the last of the drones... the bad news is that <color=#EE8322>demon</color> we had locked the Dungeon broke out whilst we were distracted. He's resummoned the magical fires, <color=#EE8322>trapping us in the Courtyard</color>. He's also much more powerful than any drone and has <color=#EE8322>destroyed all the power up gems</color>... but we can't let him escape! Defeat him at all costs!", saveData.BossBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.SpawnOnDeathBuff, new WVDTutorialDetails("Just like a hydra! This drone had an <color=#EE8322>upgrade to summon two more</color> in its place!", saveData.SpawnOnDeathBuffBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.ShieldBuff, new WVDTutorialDetails("That <color=#EE8322>shield upgrade</color> will be back shortly. Smash it whilst it's down!", saveData.ShieldBuffBeenPlayedBefore));
        _tutorialDictionary.Add(TutorialPart.SlowBuff, new WVDTutorialDetails("Brrrrr... whatever is emanating out of that one is freezing! This drone's <color=#EE8322>upgrade is slowing</color> you down.", saveData.SlowBuffBeenPlayedBefore));
    }
    public enum TutorialPart
    {
        Intro,
        ElectricDrone,
        LaserDrone,
        FastDrone,
        TeleportDrone,
        SpawnOnDeathBuff,
        ShieldBuff,
        SlowBuff,
        Shop,
        AttackPowerUp,
        ShieldPowerUp,
        HealPowerUp,
        TrapPowerUp,
        Tome,
        GreatHall,
        Library,
        Dungeon,
        Boss
    }

    public async void DisplayTutorial(TutorialPart part, float delay)
    {
        WVDTutorialDetails details = new WVDTutorialDetails("DEFAULT", false);
        _tutorialDictionary.TryGetValue(part, out details);
        if (details.TutorialInformation != "DEFAULT" && !details.BeenPlayedBefore)
        {
            float timer = 0.0f;
            while (timer < delay)
            {
                timer += Time.deltaTime;
                await Task.Yield();
            }
            Time.timeScale = 0.0f;
            _tutorialText.text = "";
            if (!_tutorialBackground) // just in case try to quit whilst a tutorial is loading
            {
                Time.timeScale = 1.0f;
                return;
            }
            _tutorialBackground.SetActive(true);
            WVDFunctionsCheck.InTutorial = true;
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

            _tutorialDictionary[part] = new WVDTutorialDetails(_tutorialDictionary[part].TutorialInformation, true); // for this session of the tutorial manager set to true


            switch (part)
            {
                case TutorialPart.Intro:
                    _saveDataManager.SaveData.IntroBeenPlayedBefore = true;
                    break;
                case TutorialPart.ElectricDrone:
                    _saveDataManager.SaveData.ElectricDroneBeenPlayedBefore = true;
                    break;
                case TutorialPart.LaserDrone:
                    _saveDataManager.SaveData.LaserDroneBeenPlayedBefore = true;
                    break;
                case TutorialPart.FastDrone:
                    _saveDataManager.SaveData.FastDroneBeenPlayedBefore = true;
                    break;
                case TutorialPart.TeleportDrone:
                    _saveDataManager.SaveData.TeleportDroneBeenPlayedBefore = true;
                    break;
                case TutorialPart.SpawnOnDeathBuff:
                    _saveDataManager.SaveData.SpawnOnDeathBuffBeenPlayedBefore = true;
                    break;
                case TutorialPart.ShieldBuff:
                    _saveDataManager.SaveData.ShieldBuffBeenPlayedBefore = true;
                    break;
                case TutorialPart.SlowBuff:
                    _saveDataManager.SaveData.SlowBuffBeenPlayedBefore = true;
                    break;
                case TutorialPart.Shop:
                    _saveDataManager.SaveData.ShopBeenPlayedBefore = true;
                    break;
                case TutorialPart.AttackPowerUp:
                    _saveDataManager.SaveData.AttackPowerUpBeenPlayedBefore = true;
                    break;
                case TutorialPart.ShieldPowerUp:
                    _saveDataManager.SaveData.ShieldPowerUpBeenPlayedBefore = true;
                    break;
                case TutorialPart.HealPowerUp:
                    _saveDataManager.SaveData.HealPowerUpBeenPlayedBefore = true;
                    break;
                case TutorialPart.TrapPowerUp:
                    _saveDataManager.SaveData.TrapPowerUpBeenPlayedBefore = true;
                    break;
                case TutorialPart.Tome:
                    _saveDataManager.SaveData.TomeBeenPlayedBefore = true;
                    break;
                case TutorialPart.GreatHall:
                    _saveDataManager.SaveData.GreatHallBeenPlayedBefore = true;
                    break;
                case TutorialPart.Library:
                    _saveDataManager.SaveData.LibraryBeenPlayedBefore = true;
                    break;
                case TutorialPart.Dungeon:
                    _saveDataManager.SaveData.DungeonBeenPlayedBefore = true;
                    break;
                case TutorialPart.Boss:
                    _saveDataManager.SaveData.BossBeenPlayedBefore = true;
                    break;
            }
            _saveDataManager.SaveNewData(); // for future sessions saving into JSON


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
