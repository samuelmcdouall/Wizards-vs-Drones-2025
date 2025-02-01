using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDStatsManager : MonoBehaviour
{
    public int ElectricDronesDestroyed;
    public int LaserDronesDestroyed;
    public int FastDronesDestroyed;
    public int TeleportDronesDestroyed;
    public int BatteriesCollected;
    public float TimeTaken;

    public bool TimerStopped;
    void Start()
    {
        ElectricDronesDestroyed = 0;
        LaserDronesDestroyed = 0;
        FastDronesDestroyed = 0;
        TeleportDronesDestroyed = 0;
        BatteriesCollected = 0;
        TimeTaken = 0.0f;
        TimerStopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TimerStopped)
        {
            TimeTaken += Time.deltaTime;
        }
    }
}
