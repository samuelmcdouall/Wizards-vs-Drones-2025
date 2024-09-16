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
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Horizontal movement
        Vector3 movementInput = GetHorizontalInput();

        _playerCC.Move(movementInput * _playerScript.MaxSpeed * Time.deltaTime);

        // Vertical movement
        HandleJumping();

        _playerCC.Move(_velocity * Time.deltaTime);
    }

    void HandleJumping()
    {
        if (_groundCheckScript.IsGrounded && _velocity.y < -2.0f)
        {
            _velocity.y = -2.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _groundCheckScript.IsGrounded)
        {
            _velocity.y = _initialJumpVelocity;
            print("Jump!");
        }

        _velocity.y += _gravity * Time.deltaTime;
    }

    Vector3 GetHorizontalInput()
    {
        Vector3 movementInput = Vector3.zero;
        Vector3 cameraForwardYIndependent = new Vector3(_cameraRotationObject.forward.x, 0.0f, _cameraRotationObject.forward.z).normalized;
        Vector3 cameraRightYIndependent = new Vector3(_cameraRotationObject.right.x, 0.0f, _cameraRotationObject.right.z).normalized;

        // try an input vector for the movement animation next
        bool isIdle = true;
        if (Input.GetKey(KeyCode.W))
        {
            movementInput += cameraForwardYIndependent;
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerForwardAnimation);
            isIdle = false;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementInput -= cameraRightYIndependent;
            isIdle = false;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementInput -= cameraForwardYIndependent;
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerBackwardAnimation);
            isIdle = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementInput += cameraRightYIndependent;
            isIdle = false;
        }
        if (isIdle)
        {
            _playerScript.SwitchToAnimation(WVDAnimationStrings.PlayerIdleAnimation);
        }
        return movementInput;
    }
}