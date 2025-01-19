using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WVDPlayerInputs : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    Transform _cameraRotationObject;
    [SerializeField]
    Transform _camera;

    [Header("Movement")]
    CharacterController _playerCC;
    Vector3 _movementInput;
    WVDPlayer _playerScript;
    [SerializeField]
    WVDGroundCheck _groundCheckScript;
    bool _spacePressed;
    [SerializeField]
    float _jumpHeight;
    readonly float _gravity = -9.81f;
    Vector3 _velocity;
    float _initialJumpVelocity;
    [SerializeField]
    PlayerMovementState _currentPlayerMovementState;
    [SerializeField]
    float _dashInterval;
    [SerializeField]
    float _dashRechargeInterval;
    bool _canDash;
    [SerializeField]
    GameObject _dashUI;
    [SerializeField]
    GameObject _dashFX;

    [Header("Attacking")]
    [SerializeField]
    Transform _attackFirePoint;
    [SerializeField]
    GameObject _magicMissilePrefab;
    bool _canAttack;
    [SerializeField]
    float _attackRechargeInterval;

    public PlayerMovementState CurrentPlayerMovementState 
    { 
        get => _currentPlayerMovementState; 
        set => _currentPlayerMovementState = value; 
    }
    public bool CanDash 
    { 
        get => _canDash;
        set
        {
            _canDash = value;
            _dashUI.SetActive(_canDash);
        }
    }

    void Start()
    {
        _playerCC = GetComponent<CharacterController>();
        _playerScript = GetComponent<WVDPlayer>();
        _movementInput = Vector3.zero;
        _velocity = Vector3.zero;
        _initialJumpVelocity = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
        CurrentPlayerMovementState = PlayerMovementState.Still;
        CanDash = true;
        _canAttack = true;
    }

    void Update()
    {
        switch (CurrentPlayerMovementState)
        {
            case PlayerMovementState.Still:
                HandleMouseAndKeyInputs();
                break;
            case PlayerMovementState.Moving:
                HandleMouseAndKeyInputs();
                break;
            case PlayerMovementState.Dashing:
                // Doing the dash, ignore key inputs
                break;
        }

        // Regardless still need to apply gravity
        ApplyVerticalMovement();
    }

    void HandleMouseAndKeyInputs()
    {
        // If blocking just do that and don't take any movement/attacking input, todo leave this here in case come back to it, but shield would be better as a power up
        if (Input.GetMouseButton(1))
        {
            //_playerScript.ActivateShield = true;
            //_playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerBlockAnimation);
            //CurrentPlayerMovementState = PlayerMovementState.Still;
        }
        else
        {
            if (WVDFunctionsCheck.PlayerInputsAllowed() && _canAttack && Input.GetMouseButtonDown(0)) // todo change back to MouseButton once got cooldown in
            {
                int bonusDamage = 0;
                if (_playerScript.CurrentHealth <= _playerScript.MaxHealth / 2)
                {
                    bonusDamage = _playerScript.PurchasedUpgrades.LowHealthDamageBonus;
                }

                WVDAttackEffects currentEffects = GetActiveAttackEffects();

                // Determine projectile direction that fires from just in front of player but goes in direction desired, i.e. direction camera is looking
                RaycastHit hit;
                Vector3 direction = Vector3.zero;
                if (Physics.Raycast(_camera.position, _camera.forward, out hit, 1000.0f))
                {
                    direction = hit.point - _attackFirePoint.position;
                }
                else
                {
                    Debug.LogError("DIDNT FIND AN END TARGET TO HIT");
                }


                WVDPlayerProjectile magicMissile = Instantiate(_magicMissilePrefab, _attackFirePoint.position, _playerScript.GetModelTransform().rotation).GetComponent<WVDPlayerProjectile>();
                magicMissile.SetProjectileDirection(direction);
                magicMissile.SetProjectileEffects(currentEffects);
                magicMissile.Damage += bonusDamage;
                magicMissile.PlayerScript = _playerScript;

                if (_playerScript.PurchasedUpgrades.ShootThreeArc)
                {
                    WVDPlayerProjectile magicMissileLeft = Instantiate(_magicMissilePrefab, _attackFirePoint.position, Quaternion.Euler(0.0f, -30.0f, 0.0f) * _playerScript.GetModelTransform().rotation).GetComponent<WVDPlayerProjectile>();
                    magicMissileLeft.SetProjectileDirection(Quaternion.Euler(0.0f,-30.0f,0.0f) * direction);
                    magicMissileLeft.SetProjectileEffects(currentEffects);
                    magicMissileLeft.Damage += bonusDamage;
                    magicMissileLeft.PlayerScript = _playerScript;
                    WVDPlayerProjectile magicMissileRight = Instantiate(_magicMissilePrefab, _attackFirePoint.position, Quaternion.Euler(0.0f, 30.0f, 0.0f) * _playerScript.GetModelTransform().rotation).GetComponent<WVDPlayerProjectile>();
                    magicMissileRight.SetProjectileDirection(Quaternion.Euler(0.0f, 30.0f, 0.0f) * direction);
                    magicMissileRight.SetProjectileEffects(currentEffects);
                    magicMissileRight.Damage += bonusDamage;
                    magicMissileRight.PlayerScript = _playerScript;
                }

                CurrentPlayerMovementState = PlayerMovementState.Attacking;
                _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerAttackAnimation);
                _canAttack = false;
                RechargeAttack();
            }
            //_playerScript.ActivateShield = false;
            if (WVDFunctionsCheck.PlayerInputsAllowed())
            {
                HandleMovement();
            }
        }
    }

    void HandleMovement()
    {
        // Horizontal movement
        WVDPlayerDirection playerDirection = GetPlayerDirection();

        if (playerDirection.InputVector.z == 1) // forward
        {
            _playerCC.Move(playerDirection.DirectionVector * _playerScript.MaxNormalSpeed * Time.deltaTime);
        }
        else // sideways/backward todo could possibly just have a modifier for the side/back speed rather than additional variable
        {
            _playerCC.Move(playerDirection.DirectionVector * _playerScript.MaxSideBackSpeed * Time.deltaTime);
        }

        // Vertical movement this is for jumping which may include later but not for now
        //HandleJumpingInput();

        // Play corresponding animation
        PlayMovementAnimation(playerDirection.InputVector);

        if (playerDirection.InputVector == Vector3.zero)
        {
            CurrentPlayerMovementState = PlayerMovementState.Still;
        }
        else if (GivenDashInput())
        {
            //_playerScript.ShieldFXOn = false;
            //_playerScript.ActivateShield = false;
            _playerScript.PlayerModelOn = false;
            CanDash = false;
            CurrentPlayerMovementState = PlayerMovementState.Dashing;
            DashInDirection(playerDirection.DirectionVector);


        }
        else
        {
            CurrentPlayerMovementState = PlayerMovementState.Moving;
        }
    }

    public async void DashInDirection(Vector3 dashDirection)
    {
        _dashFX.SetActive(true);
        float endDashTime = Time.time + _dashInterval;
        while (Time.time < endDashTime)
        {
            _playerCC.Move(dashDirection * _playerScript.DashSpeed * Time.deltaTime);
            await Task.Yield();
        }
        print("done dashing");
        RechargeDash();
        _playerScript.PlayerModelOn = true;
        CurrentPlayerMovementState = PlayerMovementState.Moving;
        _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerIdleAnimation); // This is so that input after the dash allows the animation to change back to running
        _dashFX.SetActive(false);
    }
    public async void RechargeDash()
    {
        float endRechargeTime = Time.time + _dashRechargeInterval / _playerScript.PurchasedUpgrades.DashRechargeModifier;
        while (Time.time < endRechargeTime)
        {
            await Task.Yield();
        }
        print("done recharging dash");
        CanDash = true;
    }
    public async void RechargeAttack()
    {
        float endRechargeTime = Time.time + _attackRechargeInterval / _playerScript.PurchasedUpgrades.AttackSpeedModifier;
        while (Time.time < endRechargeTime)
        {
            await Task.Yield();
        }
        _canAttack = true;
    }

    WVDAttackEffects GetActiveAttackEffects()
    {
        WVDAttackEffects effects = new WVDAttackEffects();
        effects.SetToDefault();
        WVDPlayerUpgrades currentUpgrades = _playerScript.PurchasedUpgrades;
        effects.Stun = currentUpgrades.StunAttacks;
        effects.StunDuration = currentUpgrades.StunAttackDuration;
        effects.DropRateIncrease = currentUpgrades.DropRateIncrease;
        effects.ExplodeOnDeathChance = currentUpgrades.ExplodeOnDeathChance;
        //effects.DamageNearby = currentUpgrades.DamageNearby;
        //effects.DamageNearbyDistance = currentUpgrades.DamageNearbyDistance;
        //effects.DamageNearbyTargetsNumber = currentUpgrades.DamageNearbyTargetsNumber;
        effects.Slow = currentUpgrades.SlowAttacks;
        effects.SlowDuration = currentUpgrades.SlowAttackDuration;
        effects.SlowPercentage = currentUpgrades.SlowAttackPercentage;
        effects.DOT = currentUpgrades.DOTAttacks;
        effects.DOTDamage = currentUpgrades.DOTAttackDamage;
        effects.DOTInterval = currentUpgrades.DOTAttackInterval;
        effects.DOTDuration = currentUpgrades.DOTAttackDuration;
        effects.Pierce = currentUpgrades.Pierce;
        effects.CriticalChance = currentUpgrades.CriticalChance;
        effects.LifeSteal = _playerScript.LifeSteal;
        return effects;
    }

    WVDPlayerDirection GetPlayerDirection()
    {
        Vector3 movement = Vector3.zero;
        Vector3 cameraForwardYIndependent = new Vector3(_cameraRotationObject.forward.x, 0.0f, _cameraRotationObject.forward.z).normalized;
        Vector3 cameraRightYIndependent = new Vector3(_cameraRotationObject.right.x, 0.0f, _cameraRotationObject.right.z).normalized;

        // Separate input vector because movementInput will be dependant on camera rotation
        Vector3 inputVector = Vector3.zero;

        if (WVDFunctionsCheck.PlayerInputsAllowed())
        {
            if (Input.GetKey(KeyCode.W))
            {
                movement += cameraForwardYIndependent;
                inputVector += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                movement -= cameraRightYIndependent;
                inputVector += Vector3.left;
            }
            if (Input.GetKey(KeyCode.S))
            {
                movement -= cameraForwardYIndependent;
                inputVector += Vector3.back;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movement += cameraRightYIndependent;
                inputVector += Vector3.right;
            }
        }

        return new WVDPlayerDirection(movement, inputVector);
    }

    void HandleJumpingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _groundCheckScript.IsGrounded)
        {
            _velocity.y = _initialJumpVelocity;
            print("Jump!");
        }
    }

    bool GivenDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _groundCheckScript.IsGrounded && CanDash)
        {
            print("Dash!");
            return true;
        }
        return false;
    }

    void PlayMovementAnimation(Vector3 inputVector)
    {
        if (inputVector.z == 1)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerForwardAnimation, _playerScript.CurrentSpeedModifier);
        }
        else if (inputVector.z == -1)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerBackwardAnimation, _playerScript.CurrentSpeedModifier);
        }
        else if (inputVector.x == 1)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerRightAnimation, _playerScript.CurrentSpeedModifier);
        }
        else if (inputVector.x == -1)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerLeftAnimation, _playerScript.CurrentSpeedModifier);
        }
        else
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerIdleAnimation);
        }
    }

    void ApplyVerticalMovement()
    {
        ApplyGravity();
        _playerCC.Move(_velocity * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (_groundCheckScript.IsGrounded && _velocity.y < -4.0f)
        {
            _velocity.y = -4.0f;
        }

        _velocity.y += _gravity * Time.deltaTime;
    }

    public enum PlayerMovementState
    {
        Still,
        Moving,
        Dashing,
        Attacking
    }
}