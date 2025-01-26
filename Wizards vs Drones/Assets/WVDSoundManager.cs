using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDSoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource _SFXAS;
    [SerializeField]
    WVDOptionsManager _optionsManager;
    public AudioClip PlayerProjectileLaunchSFX; // potentially multiple
    public AudioClip PlayerProjectileImpactSFX;
    

    public void PlaySFXAtPlayer(AudioClip clip) // to be played on the audio listener, otherwise define a point
    {
        _SFXAS.PlayOneShot(clip, _optionsManager.SFXVolume);
    }
    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position, _optionsManager.SFXVolume);
    }
}
