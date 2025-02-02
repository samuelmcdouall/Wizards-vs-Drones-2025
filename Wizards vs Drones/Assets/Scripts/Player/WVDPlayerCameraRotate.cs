using UnityEngine;

public class WVDPlayerCameraRotate : MonoBehaviour
{
    [Header("Rotation")]
    float _mouseX;
    float _mouseY;

    [Header("Vertical Mouse Limits")]
    readonly float _mouseYMinClamp = -35.0f;
    readonly float _mouseYMaxClamp = 60.0f;

    [Header("Other")]
    [SerializeField]
    WVDPlayerInputs _playerMovementScript;
    [SerializeField]
    WVDOptionsManager _optionsManagerScript;

    void Update()
    {
        if (_playerMovementScript.CurrentPlayerMovementState != WVDPlayerInputs.PlayerMovementState.Dashing)
        {
            if (WVDFunctionsCheck.PlayerInputsAllowed())
            {
                GetMouseInput();
            }
            transform.rotation = Quaternion.Euler(_mouseY, _mouseX, 0.0f);
        }
    }
    void GetMouseInput()
    {
        _mouseX += Input.GetAxis("Mouse X") * _optionsManagerScript.MouseSensitivity;
        _mouseY -= Input.GetAxis("Mouse Y") * _optionsManagerScript.MouseSensitivity;
        _mouseY = Mathf.Clamp(_mouseY, _mouseYMinClamp, _mouseYMaxClamp);
    }
}