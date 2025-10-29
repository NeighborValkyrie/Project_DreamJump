using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player_clickk : MonoBehaviour
{
    RaycastHit hit;
    bool isHit;
    public UnityEvent<RaycastHit> hitEvent = new UnityEvent<RaycastHit> ();
    public UnityEvent<RaycastHit,bool> CrossOverEvent = new UnityEvent<RaycastHit,bool> ();
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            isHit = true;
        else
            isHit = false;

        CrossOverEvent.Invoke(hit,isHit);

        if (Input.GetKeyDown(KeyCode.Mouse0) && isHit == true)
                hitEvent.Invoke(hit);
    }
}
