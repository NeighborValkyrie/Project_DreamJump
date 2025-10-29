using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorTransform; // door_001 오브젝트
    public bool isOpen = false;
    public float openAngle = 90f;
    public float animationSpeed = 3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isAnimating = false;
    private bool playerNearby = false;

    private void Start()
    {
        // door_001의 초기 회전값 저장
        if (doorTransform != null)
        {
            closedRotation = doorTransform.localRotation;
            openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        }
        else
        {
            Debug.LogError($"{gameObject.name}: doorTransform이 할당되지 않았습니다!");
        }

        // Player의 CrossOverEvent에 리스너 추가
        Player_clickk player = FindObjectOfType<Player_clickk>();
        if (player != null)
        {
            player.CrossOverEvent.AddListener(OnMouseOver);
            Debug.Log($"{gameObject.name}: Player_clickk 리스너 등록 완료");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Player_clickk를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        // 문 애니메이션
        if (isAnimating && doorTransform != null)
        {
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            doorTransform.localRotation = Quaternion.Lerp(
                doorTransform.localRotation,
                targetRotation,
                Time.deltaTime * animationSpeed
            );

            // 애니메이션 완료 체크
            if (Quaternion.Angle(doorTransform.localRotation, targetRotation) < 0.1f)
            {
                doorTransform.localRotation = targetRotation;
                isAnimating = false;
            }
        }

        // E키 입력 (플레이어가 문 근처에 있을 때만)
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
    }

    // 마우스 오버 감지
    public void OnMouseOver(RaycastHit _hit, bool _isHit)
    {
        // Ray가 이 오브젝트 또는 자식을 맞췄는지 확인
        if (_isHit && _hit.collider != null)
        {
            if (IsPartOfThisDoor(_hit.collider.gameObject))
            {
                playerNearby = true;
                Debug.Log($"{gameObject.name}: E키를 눌러 문 열기/닫기");
            }
            else
            {
                playerNearby = false;
            }
        }
        else
        {
            playerNearby = false;
        }
    }

    // 히트한 오브젝트가 이 문의 일부인지 확인
    bool IsPartOfThisDoor(GameObject hitObject)
    {
        // 부모 오브젝트 자체를 맞췄거나
        if (hitObject == gameObject)
            return true;

        // doorTransform을 맞췄거나
        if (doorTransform != null && hitObject == doorTransform.gameObject)
            return true;

        // 자식 오브젝트인지 확인
        return hitObject.transform.IsChildOf(transform);
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        isAnimating = true;

        Debug.Log($"{gameObject.name}: 문이 {(isOpen ? "열렸습니다" : "닫혔습니다")}");
    }

    private void OnDestroy()
    {
        // 리스너 제거 (메모리 누수 방지)
        Player_clickk player = FindObjectOfType<Player_clickk>();
        if (player != null)
        {
            player.CrossOverEvent.RemoveListener(OnMouseOver);
        }
    }
}
