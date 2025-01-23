using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WVDBaseDrone;
using UnityEngine.AI;
using System.Threading.Tasks;
using Unity.VisualScripting;

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

    [Header("Dungeon Escape - Boss")]
    [SerializeField]
    Transform _dungeonEscapeWayPoint1;
    [SerializeField]
    Transform _dungeonEscapeWayPoint2;
    [SerializeField]
    float _waitAtDoorDelay;
    [SerializeField]
    float _destroyingDoorDelay1; // before the explosion
    [SerializeField]
    float _destroyingDoorDelay2; // after the explosion, before the animation ends
    [SerializeField]
    float _waitBeforeEscapingDelay;
    [SerializeField]
    GameObject _door;
    [SerializeField]
    GameObject _doorExplosionFX;
    [SerializeField]
    WVDBossCutsceneManager _bossCutsceneManagerScript;

    public BossState CurrentBossState 
    { 
        get => _currentBossState; 
        set => _currentBossState = value; 
    }

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
        switch (_currentBossState)
        {
            case BossState.DungeonIdle:
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

                    break;
                }

            case BossState.DungeonFlying:
                if (Vector3.Distance(transform.position, _chosenWayPoint.position) < _wayPointThreshold)
                {
                    SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                    _currentBossState = BossState.DungeonIdle;
                }
                else
                {
                    transform.position += _movementVector * MaxNormalSpeed * Time.deltaTime;
                }
                break;
            case BossState.DungeonFlyToDoor:
                if (Vector3.Distance(transform.position, _dungeonEscapeWayPoint1.position) < _wayPointThreshold)
                {
                    SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                    TransitionToStateAfterDelay(BossState.DungeonBreakingDoorPart1, _waitAtDoorDelay);
                }
                else
                {
                    transform.LookAt(_dungeonEscapeWayPoint1);
                    transform.position += _movementVector * MaxNormalSpeed * Time.deltaTime;
                }
                break;
            case BossState.DungeonBreakingDoorPart1:
                transform.LookAt(_dungeonEscapeWayPoint2);
                _movementVector = (_dungeonEscapeWayPoint2.position - _dungeonEscapeWayPoint1.position).normalized;
                SwitchToAnimation(WVDAnimationStrings.BossFireballAttackAnimation);
                TransitionToStateAfterDelay(BossState.DungeonDoorExplodes, _destroyingDoorDelay1);
                break;
            case BossState.DungeonDoorExplodes:
                _door.SetActive(false);
                Instantiate(_doorExplosionFX, _door.transform.position, Quaternion.identity);
                TransitionToStateAfterDelay(BossState.DungeonBreakingDoorPart2, _destroyingDoorDelay2);
                break;
            case BossState.DungeonBreakingDoorPart2:
                SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                TransitionToStateAfterDelay(BossState.DungeonEscaping, _waitBeforeEscapingDelay);
                break;
            case BossState.DungeonEscaping:
                if (Vector3.Distance(transform.position, _dungeonEscapeWayPoint2.position) < _wayPointThreshold)
                {
                    SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                    _currentBossState = BossState.Idle;
                    _bossCutsceneManagerScript.EndBossCutscene();
                }
                else
                {
                    SwitchToAnimation(WVDAnimationStrings.BossFlyingAnimation);
                    transform.position += _movementVector * MaxNormalSpeed * Time.deltaTime;
                }
                break;
            case BossState.Transitional:
                // Don't do anything, will move to another state shortly
                break;
            case BossState.Idle:
                break;

        }


    }

    public void CalculateMovementVectorToDoor()
    {
        _movementVector = (_dungeonEscapeWayPoint1.position - transform.position).normalized;
    }

    async void TransitionToStateAfterDelay(BossState nextState, float delay)
    {
        _currentBossState = BossState.Transitional;
        float endTime = Time.time + delay;
        while (Time.time < endTime)
        {
            await Task.Yield();
        }
        _currentBossState = nextState;
    }


    public enum BossState
    {
        // Whilst in dungeon, i.e. when the game is going on before the boss level
        DungeonIdle,
        DungeonFlying,

        // Escaping cutscene
        DungeonFlyToDoor,
        DungeonBreakingDoorPart1,
        DungeonBreakingDoorPart2,
        DungeonDoorExplodes,
        DungeonEscaping,

        // During combat
        Idle,
        FireballAttacking, // fireballs can destroy trees (they burn for a moment, turning black, before vanishing)
        Flamestrike,
        Healing,

        Transitional // will move to another state after a small delay
    }
}
