using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPowerUp : MonoBehaviour
{
    [SerializeField]
    WVDPlayerPowerUpManager.PowerUpType _selectedPowerUpType;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_selectedPowerUpType == WVDPlayerPowerUpManager.PowerUpType.Upgrade)
            {
                other.gameObject.GetComponent<WVDPlayerPowerUpManager>().SecondaryPowerUpCountHeld++;
            }
            else
            {
                other.gameObject.GetComponent<WVDPlayerPowerUpManager>().PrimaryPowerUpHeld = _selectedPowerUpType;
            }
            Destroy(gameObject);
        }
    }
}