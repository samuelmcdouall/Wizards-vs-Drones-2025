using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDSoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource _SFXAS;
    [SerializeField]
    WVDOptionsManager _optionsManager;
    public AudioClip PlayerProjectileLaunchSFX1;
    public AudioClip PlayerProjectileLaunchSFX2;
    public AudioClip PlayerProjectileImpactSFX;
    public AudioClip PlayerDashSFX;
    public AudioClip HealPowerUpSFX;
    public AudioClip LifestealPowerUpSFX;
    public AudioClip InvulnerablePowerUpSFX;
    public AudioClip CirclePowerUpSFX;
    public AudioClip GrenadeThrowPowerUpSFX;
    public AudioClip GrenadeImpactPowerUpSFX;
    public AudioClip GhostPowerUpSFX;
    public AudioClip TrapPowerUpSFX;
    public AudioClip ShieldPowerUpSFX;
    public AudioClip PickupPowerUpSFX;
    public AudioClip TomePowerUpSFX;
    public AudioClip PickupBatterySFX;
    public AudioClip PlayerHitSFX1;
    public AudioClip PlayerHitSFX2;
    public AudioClip PlayerHitSFX3;
    public AudioClip PlayerDeathSFX1;
    public AudioClip PlayerDeathSFX2;
    public AudioClip PlayerDeathSFX3;
    public AudioClip DroneSpawnSFX;
    public AudioClip DroneZapSFX;
    public AudioClip DroneLaserLauchSFX;
    public AudioClip DroneLaserCollideSFX; // yet to find
    public AudioClip DroneTeleportSFX;
    public AudioClip DroneBlowUpSFX;
    public AudioClip BossBlowUpGateSFX;
    public AudioClip BossEvilShortLaughSFX; // blowing up gate
    public AudioClip BossEvilLongLaughSFX; // healing
    public AudioClip BossSpawnProjectileSFX1;
    public AudioClip BossSpawnProjectileSFX2;
    public AudioClip BossSpawnProjectileSFX3;
    public AudioClip BossProjectileImpactSFX;
    public AudioClip BossSpawnFireElementSFX;
    public AudioClip BossDeathSFX;
    public AudioClip UIButtonSFX;
    public AudioClip BuyButtonSFX;



    

    public void PlaySFXAtPlayer(AudioClip clip, float volumeModifier = 1.0f) // to be played on the audio listener, otherwise define a point
    {
        _SFXAS.PlayOneShot(clip, _optionsManager.SFXVolume * volumeModifier);
    }
    public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volumeModifier = 1.0f)
    {
        AudioSource.PlayClipAtPoint(clip, position, _optionsManager.SFXVolume * volumeModifier);
    }
    public void PlayRandomSFXAtPlayer(AudioClip[] clips, float volumeModifier = 1.0f) // to be played on the audio listener, otherwise define a point
    {
        AudioClip chosenClip = clips[Random.Range(0, clips.Length)];
        _SFXAS.PlayOneShot(chosenClip, _optionsManager.SFXVolume * volumeModifier);
    }
    public void PlayRandomSFXAtPoint(AudioClip[] clips, Vector3 position, float volumeModifier = 1.0f)
    {
        AudioClip chosenClip = clips[Random.Range(0, clips.Length)];
        AudioSource.PlayClipAtPoint(chosenClip, position, _optionsManager.SFXVolume * volumeModifier);
    }
}
