using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WVDTree : MonoBehaviour
{
    [SerializeField]
    float _burnTime;
    [SerializeField]
    float _lifeTime;
    [SerializeField]
    GameObject _flameFX;
    Material _treeMaterial;
    bool _burning;

    void Start()
    {
        _treeMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void BurnTree(Vector3 flameFXPos)
    {
        Destroy(gameObject, _lifeTime);
        GameObject flame = Instantiate(_flameFX, flameFXPos, Quaternion.identity);
        flame.transform.parent = transform; // parenting it to the tree
        if (!_burning)
        {
            _burning = true;
            float timer = 0.0f;
            Color originalColour = new Color(_treeMaterial.color.r, _treeMaterial.color.g, _treeMaterial.color.b);
            while (timer < _burnTime)
            {
                Color newColour = Color.Lerp(originalColour, Color.black, timer / _burnTime);
                _treeMaterial?.SetColor("_Color", newColour);
                timer += Time.deltaTime;
                await Task.Yield();
            }
            _treeMaterial?.SetColor("_Color", Color.black);
        }
    }

}
