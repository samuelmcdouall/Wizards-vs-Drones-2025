using UnityEngine;

public class WVDGroundCheck : MonoBehaviour
{
    [Header("General")]
    public bool IsGrounded;

    void OnTriggerStay(Collider other)
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