using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WVDFlammable : MonoBehaviour
{
    [SerializeField]
    float _burnTime;
    [SerializeField]
    float _lifeTime;
    [SerializeField]
    GameObject _flameFX;
    Material _material;
    bool _burning;

    [SerializeField]
    List<GameObject> _otherObjectsToDestroy;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void BurnObject(Vector3 flameFXPos)
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
