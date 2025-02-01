using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WVDDroneRemainingHelpUI : MonoBehaviour
{
    Image _helpUI;
    Camera _camera;
    Transform _droneTransform;
    Transform _playerTransform;
    [SerializeField]
    float _showUIThreshold;
    Vector3 _dronePositionOffset = new Vector3(0.0f, 2.0f, 0.0f);
    void Start()
    {
        _helpUI = GetComponent<Image>();
        _camera = Camera.main;
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!_droneTransform)
        {
            Destroy(gameObject);
        }
        else
        {
            if (Vector3.Distance(_droneTransform.position, _playerTransform.position) < _showUIThreshold)
            {
                _helpUI.enabled = false;
            }
            else
            {
                _helpUI.enabled = true;
            }
            Vector3 screenPos = _camera.WorldToScreenPoint(_droneTransform.position + _dronePositionOffset);
            if (screenPos.z < 0)
            {
                screenPos.x = -10000.0f; // i.e. don't show, place way off screen
            }
            _helpUI.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        }

    }

    public void SetDroneTransform(Transform droneTransform)
    {
        _droneTransform = droneTransform;
    }
}
