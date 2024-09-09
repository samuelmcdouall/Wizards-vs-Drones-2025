using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPlayerMovement : MonoBehaviour
{

    [SerializeField]
    Transform _cameraRotationObject;
    Rigidbody _playerRB;
    Vector3 movementInput;
    WVDPlayer _playerScript;
    void Start()
    {
        _playerRB = GetComponent<Rigidbody>();
        _playerScript = GetComponent<WVDPlayer>();
        _playerRB.maxLinearVelocity = _playerScript.MaxSpeed;
        movementInput = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = Vector3.zero;
        Vector3 cameraForwardYIndependent = new Vector3(_cameraRotationObject.forward.x, 0.0f, _cameraRotationObject.forward.z).normalized;
        Vector3 cameraRightYIndependent = new Vector3(_cameraRotationObject.right.x, 0.0f, _cameraRotationObject.right.z).normalized;

        if (Input.GetKey(KeyCode.W))
        {
            movementInput += cameraForwardYIndependent;
        }
    }

    void FixedUpdate()
    {
        _playerRB.AddForce(movementInput * 50, ForceMode.Acceleration);
    }
}
