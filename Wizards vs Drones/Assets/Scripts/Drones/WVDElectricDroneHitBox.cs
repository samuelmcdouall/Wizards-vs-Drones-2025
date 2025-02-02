using UnityEngine;

public class WVDElectricDroneHitBox : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    WVDElectricDrone _droneScript;

    [Header("Damage")]
    public bool CanDamage; // Can only hit once per turn of being switched on
    
    void OnTriggerEnter(Collider other)
    {
        if (CanDamage && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<WVDPlayer>().TakeDamage(_droneScript.ZapDamage, true);
            CanDamage = false;
        }
    }
}