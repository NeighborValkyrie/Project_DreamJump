# 🎯 가이드 메시지 → 체크포인트 팝업 통합 가이드

## ✨ 변경 사항

**GameUIController의 가이드 메시지 기능을 CheckpointPopupUI로 통합했습니다!**

### Before (기존)
```csharp
// 별도의 가이드 메시지 패널 사용
[SerializeField] private GameObject guideMessagePanel;
[SerializeField] private TextMeshProUGUI guideMessageText;

// 코루틴으로 타이머 구현
StartCoroutine(ShowGuideMessageCoroutine(message, duration));
```

### After (변경)
```csharp
// CheckpointPopupUI 싱글톤 사용 (더 예쁜 애니메이션!)
public void ShowGuideMessage(string message, float duration = 0f)
{
    CheckpointPopupUI.Show(message);
}
```

---

## 📋 설정 방법

### 1단계: GameUIController 교체
```
기존 파일:
Assets/UISystem/Prefabs/GameUIController.cs

→ 이미 업데이트 완료! ✅
```

### 2단계: Inspector 설정 간소화

**GameUI 오브젝트 선택 → Inspector:**

```
GameUIController (Script)

✅ 필요한 것만:
├─ Game UI Elements
│   └─ Pause Menu Panel: [할당]
│
├─ Pause Menu Buttons
│   ├─ Resume Button: [할당]
│   ├─ Restart Button: [할당]
│   ├─ Title Button: [할당]
│   ├─ Settings Button: [할당]
│   └─ Quit Button: [할당]
│
├─ Settings
│   └─ Settings Panel: [할당]
│
└─ Scene Names
    └─ Title Scene Name: "Title"

❌ 제거된 것 (더 이상 필요 없음):
    - Guide Message Panel (삭제됨)
    - Guide Message Text (삭제됨)
```

### 3단계: CheckpointPopupUI 확인

**게임 씬 Hierarchy에 있어야 함:**

```
Canvas
└─ CheckpointPopup (이미 있을 것)
    └─ CheckpointPopupUI (Script) ✅
```

---

## 🎮 사용 방법

### 기본 사용
```csharp
// 다른 스크립트에서 호출
GameUIController uiController = FindObjectOfType<GameUIController>();
uiController.ShowGuideMessage("게임 시작!");
```

### 체크포인트에서 호출
```csharp
// 체크포인트 스크립트에서
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        // 방법 1: CheckpointPopupUI 직접 사용 (권장)
        CheckpointPopupUI.Show("체크포인트!");
        
        // 방법 2: GameUIController 통해 사용
        GameUIController uiController = FindObjectOfType<GameUIController>();
        if (uiController != null)
        {
            uiController.ShowCheckpointMessage("체크포인트!");
        }
    }
}
```

### 튜토리얼 메시지
```csharp
// 튜토리얼 스크립트에서
void Start()
{
    CheckpointPopupUI.Show("WASD로 이동하세요!");
}

void OnJump()
{
    CheckpointPopupUI.Show("스페이스바로 점프!");
}
```

---

## ✨ CheckpointPopupUI 장점

### 1. 더 예쁜 애니메이션
```
- Fade In (부드럽게 나타남)
- Hold (잠시 유지)
- Fade Out (부드럽게 사라짐)
- 위로 살짝 떠오르는 효과 (moveOffset)
```

### 2. 싱글톤 패턴
```csharp
// 어디서든 간편하게 호출
CheckpointPopupUI.Show("메시지");
```

### 3. 타이밍 자동 관리
```
duration 파라미터 필요 없음!
CheckpointPopupUI가 자동으로:
- fadeIn: 0.12초
- hold: 0.80초
- fadeOut: 0.25초
총 약 1.2초 자동 표시
```

### 4. Time.unscaledDeltaTime 사용
```
일시정지 중에도 메시지 표시 가능!
Time.timeScale = 0 상태에서도 작동
```

---

## 🎨 커스터마이징

CheckpointPopupUI.cs에서 조정 가능:

```csharp
[Header("Animation")]
[Min(0)] public float fadeIn = 0.12f;    // 나타나는 시간
[Min(0)] public float hold = 0.80f;      // 유지 시간
[Min(0)] public float fadeOut = 0.25f;   // 사라지는 시간
public Vector2 moveOffset = new Vector2(0f, 40f); // 이동 거리
```

Inspector에서 직접 수정 가능! ⭐

---

## 📊 비교

| 기능 | 기존 가이드 메시지 | CheckpointPopupUI |
|------|------------------|-------------------|
| 애니메이션 | 단순 On/Off | Fade + Move ✨ |
| 타이밍 | 수동 설정 필요 | 자동 관리 ✅ |
| 일시정지 중 | 안 보임 ❌ | 보임 ✅ |
| 코드 복잡도 | 코루틴 필요 | 한 줄 호출 ✅ |
| 싱글톤 | 없음 | 있음 ✅ |

---

## 🔧 주의사항

### 1. CheckpointPopupUI가 씬에 있어야 함
```
Canvas
└─ CheckpointPopup (GameObject)
    └─ CheckpointPopupUI (Script)
        ├─ Canvas Group
        └─ Text (또는 TextMeshPro)
```

### 2. 필수 컴포넌트
```
CheckpointPopup GameObject:
- RectTransform ✅
- CanvasGroup ✅
- CheckpointPopupUI ✅

자식 오브젝트:
- Text 또는 TextMeshProUGUI ✅
```

### 3. 싱글톤 확인
```csharp
// 사용 전 확인
if (CheckpointPopupUI.Instance != null)
{
    CheckpointPopupUI.Show("메시지");
}
else
{
    Debug.LogWarning("CheckpointPopupUI가 씬에 없습니다!");
}
```

---

## 💡 활용 예시

### 예시 1: 스테이지 시작
```csharp
void Start()
{
    CheckpointPopupUI.Show("스테이지 1 시작!");
}
```

### 예시 2: 아이템 획득
```csharp
void OnItemCollected(string itemName)
{
    CheckpointPopupUI.Show($"{itemName} 획득!");
}
```

### 예시 3: 보스 등장
```csharp
void OnBossAppear()
{
    CheckpointPopupUI.Show("보스 등장!");
}
```

### 예시 4: 미션 완료
```csharp
void OnMissionComplete()
{
    CheckpointPopupUI.Show("미션 완료!");
}
```

### 예시 5: 체크포인트
```csharp
void OnCheckpoint()
{
    CheckpointPopupUI.Show("체크포인트!");
    // 세이브 로직...
}
```

---

## 🎯 마이그레이션 가이드

### 기존 코드가 있다면:

```csharp
// Before (기존)
GameUIController uiController = FindObjectOfType<GameUIController>();
uiController.ShowGuideMessage("메시지", 3f);

// After (변경 필요 없음! 그대로 작동)
GameUIController uiController = FindObjectOfType<GameUIController>();
uiController.ShowGuideMessage("메시지"); // duration 무시됨

// 또는 직접 사용 (더 간단!)
CheckpointPopupUI.Show("메시지");
```

---

## ✅ 체크리스트

- [ ] GameUIController.cs 업데이트됨
- [ ] CheckpointPopupUI가 씬에 있음
- [ ] CheckpointPopup에 CanvasGroup 있음
- [ ] CheckpointPopup에 Text 있음
- [ ] Inspector에서 타이밍 조정 (선택사항)
- [ ] 테스트: CheckpointPopupUI.Show("테스트") 작동 확인

---

## 🚀 결과

**더 이상 별도의 가이드 메시지 패널 필요 없음!**
- ✅ 코드 간소화
- ✅ 더 예쁜 애니메이션
- ✅ 일시정지 중에도 작동
- ✅ 어디서든 쉽게 호출

체크포인트와 가이드 메시지를 하나로 통합 완료! 🎉
