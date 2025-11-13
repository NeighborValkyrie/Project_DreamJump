using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AutoDoor_Double_Target : MonoBehaviour
{
    [Header("문 Transform")]
    public Transform leftDoor;          // 실제 왼쪽 문           /*[변경가능_왼쪽문]*/
    public Transform rightDoor;         // 실제 오른쪽 문         /*[변경가능_오른쪽문]*/

    [Header("열렸을 때 위치(Target)")]
    public Transform leftOpenTarget;    // 왼쪽 문이 완전히 열렸을 때 자리  /*[변경가능_왼쪽열림타겟]*/
    public Transform rightOpenTarget;   // 오른쪽 문이 완전히 열렸을 때 자리/*[변경가능_오른쪽열림타겟]*/

    [Header("동작 설정")]
    public float moveSpeed = 4f;        // 열리고 닫히는 속도           /*[변경가능_속도]*/
    public string playerTag = "Player"; // 플레이어 태그 이름           /*[변경가능_플레이어태그]*/

    [Header("오디오 설정")]
    public AudioClip openDoorClip;      // 문 열릴 때 재생할 사운드      /*[변경가능_문열림사운드]*/
    public float openDoorVolume = 1f;   // 사운드 볼륨                  /*[변경가능_볼륨]*/
    public AudioSource audioSource;     // 재생에 사용할 AudioSource     /*[변경가능_AudioSource]*/
    public float minInterval = 0.3f;    // 연속 재생 방지 최소 간격(초)  /*[변경가능_쿨타임]*/

    Vector3 _leftClosedLocal;
    Vector3 _rightClosedLocal;
    Vector3 _leftOpenLocal;
    Vector3 _rightOpenLocal;

    bool _playerInRange = false;
    bool _prevPlayerInRange = false;
    float _lastOpenSoundTime = -999f;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (!leftDoor || !rightDoor || !leftOpenTarget || !rightOpenTarget)
        {
            Debug.LogWarning("AutoDoor_Double_Target : 문/타겟 Transform을 모두 지정해야 합니다.");
            enabled = false;
            return;
        }

        // **중요**: door와 target은 같은 부모 아래에 두는 걸 권장 (localPosition 기준)
        // 닫힌 위치 = 시작 localPosition
        _leftClosedLocal = leftDoor.localPosition;
        _rightClosedLocal = rightDoor.localPosition;

        // 열린 위치 = 타겟의 localPosition
        _leftOpenLocal = leftOpenTarget.localPosition;
        _rightOpenLocal = rightOpenTarget.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = false;
        }
    }

    void Update()
    {
        // 👉 상태 변화 체크: 닫힘(false) → 열림(true)로 바뀌는 그 프레임
        if (!_prevPlayerInRange && _playerInRange)
        {
            TryPlayOpenSound();
        }
        _prevPlayerInRange = _playerInRange;

        Vector3 targetL = _playerInRange ? _leftOpenLocal : _leftClosedLocal;
        Vector3 targetR = _playerInRange ? _rightOpenLocal : _rightClosedLocal;

        float t = moveSpeed * Time.deltaTime;

        leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, targetL, t);
        rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, targetR, t);
    }

    void TryPlayOpenSound()
    {
        if (!openDoorClip) return;

        // 연속 재생 방지 쿨타임 체크
        if (Time.time - _lastOpenSoundTime < minInterval) return;

        // AudioSource가 연결되어 있으면 그걸 사용
        if (audioSource)
        {
            audioSource.PlayOneShot(openDoorClip, openDoorVolume);
        }
        else
        {
            // 없으면 이 오브젝트 위치에서 3D 재생
            AudioSource.PlayClipAtPoint(openDoorClip, transform.position, openDoorVolume);
        }

        _lastOpenSoundTime = Time.time;
    }
}
