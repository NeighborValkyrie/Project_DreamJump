using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Sphere_talk : MonoBehaviour
{
    public string Char_name;
    public string[] contents;
    public int idx = 0;
    public GameObject Canvas;
    public Text name_text;
    public Text talk_text;
    public GameObject Preview_btn;
    public GameObject Next_btn;
    public GameObject Escape_btn;
    // Start is called before the first frame update
    private void Start()
    {
        GameObject.Find("Player").GetComponent<Player_clickk>().hitEvent.AddListener(ShowUI);
    }

    public void ShowUI(RaycastHit _hit)
    {
        if (_hit.collider.gameObject.name != this.gameObject.name)
            return;

        Canvas.SetActive(true);
        name_text.text = Char_name;
        talk_text.text = contents[idx];

        Preview_btn.GetComponent<Button>().onClick.AddListener(prev_btn);
        Next_btn.GetComponent<Button>().onClick.AddListener(next_btn);
        Escape_btn.GetComponent<Button>().onClick.AddListener(escape);

        Preview_btn.SetActive(false);
        Next_btn.SetActive(true);
    }
    public void next_btn()
    {
        Preview_btn.SetActive(true);
        if (idx < contents.Length - 1)
        {
            idx++;
            talk_text.text = contents[idx];
        }

        if (idx == contents.Length - 1)
            Next_btn.SetActive(false);
    }
    public void prev_btn()
    {
        Next_btn.SetActive(true);
        if (idx > 0)
        {
            idx--;
            talk_text.text = contents[idx];
        }

        if (idx == 0)
            Preview_btn.SetActive(false);
    }

    public void escape()
    {
        idx = 0;
        Preview_btn.GetComponent<Button>().onClick.RemoveListener(prev_btn);
        Next_btn.GetComponent<Button>().onClick.RemoveListener(next_btn);
        Escape_btn.GetComponent<Button>().onClick.RemoveListener(escape);
        Canvas.SetActive(false);
    }

}
