using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingPoint : MonoBehaviour
{
    [Header("씬 이름 설정")]
    public string endingSceneName = "EndingScene";   /*[변경가능_엔딩씬이름]*/
    public string mainTitleSceneName = "MainTitle";  /*[변경가능_메인타이틀씬이름]*/

    [Header("엔딩씬 재생 시간(초)")]
    public float endingDuration = 10f;               /*[변경가능_엔딩길이초]*/

    bool _triggered = false;

    void Awake()
    {
        // 이 오브젝트는 씬이 바뀌어도 유지되게 함
        DontDestroyOnLoad(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // 이미 한 번 트리거 됐으면 무시
        if (_triggered) return;

        // Player 태그에만 반응
        if (!other.CompareTag("Player")) return;      /*[변경가능_태그이름]*/

        _triggered = true;

        // 다시 닿지 않도록 콜라이더 비활성화
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        StartCoroutine(PlayEndingRoutine());
    }

    System.Collections.IEnumerator PlayEndingRoutine()
    {
        // 1) 엔딩 씬 로드
        Debug.Log("EndingPoint: 엔딩씬 로드 → " + endingSceneName);
        SceneManager.LoadScene(endingSceneName, LoadSceneMode.Single);

        // 2) 엔딩씬 재생 시간만큼 대기
        yield return new WaitForSeconds(endingDuration);

        // 3) 메인 타이틀 씬 로드
        Debug.Log("EndingPoint: 메인 타이틀 로드 → " + mainTitleSceneName);
        SceneManager.LoadScene(mainTitleSceneName, LoadSceneMode.Single);

        // 🔹 3-1) 메인 타이틀 진입 시 마우스 커서 다시 보이게 & 잠금 해제
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 4) 더 이상 필요 없으니 자기 자신 삭제
        Destroy(gameObject);
    }
}
