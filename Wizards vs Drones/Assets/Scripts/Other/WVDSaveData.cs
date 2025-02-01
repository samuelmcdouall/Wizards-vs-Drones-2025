using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDSaveData
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
    public bool ChallengeModeUnlocked;
    public bool BatteryPlayedBefore;
    public bool NewAreasPlayedBefore;

    public void ResetTutorialTips()
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
    }

    public WVDSaveData() { }

    public WVDSaveData(bool challengeModeUnlocked) // For resetting tutorial tips
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
