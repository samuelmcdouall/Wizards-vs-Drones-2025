using UnityEngine;

public class WVDSoundManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    AudioSource _SFXAS;
    [SerializeField]
    WVDOptionsManager _optionsManager;

    [Header("Player")]
    public AudioClip PlayerProjectileLaunchSFX1;
    public AudioClip PlayerProjectileLaunchSFX2;
    public AudioClip PlayerProjectileImpactSFX;
    public AudioClip PlayerDashSFX;
    public AudioClip PlayerHitSFX1;
    public AudioClip PlayerHitSFX2;
    public AudioClip PlayerHitSFX3;
    public AudioClip PlayerDeathSFX1;
    public AudioClip PlayerDeathSFX2;
    public AudioClip PlayerDeathSFX3;

    [Header("Power Ups")]
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

    [Header("Battery")]
    public AudioClip PickupBatterySFX;

    [Header("Drones")]
    public AudioClip DroneSpawnSFX;
    public AudioClip DroneZapSFX;
    public AudioClip DroneLaserLauchSFX;
    public AudioClip DroneLaserCollideSFX;
    public AudioClip DroneTeleportSFX;
    public AudioClip DroneTakeDamageSFX1;
    public AudioClip DroneTakeDamageSFX2;

    [Header("Boss")]
    public AudioClip BossBlowUpGateSFX;
    public AudioClip BossEvilShortLaughSFX; // blowing up gate
    public AudioClip BossEvilLongLaughSFX; // healing
    public AudioClip BossSpawnProjectileSFX1;
    public AudioClip BossSpawnProjectileSFX2;
    public AudioClip BossSpawnProjectileSFX3;
    public AudioClip BossProjectileImpactSFX;
    public AudioClip BossSpawnFireElementSFX;
    public AudioClip BossDeathSFX;

    [Header("UI")]
    public AudioClip UIButtonSFX;
    public AudioClip BuyButtonSFX;

    public void PlaySFXAtPlayer(AudioClip clip, float volumeModifier = 1.0f)
    {
        _SFXAS.PlayOneShot(clip, _optionsManager.SFXVolume * volumeModifier);
    }
    public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volumeModifier = 1.0f)
    {
        AudioSource.PlayClipAtPoint(clip, position, _optionsManager.SFXVolume * volumeModifier);
    }
    public void PlayRandomSFXAtPlayer(AudioClip[] clips, float volumeModifier = 1.0f)
    {
        AudioClip chosenClip = clips[Random.Range(0, clips.Length)];
        _SFXAS.PlayOneShot(chosenClip, _optionsManager.SFXVolume * volumeModifier);
    }
    public void PlayRandomSFXAtPoint(AudioClip[] clips, Vector3 position, float volumeModifier = 1.0f) // For completeness but this isn't actually used
    {
        AudioClip chosenClip = clips[Random.Range(0, clips.Length)];
        AudioSource.PlayClipAtPoint(chosenClip, position, _optionsManager.SFXVolume * volumeModifier);
    }
}