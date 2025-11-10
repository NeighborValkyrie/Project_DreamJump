using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    public Transform moonTr;
    private bool _isTrigger = false;

    IEnumerator EndAnim()
    {
        while (moonTr.localScale.x < 1)
        {
            moonTr.localScale += Vector3.one * Time.deltaTime * 0.3f;
            yield return null;
        }
        moonTr.GetComponentInChildren<ParticleSystem>()?.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_isTrigger && other.CompareTag("Player"))
        {
            _isTrigger = true;
            StartCoroutine(EndAnim());
        }
    }
}
