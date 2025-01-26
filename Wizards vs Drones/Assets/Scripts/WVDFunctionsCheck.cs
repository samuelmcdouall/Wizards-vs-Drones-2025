public static class WVDFunctionsCheck
{
    public static bool InShopMenu;
    public static bool InPauseMenu;
    public static bool IsDead;
    public static bool InCutscene;
    public static bool HasWon;

    public static bool PlayerInputsAllowed()
    {
        return !InShopMenu && !InPauseMenu && !IsDead && !InCutscene && !HasWon;
    }
    public static void SetToDefault()
    {
        InShopMenu = false;
        InPauseMenu = false;
        IsDead = false;
        InCutscene = false;
        HasWon = false;
    }
}
