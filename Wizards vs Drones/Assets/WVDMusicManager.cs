using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WVDMusicManager : MonoBehaviour
{
    [SerializeField]
    AudioSource _musicAS;
    [SerializeField]
    List<AudioClip> _combatMusic;
    int _currentCombatMusicIndex;
    [SerializeField]
    List<AudioClip> _shopMusic;
    int _currentShopMusicIndex;
    [SerializeField]
    AudioClip _bossMusic;
    [SerializeField]
    AudioClip _victoryMusic;
    [SerializeField]
    float _victoryMusicDelay;
    [SerializeField]
    float _musicFadePeriod;
    [SerializeField]
    WVDOptionsManager _optionsManagerScript;
    void Start()
    {

        //FadeNewMusicIn(PickNewRandomCombatMusicClip());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitialMusicSetup() // called from other Start function
    {        
        _currentCombatMusicIndex = Random.Range(0, _combatMusic.Count);
        _currentShopMusicIndex = Random.Range(0, _shopMusic.Count);

    }

    public void FadeCurrentMusicOutAndBossMusicIn()
    {
        FadeCurrentMusicOutAndNewMusicIn(_bossMusic);
    }
    public async void FadeCurrentMusicOutAndVictoryMusicIn()
    {
        float timer = 0.0f;
        while (timer < _victoryMusicDelay)
        {
            timer += Time.deltaTime;
            await Task.Yield();
        }
        FadeCurrentMusicOutAndNewMusicIn(_victoryMusic);
    }

    public AudioClip PickNewRandomCombatMusicClip() // Randomise each time but not same one
    {
        int newIndex = _currentCombatMusicIndex;
        if (Random.Range(0.0f, 1.0f) < 0.5f)
        {
            newIndex--;
        }
        else
        {
            newIndex++;
        }
        if (newIndex == -1)
        {
            newIndex = _combatMusic.Count - 1;
        }
        else if (newIndex == _combatMusic.Count)
        {
            newIndex = 0;
        }
        _currentCombatMusicIndex = newIndex;
        return _combatMusic[_currentCombatMusicIndex];
    }
    public AudioClip PickOtherShopMusicClip() // Pick the other as there are only two
    {
        if (_currentShopMusicIndex == 0)
        {
            _currentShopMusicIndex = 1;
        }
        else
        {
            _currentShopMusicIndex = 0;
        }
        return _shopMusic[_currentShopMusicIndex];
    }
    public async void FadeCurrentMusicOutAndNewMusicIn(AudioClip clip)
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
        _musicAS.Stop();
        FadeNewMusicIn(clip);
    }

    async void FadeNewMusicIn(AudioClip clip)
    {
        _musicAS.clip = clip;
        _musicAS.Play();
        float fadeInTimer = 0.0f;
        float fadeRate = _optionsManagerScript.MusicVolume * (1.0f / _musicFadePeriod);
        while (fadeInTimer < _musicFadePeriod)
        {
            _musicAS.volume += fadeRate * Time.deltaTime;
            fadeInTimer += Time.deltaTime;
            await Task.Yield();
        }
        _musicAS.volume = _optionsManagerScript.MusicVolume;
    }
}
