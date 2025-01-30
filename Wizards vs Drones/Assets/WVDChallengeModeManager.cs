using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDChallengeModeManager : MonoBehaviour
{
    public static WVDChallengeModeManager Instance;
    public bool ChallengeModeActive;
    public Difficulty SelectedDifficulty;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}
