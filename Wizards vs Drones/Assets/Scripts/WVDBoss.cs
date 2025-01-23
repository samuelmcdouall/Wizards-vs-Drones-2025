using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WVDBaseDrone;
using UnityEngine.AI;

public class WVDBoss : WVDBaseEntity
{
    [Header("General - Boss")]
    BossState _currentBossState;

    [Header("Movement - Boss")]
    Vector3 _movementVector;

    [Header("Dungeon Idle - Boss")]
    [SerializeField]
    List<Transform> _dungeonIdleWayPoints;
    Transform _chosenWayPoint;
    [SerializeField]
    float _wayPointThreshold;
    [SerializeField]
    float _minDungeonIdleTime;
    [SerializeField]
    float _maxDungeonIdleTime;
    float _dungeonIdleTimer;



    public override void Start()
    {
        base.Start();
        _currentBossState = BossState.DungeonIdle;
        _dungeonIdleTimer = Random.Range(_minDungeonIdleTime, _maxDungeonIdleTime);
        _chosenWayPoint = _dungeonIdleWayPoints[Random.Range(0, _dungeonIdleWayPoints.Count)];
    }

    public override void Update()
    {
        //base.Update(); todo probably don't need this as boss cannot be slowed/stunned or invul
        if (_currentBossState == BossState.DungeonIdle)
        {
            if (_dungeonIdleTimer < 0.0f)
            {
                _dungeonIdleTimer = Random.Range(_minDungeonIdleTime, _maxDungeonIdleTime);
                Transform initialChosenWayPoint = _dungeonIdleWayPoints[Random.Range(0, _dungeonIdleWayPoints.Count)];
                while (initialChosenWayPoint == _chosenWayPoint)
                {
                    initialChosenWayPoint = _dungeonIdleWayPoints[Random.Range(0, _dungeonIdleWayPoints.Count)]; // making sure we don't choose the same one
                }
                _chosenWayPoint = initialChosenWayPoint;
                _movementVector = (_chosenWayPoint.position - transform.position).normalized;
                transform.LookAt(_chosenWayPoint);
                SwitchToAnimation(WVDAnimationStrings.BossFlyingAnimation);
                _currentBossState = BossState.DungeonFlying;
            }
            else
            {
                _dungeonIdleTimer -= Time.deltaTime;
            }
        }
        else if (_currentBossState == BossState.DungeonFlying)
        {
            if (Vector3.Distance(transform.position, _chosenWayPoint.position) < _wayPointThreshold)
            {
                SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                _currentBossState = BossState.DungeonIdle;
            }
            else
            {
                transform.position += _movementVector * MaxNormalSpeed * Time.deltaTime;
            }
        }

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
