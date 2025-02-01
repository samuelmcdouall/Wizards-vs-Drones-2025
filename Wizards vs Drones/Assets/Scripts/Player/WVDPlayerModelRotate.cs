using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPlayerModelRotate : MonoBehaviour
{
    [SerializeField]
    WVDOptionsManager _optionsManagerScript;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (WVDFunctionsCheck.PlayerInputsAllowed())
        {
            transform.Rotate(0.0f, Input.GetAxis("Mouse X") * _optionsManagerScript.MouseSensitivity, 0.0f); // todo put in mouse sensitivity
        }
    }
}
