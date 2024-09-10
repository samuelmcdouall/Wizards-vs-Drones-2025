using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDGroundCheck : MonoBehaviour
{
    public bool IsGrounded;

    private void OnTriggerStay(Collider other)
    {
        if (other != null) 
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IsGrounded = false;
    }

}
