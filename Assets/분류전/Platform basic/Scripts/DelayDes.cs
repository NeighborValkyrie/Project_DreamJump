using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDes : MonoBehaviour
{
    [SerializeField] private float delay;
    private void Start()
    {
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
