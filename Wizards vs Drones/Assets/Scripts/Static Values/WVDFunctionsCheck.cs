public static class WVDFunctionsCheck
{
    public static bool InShopMenu;
    public static bool InPauseMenu;
    public static bool IsDead;
    public static bool InCutscene;
    public static bool HasWon;
    public static bool InTutorial;
    public static bool WhiteScreenFading;

    public static bool BatteryCollected;

    public static bool PlayerInputsAllowed()
    {
        return !InShopMenu && !InPauseMenu && !IsDead && !InCutscene && !HasWon && !InTutorial;
    }
    public static void SetToDefault()
    {
        InShopMenu = false;
        InPauseMenu = false;
        IsDead = false;
        InCutscene = false;
        HasWon = false;
        InTutorial = false;
        WhiteScreenFading = false;
        BatteryCollected = false;
    }
}