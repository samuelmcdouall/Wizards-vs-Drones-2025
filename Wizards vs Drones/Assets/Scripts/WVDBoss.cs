using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WVDBaseDrone;
using UnityEngine.AI;

public class WVDBoss : WVDBaseEntity
{
    BossState _currentBossState;
    NavMeshAgent BossNMA;

    public override void Start()
    {
        base.Start();
        _currentBossState = BossState.DungeonIdle;
        BossNMA = GetComponent<NavMeshAgent>();
    }


    public enum BossState
    {
        // Whilst in dungeon, i.e. when the game is going on before the boss level
        DungeonIdle,
        DungeonFlying,

        // Escaping cutscene
        DungeonFlyToDoor,
        DungeonBreakingDoor,
        DungeonEscaping,

        // During combat
        Idle,
        FireballAttacking, // fireballs can destroy trees (they burn for a moment, turning black, before vanishing)
        Flamestrike,
        Healing
    }
}
