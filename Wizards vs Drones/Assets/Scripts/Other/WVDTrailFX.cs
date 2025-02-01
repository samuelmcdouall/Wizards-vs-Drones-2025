using UnityEngine;

public class WVDTrailFX : MonoBehaviour
{
    [SerializeField]
    float _lifetime;
    [SerializeField]
    ParticleSystem _ps;
    void OnEnable() // Once set to active, set to inactive after a time so its ready to be taken from the pool to be used again in another position
    {
        _ps.time = 0.0f;
        Invoke("DisableObject", _lifetime);
    }
    void DisableObject()
    {
        gameObject.SetActive(false);
    }
}