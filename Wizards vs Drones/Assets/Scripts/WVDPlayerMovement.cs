using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPlayerMovement : MonoBehaviour
{
    [Header("Camera Object")]
    [SerializeField]
    Transform _cameraRotationObject;

    [Header("Movement")]
    Rigidbody _playerRB;
    Vector3 _movementInput;
    WVDPlayer _playerScript;
    [SerializeField]
    WVDGroundCheck _groundCheckScript;
    bool _spacePressed;
    readonly float _moveForce = 150.0f;
    readonly float _jumpForce = 10.0f;
    void Start()
    {
        _playerRB = GetComponent<Rigidbody>();
        _playerScript = GetComponent<WVDPlayer>();
        _movementInput = Vector3.zero;
    }

    void Update()
    {
        _movementInput = Vector3.zero;
        Vector3 cameraForwardYIndependent = new Vector3(_cameraRotationObject.forward.x, 0.0f, _cameraRotationObject.forward.z).normalized;
        Vector3 cameraRightYIndependent = new Vector3(_cameraRotationObject.right.x, 0.0f, _cameraRotationObject.right.z).normalized;

        if (Input.GetKey(KeyCode.W))
        {
            _movementInput += cameraForwardYIndependent;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _movementInput -= cameraRightYIndependent;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _movementInput -= cameraForwardYIndependent;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _movementInput += cameraRightYIndependent;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !_spacePressed && _groundCheckScript.IsGrounded)
        {
            _spacePressed = true;
        }
    }

    void FixedUpdate()
    {
        LimitSpeedToMaximum();
        _playerRB.AddForce(_movementInput * _moveForce, ForceMode.Force);
        if (_spacePressed)
        {
            print("Jump!");
            _spacePressed = false;
            _playerRB.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

    }
    void LimitSpeedToMaximum()
    {
        if (_playerRB.velocity.magnitude > _playerScript.MaxSpeed) // todo maybe times modifier
        {
            float originalYSpeed = _playerRB.velocity.y;
            _playerRB.velocity = _playerRB.velocity.normalized * _playerScript.MaxSpeed;
            _playerRB.velocity = new Vector3(_playerRB.velocity.x, originalYSpeed, _playerRB.velocity.z);
        }
    }
}
