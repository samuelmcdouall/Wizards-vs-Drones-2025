[System.Serializable]
public struct WVDDroneSpawnRound
{
    public int MinElectric;
    public int MaxElectric;

    public int MinLaser;
    public int MaxLaser;

    public int MinFast;
    public int MaxFast;

    public int MinTeleport;
    public int MaxTeleport;

    public int MaxDroneLimit;

    // Upgrades
    public float SpawnOnDeathChance; 
    public float ShieldChance;
    public float SlowChance;
}