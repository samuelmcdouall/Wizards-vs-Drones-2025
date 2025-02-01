using UnityEngine;

public class WVDDifficultySettingsManager : MonoBehaviour
{
    public static WVDDifficultySettingsManager Instance;
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