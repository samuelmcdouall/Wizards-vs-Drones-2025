using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WVDOptionsManager : MonoBehaviour
{
    public float MusicVolume;
    public float SFXVolume;
    public float MouseSensitivity;

    public Slider MusicSlider;
    public Slider SFXSlider;
    public Slider MouseSlider;

    [SerializeField]
    WVDMainMenuManager _mainMenuManagerScript;

    void Awake()
    {
        MusicVolume = PlayerPrefs.GetFloat(WVDOptionsStrings.MusicVolume, 0.5f);
        MusicSlider.value = MusicVolume;
        _mainMenuManagerScript?.FadeMusicIn();
        SFXVolume = PlayerPrefs.GetFloat(WVDOptionsStrings.SFXVolume, 0.5f);
        SFXSlider.value = SFXVolume;
        MouseSensitivity = PlayerPrefs.GetFloat(WVDOptionsStrings.MouseSensitivity, 5.0f);
        MouseSlider.value = MouseSensitivity;
    }
}
