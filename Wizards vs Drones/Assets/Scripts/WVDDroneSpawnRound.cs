using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int MinTank;
    public int MaxTank;

    public int MaxDroneLimit;
}
