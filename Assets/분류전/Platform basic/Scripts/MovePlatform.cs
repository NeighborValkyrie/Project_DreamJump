using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag.Equals("Player"))
        {
            other.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.transform.tag.Equals("Player"))
        {
            other.transform.SetParent(null);
        }
    }

}
