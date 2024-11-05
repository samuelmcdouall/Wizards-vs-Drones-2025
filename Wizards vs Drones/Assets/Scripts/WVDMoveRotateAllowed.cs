public static class WVDMoveRotateAllowed
{
    public static bool InShopMenu;
    public static bool InPauseMenu;

    public static bool CanMoveAndRotate()
    {
        return !InShopMenu && !InPauseMenu;
    }
}
