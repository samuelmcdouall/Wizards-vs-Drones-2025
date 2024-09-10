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
    void Start()
    {
        _playerRB = GetComponent<Rigidbody>();
        _playerScript = GetComponent<WVDPlayer>();
        _playerRB.maxLinearVelocity = _playerScript.MaxSpeed;
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
    }

    void FixedUpdate()
    {
        _playerRB.AddForce(_movementInput * 50, ForceMode.Acceleration);
    }
}
