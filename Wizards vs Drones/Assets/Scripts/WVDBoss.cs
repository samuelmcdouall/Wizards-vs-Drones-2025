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
    [SerializeField]
    GameObject _player;
    BossState _currentBossState;

    [Header("Movement - Boss")]
    Vector3 _movementVector;

    [Header("Dungeon Idle")]
    [SerializeField]
    List<Transform> _dungeonIdleWayPoints;
    Transform _chosenDungeonWayPoint;
    [SerializeField]
    float _wayPointThreshold;
    [SerializeField]
    float _minDungeonIdleTime;
    [SerializeField]
    float _maxDungeonIdleTime;
    float _dungeonIdleTimer;

    [Header("Dungeon Escape")]
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

    [Header("Battle - General")]
    [SerializeField]
    float _minCombatIdleTime;
    [SerializeField]
    float _maxCombatIdleTime;
    float _combatIdleTimer;
    [SerializeField]
    List<Transform> _battleWayPoints;
    Transform _chosenBattleWayPoint;
    BossFightStage _currentBossFightStage;

    [Header("Battle - Fireball Attack")]
    [SerializeField]
    GameObject _fireballAttackPrefab;
    [SerializeField]
    float _fireballInBetweenAttacksDelay;
    [SerializeField]
    float _fireballPreLaunchDelay;
    [SerializeField]
    float _fireballTotalLaunchInterval;
    [SerializeField]
    float _fireballPostLaunchDelay;
    [SerializeField]
    int _fireballAttacksNumberStageOne;
    [SerializeField]
    int _fireballAttacksNumberStageTwo;
    [SerializeField]
    int _fireballAttacksNumberStageThree;
    [SerializeField]
    int _fireballsInArcNumberStageOne;
    [SerializeField]
    int _fireballsInArcNumberStageTwo;
    [SerializeField]
    int _fireballsInArcNumberStageThree;
    int _currentFireballAttackNumber;
    [SerializeField]
    float _fireballLaunchArc;
    [SerializeField]
    Transform _middleFireballFirePoint;
    BossFireballAttackState _currentBossFireballAttackState;

    [Header("Battle - Fire Stream Attack")]
    [SerializeField]
    GameObject _fireStreamElementPrefab;
    [SerializeField]
    float _fireStreamInBetweenAttacksDelay;
    [SerializeField]
    float _fireStreamPreLaunchDelay;
    [SerializeField]
    float _fireStreamPostLaunchDelay;
    [SerializeField]
    int _fireStreamsNumberStageOne;
    [SerializeField]
    int _fireStreamsNumberStageTwo;
    [SerializeField]
    int _fireStreamsNumberStageThree;
    int _currentFireStreamAttackNumber;
    [SerializeField]
    float _fireStreamElementIntervalStageOne; // time interval, not distance
    [SerializeField]
    float _fireStreamElementIntervalStageTwo;
    [SerializeField]
    float _fireStreamElementIntervalStageThree;
    BossFireStreamAttackState _currentBossFireStreamAttackState;
    // NEXT how many streams, interval between successive elements spawning, then other pieces as from above + each stream will be given a direction and a time interval and a number (0). Dist + num for each stream is same


    public BossState CurrentBossState 
    { 
        get => _currentBossState; 
        set => _currentBossState = value; 
    }

    public override void Start()
    {
        base.Start();
        _currentBossState = BossState.DungeonIdle;
        _currentBossFightStage = BossFightStage.StageOne;
        _currentBossFireballAttackState = BossFireballAttackState.BetweenAttacks;
        _currentBossFireStreamAttackState = BossFireStreamAttackState.BetweenAttacks;
        _dungeonIdleTimer = Random.Range(_minDungeonIdleTime, _maxDungeonIdleTime);
        _combatIdleTimer = Random.Range(_minCombatIdleTime, _maxCombatIdleTime);
        _chosenDungeonWayPoint = _dungeonIdleWayPoints[Random.Range(0, _dungeonIdleWayPoints.Count)];
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
                        while (initialChosenWayPoint == _chosenDungeonWayPoint)
                        {
                            initialChosenWayPoint = _dungeonIdleWayPoints[Random.Range(0, _dungeonIdleWayPoints.Count)]; // making sure we don't choose the same one
                        }
                        _chosenDungeonWayPoint = initialChosenWayPoint;
                        _movementVector = (_chosenDungeonWayPoint.position - transform.position).normalized;
                        transform.LookAt(_chosenDungeonWayPoint);
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
                if (Vector3.Distance(transform.position, _chosenDungeonWayPoint.position) < _wayPointThreshold)
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
                Instantiate(_doorExplosionFX, _door.transform.position + new Vector3(0.0f, -2.5f, 0.0f), Quaternion.identity);
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
                if (_combatIdleTimer < 0.0f)
                {
                    _combatIdleTimer = Random.Range(_minCombatIdleTime, _maxCombatIdleTime);

                    _currentBossState = BossState.FireStreamAttack;
                    _currentFireStreamAttackNumber = 0;
                    //_currentBossState = BossState.FireballAttack;
                    //_currentFireballAttackNumber = 0;
                    //int indexOfCurrentWayPoint = _battleWayPoints.IndexOf(_chosenBattleWayPoint);
                    //int chosenIndex;
                    //float rand = Random.Range(0.0f, 1.0f);
                    //if (rand < 0.5f)
                    //{
                    //    chosenIndex = indexOfCurrentWayPoint + 1;
                    //}
                    //else
                    //{
                    //    chosenIndex = indexOfCurrentWayPoint - 1;
                    //}
                    //if (chosenIndex == -1)
                    //{
                    //    chosenIndex = _battleWayPoints.Count - 1;
                    //}
                    //else if (chosenIndex == _battleWayPoints.Count)
                    //{
                    //    chosenIndex = 0;
                    //}
                    //_chosenBattleWayPoint = _battleWayPoints[chosenIndex];
                    //_movementVector = (_chosenBattleWayPoint.position - transform.position).normalized;
                }
                else
                {
                    LookAtPlayer();
                    _combatIdleTimer -= Time.deltaTime;
                }
                break;

            case BossState.FireballAttack:

                if (Vector3.Distance(transform.position, _chosenBattleWayPoint.position) < _wayPointThreshold)
                {
                    SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                    _currentBossState = BossState.Idle;
                    _currentBossFireballAttackState = BossFireballAttackState.BetweenAttacks;
                }
                else
                {
                    transform.position += _movementVector * MaxNormalSpeed * Time.deltaTime;
                    LookAtPlayer();
                    switch (_currentBossFireballAttackState)
                    {
                        case BossFireballAttackState.BetweenAttacks:
                            {
                                SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                                int currentFireballsNumber = _fireballAttacksNumberStageOne;
                                if (_currentBossFightStage == BossFightStage.StageTwo)
                                {
                                    currentFireballsNumber = _fireballAttacksNumberStageTwo;
                                }
                                else if (_currentBossFightStage == BossFightStage.StageThree)
                                {
                                    currentFireballsNumber = _fireballAttacksNumberStageThree;
                                }
                                if (_currentFireballAttackNumber < currentFireballsNumber)
                                {
                                    _currentFireballAttackNumber++;
                                    TransitionToStateAfterDelay(BossFireballAttackState.ChargingUpFireball, _fireballInBetweenAttacksDelay);
                                }
                                else
                                {
                                    // remain in BetweenAttacks state, will do so until reached waypoint
                                }

                                break;
                            }

                        case BossFireballAttackState.ChargingUpFireball:
                            SwitchToAnimation(WVDAnimationStrings.BossFireballAttackAnimation);
                            TransitionToStateAfterDelay(BossFireballAttackState.LaunchingFireball, _fireballPreLaunchDelay);
                            break;
                        case BossFireballAttackState.LaunchingFireball:
                            {
                                int currentFireballsInArc = _fireballsInArcNumberStageOne;
                                if (_currentBossFightStage == BossFightStage.StageTwo)
                                {
                                    currentFireballsInArc = _fireballsInArcNumberStageTwo;
                                }
                                else if (_currentBossFightStage == BossFightStage.StageThree)
                                {
                                    currentFireballsInArc = _fireballsInArcNumberStageThree;
                                }
                                LaunchFireballsInArc(currentFireballsInArc);
                                TransitionToStateAfterDelay(BossFireballAttackState.BetweenAttacks, _fireballPostLaunchDelay);
                                break;
                            }
                    }
                }
                break;
            case BossState.FireStreamAttack:
                //if (Vector3.Distance(transform.position, _chosenBattleWayPoint.position) < _wayPointThreshold) // keeping this here for the moment in case decide need to move
                //{
                //    SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                //    _currentBossState = BossState.Idle;
                //    _currentBossFireballAttackState = BossFireballAttackState.BetweenAttacks;
                //}
                //else
                //{
                    //transform.position += _movementVector * MaxNormalSpeed * Time.deltaTime;
                    //LookAtPlayer();
                    switch (_currentBossFireStreamAttackState)
                    {
                        case BossFireStreamAttackState.BetweenAttacks:
                            {
                                SwitchToAnimation(WVDAnimationStrings.BossIdleAnimation);
                                int currentFireStreamsNumber = _fireStreamsNumberStageOne;
                                if (_currentBossFightStage == BossFightStage.StageTwo)
                                {
                                    currentFireStreamsNumber = _fireStreamsNumberStageTwo;
                                }
                                else if (_currentBossFightStage == BossFightStage.StageThree)
                                {
                                    currentFireStreamsNumber = _fireStreamsNumberStageThree;
                                }
                                if (_currentFireStreamAttackNumber < currentFireStreamsNumber)
                                {
                                    _currentFireStreamAttackNumber++;
                                    TransitionToStateAfterDelay(BossFireStreamAttackState.ChargingUpFireStream, _fireStreamInBetweenAttacksDelay);
                                }
                                else
                                {
                                    _currentBossState = BossState.Idle;
                                }

                                break;
                            }

                        case BossFireStreamAttackState.ChargingUpFireStream:
                            SwitchToAnimation(WVDAnimationStrings.BossTridentAttackAnimation);
                            TransitionToStateAfterDelay(BossFireStreamAttackState.LaunchingFireStream, _fireStreamPreLaunchDelay);
                            break;
                        case BossFireStreamAttackState.LaunchingFireStream:
                            {
                                float currentFireStreamElementInterval = _fireStreamElementIntervalStageOne;
                                if (_currentBossFightStage == BossFightStage.StageTwo)
                                {
                                    currentFireStreamElementInterval = _fireStreamElementIntervalStageTwo;
                                }
                                else if (_currentBossFightStage == BossFightStage.StageThree)
                                {
                                    currentFireStreamElementInterval = _fireStreamElementIntervalStageThree;
                                }
                                Vector3 directionToPlayer = (new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z) - transform.position).normalized;
                                WVDBossFireStreamElement fireStreamElement = Instantiate(_fireStreamElementPrefab, new Vector3(transform.position.x, 0.0f, transform.position.z) + directionToPlayer * 0.5f, Quaternion.identity).GetComponent<WVDBossFireStreamElement>();
                                fireStreamElement.SetParameters(directionToPlayer, currentFireStreamElementInterval, 0);
                                TransitionToStateAfterDelay(BossFireStreamAttackState.BetweenAttacks, _fireStreamPostLaunchDelay);
                                break;
                            }
                    }
                //}
                break;

        }


    }

    private void LookAtPlayer()
    {
        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
    }

    public void CalculateMovementVectorToDoor()
    {
        _movementVector = (_dungeonEscapeWayPoint1.position - transform.position).normalized;
    }
    public void SetCurrentBattleWayPoint(Transform wayPoint)
    {
        _chosenBattleWayPoint = wayPoint;
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
    async void TransitionToStateAfterDelay(BossFireballAttackState nextState, float delay)
    {
        _currentBossFireballAttackState = BossFireballAttackState.Transitional;
        float endTime = Time.time + delay;
        while (Time.time < endTime)
        {
            await Task.Yield();
        }
        _currentBossFireballAttackState = nextState;
    }
    async void TransitionToStateAfterDelay(BossFireStreamAttackState nextState, float delay)
    {
        _currentBossFireStreamAttackState = BossFireStreamAttackState.Transitional;
        float endTime = Time.time + delay;
        while (Time.time < endTime)
        {
            await Task.Yield();
        }
        _currentBossFireStreamAttackState = nextState;
    }

    async void LaunchFireballsInArc(int numFireballs)
    {
        float timeIncrement = _fireballTotalLaunchInterval / numFireballs;
        float arcIncrement = _fireballLaunchArc / (numFireballs - 1);
        float currentAngle = -(_fireballLaunchArc / 2.0f);

        Vector3 middleFireballDirection = (new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z) - _middleFireballFirePoint.position).normalized; 
        for (int i = 0; i < numFireballs; i++)
        {
            WVDBossProjectile fireball = Instantiate(_fireballAttackPrefab, _middleFireballFirePoint.position, Quaternion.Euler(0.0f, currentAngle, 0.0f) * transform.rotation).GetComponent<WVDBossProjectile>();
            fireball.SetProjectileDirection(Quaternion.Euler(0.0f, currentAngle, 0.0f) * middleFireballDirection);
            fireball.SetProjectileEffects(new WVDAttackEffects());

            currentAngle += arcIncrement;
            float endTime = Time.time + timeIncrement;
            while (Time.time < endTime)
            {
                await Task.Yield();
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
        DungeonBreakingDoorPart1,
        DungeonBreakingDoorPart2,
        DungeonDoorExplodes,
        DungeonEscaping,

        // During combat
        Idle,
        FireballAttack, // fireballs can destroy trees (they burn for a moment, turning black, before vanishing)
        FireStreamAttack,
        Healing,

        Transitional // will move to another state after a small delay
    }

    public enum BossFireballAttackState
    {
        BetweenAttacks,
        ChargingUpFireball,
        LaunchingFireball,
        WindingDownFireball, // Rest of animation, might not need todo

        Transitional
    }
    public enum BossFireStreamAttackState
    {
        BetweenAttacks,
        ChargingUpFireStream,
        LaunchingFireStream,

        Transitional
    }

    public enum BossFightStage
    {
        StageOne,
        StageTwo,
        StageThree
    }
}
