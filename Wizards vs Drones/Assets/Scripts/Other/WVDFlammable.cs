using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WVDFlammable : MonoBehaviour
{
    [Header("Burning")]
    [SerializeField]
    float _burnTime;
    [SerializeField]
    float _lifeTime;
    [SerializeField]
    GameObject _flameFX;
    Material _material;
    bool _burning;

    [Header("Other Objects")]
    [SerializeField]
    List<GameObject> _otherObjectsToDestroy; // E.g. this is a table and all objects on top of the table must be destroyed so they aren't left floating

    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }
    public async void BurnObject(Vector3 flameFXPos) // Slowly turning the object black from its starting colour
    {
        GameObject flame = Instantiate(_flameFX, flameFXPos, Quaternion.identity);
        flame.transform.parent = transform; // parenting it
        if (!_burning)
        {
            Invoke("DestroyAllObjects", _lifeTime);
            _burning = true;
            float timer = 0.0f;
            Color originalColour = new Color(_material.color.r, _material.color.g, _material.color.b);
            while (timer < _burnTime)
            {
                Color newColour = Color.Lerp(originalColour, Color.black, timer / _burnTime);
                _material?.SetColor("_Color", newColour);
                timer += Time.deltaTime;
                await Task.Yield();
            }
            _material?.SetColor("_Color", Color.black);
        }
    }
    void DestroyAllObjects()
    {
        foreach(GameObject obj in _otherObjectsToDestroy)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }
}