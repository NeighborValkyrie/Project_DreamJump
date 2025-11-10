using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    [SerializeField] private Transform posTr1;
    [SerializeField] private Transform posTr2;
    [SerializeField] private float moveSpeed;

    private void Update()
    {
        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
        transform.position = Vector3.Lerp(posTr1.position, posTr2.position, t);
    }
}
