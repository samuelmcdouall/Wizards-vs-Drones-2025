public class WVDSaveData // Tutorail tips played and challenge mode unlocked
{
    public bool IntroBeenPlayedBefore;
    public bool ElectricDroneBeenPlayedBefore;
    public bool LaserDroneBeenPlayedBefore;
    public bool FastDroneBeenPlayedBefore;
    public bool TeleportDroneBeenPlayedBefore;
    public bool SpawnOnDeathBuffBeenPlayedBefore;
    public bool ShieldBuffBeenPlayedBefore;
    public bool SlowBuffBeenPlayedBefore;
    public bool ShopBeenPlayedBefore;
    public bool AttackPowerUpBeenPlayedBefore;
    public bool ShieldPowerUpBeenPlayedBefore;
    public bool HealPowerUpBeenPlayedBefore;
    public bool TrapPowerUpBeenPlayedBefore;
    public bool TomeBeenPlayedBefore;
    public bool GreatHallBeenPlayedBefore;
    public bool LibraryBeenPlayedBefore;
    public bool DungeonBeenPlayedBefore;
    public bool BossBeenPlayedBefore;
    public bool BatteryPlayedBefore;
    public bool NewAreasPlayedBefore;
    public bool ChallengeModeUnlocked;

    public WVDSaveData() { }

    public WVDSaveData(bool challengeModeUnlocked) // For resetting tutorial tips, don't want to override whether or not challenge mode has been unlocked
    {
        IntroBeenPlayedBefore = false;
        ElectricDroneBeenPlayedBefore = false;
        LaserDroneBeenPlayedBefore = false;
        FastDroneBeenPlayedBefore = false;
        TeleportDroneBeenPlayedBefore = false;
        SpawnOnDeathBuffBeenPlayedBefore = false;
        ShieldBuffBeenPlayedBefore = false;
        SlowBuffBeenPlayedBefore = false;
        ShopBeenPlayedBefore = false;
        AttackPowerUpBeenPlayedBefore = false;
        ShieldPowerUpBeenPlayedBefore = false;
        HealPowerUpBeenPlayedBefore = false;
        TrapPowerUpBeenPlayedBefore = false;
        TomeBeenPlayedBefore = false;
        GreatHallBeenPlayedBefore = false;
        LibraryBeenPlayedBefore = false;
        DungeonBeenPlayedBefore = false;
        BossBeenPlayedBefore = false;
        BatteryPlayedBefore = false;
        NewAreasPlayedBefore = false;
        ChallengeModeUnlocked = challengeModeUnlocked;
    }
}