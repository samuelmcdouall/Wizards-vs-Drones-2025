public static class WVDPlayerInputsAllowed
{
    public static bool InShopMenu;
    public static bool InPauseMenu;
    public static bool IsDead;

    public static bool PlayerInputsAllowed()
    {
        return !InShopMenu && !InPauseMenu && !IsDead;
    }
}
