using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDShopInteract : MonoBehaviour
{
    Transform _player;
    [SerializeField]
    GameObject _interactIcon;
    [SerializeField]
    float _interactThreshold;
    [SerializeField]
    GameObject _shopUI;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
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
        if (_shopUI.activeSelf && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
        {
            _shopUI.SetActive(false);
        }
        else if (_interactIcon.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            _shopUI.SetActive(true);
        }
    }
}
