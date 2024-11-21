using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDGreatHallTrap : MonoBehaviour // this could probably be in a base class, complete this first then decide what needs to be in the base class
{
    [SerializeField]
    Transform _player;
    [SerializeField]
    GameObject _interactIcon;
    [SerializeField]
    float _interactThreshold;
    [SerializeField]
    float _trapCooldown;

    void Start()
    {

    }

    // Update is called once per frame
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
        if (_interactIcon.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            // do trap
        }
    }
}
