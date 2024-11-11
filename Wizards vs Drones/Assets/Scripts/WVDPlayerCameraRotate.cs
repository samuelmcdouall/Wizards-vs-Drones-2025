using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPlayerCameraRotate : MonoBehaviour
{
    [Header("Rotation")]
    float _mouseX;
    float _mouseY;

    [Header("Vertical Mouse Limits")]
    readonly float _mouseYMinClamp = -35.0f;
    readonly float _mouseYMaxClamp = 60.0f;

    [SerializeField]
    WVDPlayerInputs _playerMovementScript;

    
    void Update()
    {
        if (_playerMovementScript.CurrentPlayerMovementState != WVDPlayerInputs.PlayerMovementState.Dashing)
        {
            if (WVDPlayerInputsAllowed.PlayerInputsAllowed())
            {
                GetMouseInput();
            }
            transform.rotation = Quaternion.Euler(_mouseY, _mouseX, 0.0f);
        }
    }

    void GetMouseInput()
    {
        _mouseX += Input.GetAxis("Mouse X") * 10.0f; // todo put in mouse sensitivity setting here
        _mouseY -= Input.GetAxis("Mouse Y") * 10.0f; // todo put in mouse sensitivity setting here
        _mouseY = Mathf.Clamp(_mouseY, _mouseYMinClamp, _mouseYMaxClamp);
    }
}
