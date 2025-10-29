using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cube_outline : MonoBehaviour
{
    Outlinee outline;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outlinee>();
        GameObject.Find("Player").GetComponent<Player_clickk>().hitEvent.AddListener(change_color);
        GameObject.Find("Player").GetComponent<Player_clickk>().CrossOverEvent.AddListener(change_outline);
    }

    public void change_color(RaycastHit _hit)
    {
        if (_hit.collider.gameObject.name != this.gameObject.name)
            return;

        GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

    public void change_outline(RaycastHit _hit,bool _isHit)
    {
        if (!_isHit || _hit.collider.gameObject.name != this.gameObject.name)
        {
            outline.OutlineWidth = 0;
            return;
        }

        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 5;
    }
}
