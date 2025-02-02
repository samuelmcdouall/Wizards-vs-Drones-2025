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
    [SerializeField]
    WVDGroundCheck _groundCheckScript;
    CharacterController _playerCC;
    Vector3 _movementInput;
    WVDPlayer _playerScript;
    readonly float _gravity = -190.62f;
    Vector3 _velocity;
    [SerializeField]
    PlayerMovementState _currentPlayerMovementState;

    [Header("Dashing")]
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
    LayerMask _layerMask;

    [Header("Other")]
    WVDSoundManager _soundManager;

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
            if (WVDFunctionsCheck.PlayerInputsAllowed())
            {
                _dashUI.SetActive(_canDash);
            }
        }
    }

    void Start()
    {
        _playerCC = GetComponent<CharacterController>();
        _playerScript = GetComponent<WVDPlayer>();
        _movementInput = Vector3.zero;
        _velocity = Vector3.zero;
        CurrentPlayerMovementState = PlayerMovementState.Still;
        CanDash = true;
        _canAttack = true;
        _layerMask = LayerMask.GetMask("Ignore Raycast");
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<WVDSoundManager>();
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
        if (WVDFunctionsCheck.PlayerInputsAllowed() && _canAttack && Input.GetMouseButton(0))
        {
            int bonusDamage = 0;
            if (_playerScript.CurrentHealth <= _playerScript.MaxHealth / 2)
            {
                bonusDamage = _playerScript.PurchasedUpgrades.LowHealthDamageBonus;
            }

            WVDAttackEffects currentEffects = GetActiveAttackEffects();

            // Determine projectile direction by firing ray from camera forwards, where that hits is where the player fire ball will go (i.e. where the crosshair is)
            RaycastHit hit;
            Vector3 direction = Vector3.zero;
            if (Physics.Raycast(_camera.position, _camera.forward, out hit, 1000.0f, ~_layerMask))
            {
                direction = hit.point - _attackFirePoint.position;
            }
            else
            {
                Debug.LogError("DIDNT FIND AN END TARGET TO HIT");
            }

            // Launch initial projectile
            WVDPlayerProjectile magicMissile = Instantiate(_magicMissilePrefab, _attackFirePoint.position, _playerScript.GetModelTransform().rotation).GetComponent<WVDPlayerProjectile>();
            magicMissile.SetProjectileDirection(direction);
            magicMissile.SetProjectileEffects(currentEffects);
            magicMissile.Damage += bonusDamage;
            magicMissile.PlayerScript = _playerScript;

            _soundManager.PlayRandomSFXAtPlayer(new AudioClip[] { _soundManager.PlayerProjectileLaunchSFX1, _soundManager.PlayerProjectileLaunchSFX2 });

            // Lauch additional projectiles if purchased upgrade
            if (_playerScript.PurchasedUpgrades.ShootThreeArc)
            {
                WVDPlayerProjectile magicMissileLeft = Instantiate(_magicMissilePrefab, _attackFirePoint.position, Quaternion.Euler(0.0f, -15.0f, 0.0f) * _playerScript.GetModelTransform().rotation).GetComponent<WVDPlayerProjectile>();
                magicMissileLeft.SetProjectileDirection(Quaternion.Euler(0.0f,-15.0f,0.0f) * direction);
                magicMissileLeft.SetProjectileEffects(currentEffects);
                magicMissileLeft.Damage += bonusDamage;
                magicMissileLeft.PlayerScript = _playerScript;
                WVDPlayerProjectile magicMissileRight = Instantiate(_magicMissilePrefab, _attackFirePoint.position, Quaternion.Euler(0.0f, 15.0f, 0.0f) * _playerScript.GetModelTransform().rotation).GetComponent<WVDPlayerProjectile>();
                magicMissileRight.SetProjectileDirection(Quaternion.Euler(0.0f, 15.0f, 0.0f) * direction);
                magicMissileRight.SetProjectileEffects(currentEffects);
                magicMissileRight.Damage += bonusDamage;
                magicMissileRight.PlayerScript = _playerScript;
            }

            CurrentPlayerMovementState = PlayerMovementState.Attacking;
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerAttackAnimation);
            _canAttack = false;
            RechargeAttack();
        }

        if (WVDFunctionsCheck.PlayerInputsAllowed())
        {
            HandleMovement();
        }
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
    public async void RechargeAttack()
    {
        float endRechargeTime = Time.time + _attackRechargeInterval / _playerScript.PurchasedUpgrades.AttackSpeedModifier;
        while (Time.time < endRechargeTime)
        {
            await Task.Yield();
        }
        _canAttack = true;
    }
    void HandleMovement()
    {
        // Horizontal movement
        WVDPlayerDirection playerDirection = GetPlayerDirection(); // Need both the input keys being pressed for the directional animation and world direction to move in

        if (playerDirection.InputVector.z == 1) // Forwards
        {
            _playerCC.Move(playerDirection.DirectionVector * _playerScript.MaxNormalSpeed * Time.deltaTime);
        }
        else // Sideways/backwards
        {
            _playerCC.Move(playerDirection.DirectionVector * _playerScript.MaxSideBackSpeed * Time.deltaTime);
        }

        // Play corresponding animation
        PlayMovementAnimation(playerDirection.InputVector);

        if (playerDirection.InputVector == Vector3.zero)
        {
            CurrentPlayerMovementState = PlayerMovementState.Still;
        }
        else if (GivenDashInput())
        {
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
    WVDPlayerDirection GetPlayerDirection()
    {
        Vector3 movement = Vector3.zero;
        Vector3 inputVector = Vector3.zero;
        Vector3 cameraForwardYIndependent = new Vector3(_cameraRotationObject.forward.x, 0.0f, _cameraRotationObject.forward.z).normalized;
        Vector3 cameraRightYIndependent = new Vector3(_cameraRotationObject.right.x, 0.0f, _cameraRotationObject.right.z).normalized;

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

        return new WVDPlayerDirection(movement.normalized, inputVector);
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
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerIdleAnimation, _playerScript.CurrentSpeedModifier);
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
    public async void DashInDirection(Vector3 dashDirection)
    {
        _playerScript.Invulnerable = true;
        _dashFX.SetActive(true);
        _soundManager.PlaySFXAtPlayer(_soundManager.PlayerDashSFX);
        float endDashTime = Time.time + _dashInterval;
        while (Time.time < endDashTime)
        {
            _playerCC.Move(dashDirection * _playerScript.DashSpeed * Time.deltaTime);
            await Task.Yield();
        }
        print("Done dashing");
        RechargeDash();
        _playerScript.PlayerModelOn = true;
        CurrentPlayerMovementState = PlayerMovementState.Moving;
        _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerIdleAnimation); // This is so that key input after the dash will allow the animation to change back to running
        _dashFX.SetActive(false);
        _playerScript.Invulnerable = false;
    }
    public async void RechargeDash()
    {
        float endRechargeTime = Time.time + _dashRechargeInterval / _playerScript.PurchasedUpgrades.DashRechargeModifier;
        while (Time.time < endRechargeTime)
        {
            await Task.Yield();
        }
        print("Done recharging dash");
        CanDash = true;
    }
    void ApplyVerticalMovement()
    {
        ApplyGravity();
        _playerCC.Move(_velocity * Time.deltaTime);
    }
    void ApplyGravity()
    {
        if (_groundCheckScript.IsGrounded && _velocity.y < -8.0f)
        {
            _velocity.y = -8.0f;
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