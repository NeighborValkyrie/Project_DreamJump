using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archivepoint : MonoBehaviour
{
    public Color startCol;
    public Color triggerCol;
    
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.color = startCol;
    }

    private void Update()
    {
        transform.Rotate(0,0,100 * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Archivepoint [] archivepoints = FindObjectsOfType<Archivepoint>();
            foreach (Archivepoint archivepoint in archivepoints)
                archivepoint.mat.color = startCol;
            
            mat.color = triggerCol;
            other.GetComponent<PlayerControl>()?.SetSavePos(transform.position);
        }
    }
}
