using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDCameraMove : MonoBehaviour
{
    [SerializeField]
    Transform _cameraTransform;
    [SerializeField]
    Transform _rotatePointTransform;
    float _initialLength;
    LayerMask _mask;
    void Start()
    {
        _mask = LayerMask.GetMask("Camera");
        _initialLength = Vector3.Distance(_cameraTransform.position, _rotatePointTransform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        RaycastHit hit;
        Vector3 rotatePointToCameraDirection = (_cameraTransform.position - _rotatePointTransform.position).normalized;
        Vector3 potentialCameraStartPoint = _rotatePointTransform.position + rotatePointToCameraDirection * _initialLength;
        if (Physics.Raycast(_rotatePointTransform.position, rotatePointToCameraDirection, out hit, _initialLength, ~_mask))
        {
            //if (!hit.transform.gameObject.CompareTag("MainCamera"))
            {
                _cameraTransform.position = _rotatePointTransform.position + rotatePointToCameraDirection * Vector3.Distance(hit.point, _rotatePointTransform.position);
            }
            //else
            //{
            //    _cameraTransform.position = _rotatePointTransform.position + rotatePointToCameraDirection * _initialLength;
            //}
        }
        else
        {
            _cameraTransform.position = _rotatePointTransform.position + rotatePointToCameraDirection * _initialLength;

        }
    }
}
