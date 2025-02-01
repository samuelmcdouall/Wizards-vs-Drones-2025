using UnityEngine;

public class WVDStatsManager : MonoBehaviour
{
    [Header("Drones")]
    public int ElectricDronesDestroyed;
    public int LaserDronesDestroyed;
    public int FastDronesDestroyed;
    public int TeleportDronesDestroyed;

    [Header("Batteries")]
    public int BatteriesCollected;

    [Header("Timer")]
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

    void Update()
    {
        if (!TimerStopped)
        {
            TimeTaken += Time.deltaTime;
        }
    }
}