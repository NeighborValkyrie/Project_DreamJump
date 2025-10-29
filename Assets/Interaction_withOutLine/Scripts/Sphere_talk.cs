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

    private bool isUIActive = false;

    private void Start()
    {
        GameObject.Find("Player").GetComponent<Player_clickk>().hitEvent.AddListener(ShowUI);
    }

    public void ShowUI(RaycastHit _hit)
    {
        if (_hit.collider.gameObject.name != this.gameObject.name)
            return;

        // 배열 체크
        if (contents == null || contents.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: Contents 배열이 비어있습니다!");
            return;
        }

        // idx 범위 체크
        if (idx >= contents.Length)
            idx = 0;

        Canvas.SetActive(true);
        isUIActive = true;
        name_text.text = Char_name;
        talk_text.text = contents[idx];

        // 기존 리스너 제거 후 추가 (중복 방지)
        Preview_btn.GetComponent<Button>().onClick.RemoveListener(prev_btn);
        Next_btn.GetComponent<Button>().onClick.RemoveListener(next_btn);
        Escape_btn.GetComponent<Button>().onClick.RemoveListener(escape);

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
        isUIActive = false;
        Preview_btn.GetComponent<Button>().onClick.RemoveListener(prev_btn);
        Next_btn.GetComponent<Button>().onClick.RemoveListener(next_btn);
        Escape_btn.GetComponent<Button>().onClick.RemoveListener(escape);
        Canvas.SetActive(false);
    }

    private void Update()
    {
        // UI가 활성화되어 있을 때만 키 입력 처리
        if (isUIActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                next_btn();
            }
            if (Input.GetKeyDown(KeyCode.Q))  // ! 제거
            {
                prev_btn();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escape();
            }
        }
    }
}