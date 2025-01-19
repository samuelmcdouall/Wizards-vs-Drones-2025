using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPlayerModelRotate : MonoBehaviour
{



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (WVDFunctionsCheck.PlayerInputsAllowed())
        {
            transform.Rotate(0.0f, Input.GetAxis("Mouse X") * 10.0f, 0.0f); // todo put in mouse sensitivity
        }
    }
}
