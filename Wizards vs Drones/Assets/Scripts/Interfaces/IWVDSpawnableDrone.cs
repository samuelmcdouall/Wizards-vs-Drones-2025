using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWVDSpawnableDrone // todo maybe not needed
{
    public WVDDroneSpawner DroneSpawner { get; set; }
    void SetSpawnerParameters(WVDDroneSpawner spawner);
}
