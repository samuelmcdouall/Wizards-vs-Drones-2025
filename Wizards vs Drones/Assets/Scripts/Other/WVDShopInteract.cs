using UnityEngine;

public class WVDShopInteract : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField]
    Transform _player;
    [SerializeField]
    GameObject _interactIcon;
    [SerializeField]
    float _interactThreshold;
    [SerializeField]
    GameObject _shopUI;

    void Update()
    {
        if (Vector3.Distance(transform.position, _player.position) <= _interactThreshold)
        {
            _interactIcon.SetActive(true);
        }
        else
        {
            _interactIcon.SetActive(false);
        }

        // If in shop, exit shop
        if (_shopUI.activeSelf && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
        {
            _shopUI.SetActive(false);
        }
        // If nearby, enter shop
        else if (_interactIcon.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            _shopUI.SetActive(true);
        }
    }
}