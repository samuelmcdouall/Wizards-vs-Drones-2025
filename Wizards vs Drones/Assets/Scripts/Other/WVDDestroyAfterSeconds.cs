using UnityEngine;

public class WVDDestroyAfterSeconds : MonoBehaviour
{
    [SerializeField]
    float _lifetime;
    void Start()
    {
        Destroy(gameObject, _lifetime);
    }
}