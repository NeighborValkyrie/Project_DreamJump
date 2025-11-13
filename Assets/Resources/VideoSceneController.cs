using UnityEngine;
using UnityEngine.Video; // VideoPlayer를 사용하기 위해 필요
using UnityEngine.SceneManagement; // Scene 관리를 위해 필요

public class VideoSceneController : MonoBehaviour
{
    // 1. 인스펙터에서 비디오 플레이어를 할당받습니다.
    public VideoPlayer videoPlayer;

    // 2. 인스펙터에서 다음에 로드할 씬의 이름을 적어줍니다.
    public string nextSceneName;

    void Start()
    {
        // VideoPlayer가 할당되었는지 확인
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // 비디오 재생이 끝났을 때 호출될 이벤트에 메소드를 등록(구독)합니다.
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // 비디오 재생이 완료되면 이 메소드가 호출됩니다.
    void OnVideoFinished(VideoPlayer vp)
    {
        // 다음 씬으로 전환합니다.
        SceneManager.LoadScene(nextSceneName);
    }

    // (선택 사항) 스킵 기능 추가
    void Update()
    {
        // 스페이스바나 E키를 누르면 영상을 스킵하고 다음 씬으로 넘어갑니다.
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            // 이벤트 구독을 해제하여 중복 로드를 방지할 수 있습니다.
            videoPlayer.loopPointReached -= OnVideoFinished; 
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // 오브젝트가 파괴될 때 이벤트 구독을 해제하는 것이 좋습니다.
    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}