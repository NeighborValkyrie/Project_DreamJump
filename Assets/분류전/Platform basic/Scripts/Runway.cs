using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runway : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private Vector3 moveDir;
    private Material mat;
    private Vector2 _offset;
    
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        mat = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetTextureOffset("_MainTex", _offset);
        
        _offset += new Vector2(speed * Time.deltaTime, 0);
        _offset.x %= 1;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.AddForce(moveDir);
        }
    }
}
