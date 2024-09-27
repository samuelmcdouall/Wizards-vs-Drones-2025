using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPlayerMovement : MonoBehaviour
{
    [Header("Camera Object")]
    [SerializeField]
    Transform _cameraRotationObject;

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
    void Start()
    {
        _playerCC = GetComponent<CharacterController>();
        _playerScript = GetComponent<WVDPlayer>();
        _movementInput = Vector3.zero;
        _velocity = Vector3.zero;
        _initialJumpVelocity = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);
    }

    void Update()
    {
        // If blocking just do that and don't take any movement input
        if (Input.GetMouseButton(1))
        {
            _playerScript.ActivateShield = true;
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerBlockAnimation);
        }
        else
        {
            _playerScript.ActivateShield = false;
            HandleMovement();
        }

        // Regardless still need to apply gravity
        ApplyVerticalMovement();
    }

    void HandleMovement()
    {
        // Horizontal movement
        WVDPlayerDirection playerDirection = GetPlayerDirection();

        if (playerDirection.InputVector.z == 1) // forward
        {
            _playerCC.Move(playerDirection.DirectionVector * _playerScript.MaxNormalSpeed * Time.deltaTime);
        }
        else // sideways/backward
        {
            _playerCC.Move(playerDirection.DirectionVector * _playerScript.MaxSideBackSpeed * Time.deltaTime);
        }

        // Vertical movement
        HandleJumpingInput();

        // Play corresponding animation
        PlayMovementAnimation(playerDirection.InputVector);
    }

    WVDPlayerDirection GetPlayerDirection()
    {
        Vector3 movement = Vector3.zero;
        Vector3 cameraForwardYIndependent = new Vector3(_cameraRotationObject.forward.x, 0.0f, _cameraRotationObject.forward.z).normalized;
        Vector3 cameraRightYIndependent = new Vector3(_cameraRotationObject.right.x, 0.0f, _cameraRotationObject.right.z).normalized;

        // Separate input vector because movementInput will be dependant on camera rotation
        Vector3 inputVector = Vector3.zero;

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

    void PlayMovementAnimation(Vector3 inputVector)
    {
        if (inputVector.z == 1)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerForwardAnimation);
        }
        else if (inputVector.z == -1)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerBackwardAnimation);
        }
        else if (inputVector.x == 1)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerRightAnimation);
        }
        else if (inputVector.x == -1)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerLeftAnimation);
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
        if (_groundCheckScript.IsGrounded && _velocity.y < -2.0f)
        {
            _velocity.y = -2.0f;
        }

        _velocity.y += _gravity * Time.deltaTime;
    }
}