using UnityEngine;

public class WVDShieldElectricAttackFX : MonoBehaviour
{
    [Header("FX")]
    [SerializeField]
    GameObject _startPos;
    [SerializeField]
    GameObject _endPos;
    [SerializeField]
    float _lifeTime;

    void Start()
    {
        Destroy(gameObject, _lifeTime);
    }
    public void SetPositions(Vector3 startPos, Vector3 endPos) // Creates a lightning effect between the shield and the drone
    {
        _startPos.transform.position = startPos;
        _endPos.transform.position = endPos;
    }
}