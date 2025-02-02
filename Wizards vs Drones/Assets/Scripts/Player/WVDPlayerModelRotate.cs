using UnityEngine;

public class WVDPlayerModelRotate : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    WVDOptionsManager _optionsManagerScript;

    void Update()
    {
        if (WVDFunctionsCheck.PlayerInputsAllowed())
        {
            transform.Rotate(0.0f, Input.GetAxis("Mouse X") * _optionsManagerScript.MouseSensitivity, 0.0f);
        }
    }
}