using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationControl : MonoBehaviour
{
    [SerializeField] private RotatingShaft rotatingShaft;
    [SerializeField] private float rotationSpeed = 1f;

    [SerializeField] private bool reciprocate = false;
    [SerializeField] private Vector2 rotationRange;
    private float t = 1;
    void Update()
    {
        if (reciprocate)
        {
            Vector3 angle = transform.localEulerAngles;
            float targetAngle = 0;
            targetAngle = Mathf.Lerp(rotationRange.x, rotationRange.y, t);
            switch (rotatingShaft)
            {
                case RotatingShaft.ShaftX:
                    transform.localEulerAngles = new Vector3(targetAngle, angle.y, angle.z);
                    break;
                case RotatingShaft.ShaftY:
                    transform.localEulerAngles = new Vector3(angle.x, targetAngle, angle.z);
                    break;
                case RotatingShaft.ShaftZ:
                    transform.localEulerAngles = new Vector3(angle.x, angle.y, targetAngle);
                    break;
            }
            t = Mathf.PingPong(Time.time * rotationSpeed, 1);
        }
        else
        {
            switch (rotatingShaft)
            {
                case RotatingShaft.ShaftX:
                    transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
                    break;
                case RotatingShaft.ShaftY:
                    transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                    break;
                case RotatingShaft.ShaftZ:
                    transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                    break;
            } 
        }
        
    }
    public enum RotatingShaft
    {
        ShaftX,
        ShaftY,
        ShaftZ,
    }
}
