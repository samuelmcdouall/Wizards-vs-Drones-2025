public struct WVDTutorialDetails
{
    public string TutorialInformation; // What appears on screen
    public bool BeenPlayedBefore; // Whether its been played before or not (gotten from the SaveData class)

    public WVDTutorialDetails(string tip, bool played)
    {
        TutorialInformation = tip;
        BeenPlayedBefore = played;
    }
}