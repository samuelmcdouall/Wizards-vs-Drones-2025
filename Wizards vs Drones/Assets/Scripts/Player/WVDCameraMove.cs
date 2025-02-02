using UnityEngine;

public class WVDCameraMove : MonoBehaviour
{
    [Header("Camera Raycasting")]
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
    void LateUpdate()
    {
        RaycastHit hit;
        Vector3 rotatePointToCameraDirection = (_cameraTransform.position - _rotatePointTransform.position).normalized;
        Vector3 potentialCameraStartPoint = _rotatePointTransform.position + rotatePointToCameraDirection * _initialLength;

        // If can fire ray uninterrupted to the camera then position camera at maximum length, if not then position it where the raycast hit
        if (Physics.Raycast(_rotatePointTransform.position, rotatePointToCameraDirection, out hit, _initialLength, ~_mask))
        {
            _cameraTransform.position = _rotatePointTransform.position + rotatePointToCameraDirection * Vector3.Distance(hit.point, _rotatePointTransform.position);
        }
        else
        {
            _cameraTransform.position = _rotatePointTransform.position + rotatePointToCameraDirection * _initialLength;

        }
    }
}