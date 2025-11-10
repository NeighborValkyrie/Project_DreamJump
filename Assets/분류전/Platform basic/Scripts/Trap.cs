using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private bool isFatal;
    [SerializeField] private float impact;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (isFatal)
            {
                other.GetComponent<PlayerControl>()?.GameOver();
            }
            else
            {
                Vector3 hitPos = other.bounds.ClosestPoint(transform.position);
                other.GetComponent<Rigidbody>().AddForce((other.transform.position + Vector3.up * 3 - hitPos).normalized * impact, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag.Equals("Player"))
        {
            if (isFatal)
            {
                other.transform.GetComponent<PlayerControl>()?.GameOver();
            }
            else
            {
                Vector3 hitPos = other.contacts[0].point;
                other.transform.GetComponent<Rigidbody>().AddForce((other.transform.position + Vector3.up * 3 - hitPos).normalized * impact, ForceMode.Impulse);
            }
        }
    }
}
