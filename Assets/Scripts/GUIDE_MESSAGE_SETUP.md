# 🎯 가이드 메시지 시스템 완전 개편!

## ✨ 새로운 기능

**가이드 메시지 패널에 체크포인트 스타일 애니메이션 추가!**

### Before (기존)
```
- 단순 On/Off
- 타이머만 있음
- 일시정지 중 안 보임
```

### After (업그레이드!) 🎉
```
✅ Fade In (부드럽게 나타남)
✅ Hold (잠시 유지)
✅ Fade Out (부드럽게 사라짐)
✅ 위로 살짝 떠오르는 효과
✅ 일시정지 중에도 작동!
✅ CanvasGroup 자동 추가
```

---

## 📋 설정 방법

### 1단계: GameUIController.cs 교체
```
기존 파일:
Assets/UISystem/Prefabs/GameUIController.cs

→ 이미 업데이트 완료! ✅
```

### 2단계: Inspector 설정

**GameUI 오브젝트 선택 → Inspector:**

```
GameUIController (Script)

✅ Game UI Elements
├─ Pause Menu Panel: [PauseMenuPanel 할당]
├─ Guide Message Panel: [GuideMessagePanel 할당] ⭐
└─ Guide Message Text: [GuideMessagePanel 안의 Text 할당] ⭐

✅ Pause Menu Buttons
├─ Resume Button: [할당]
├─ Restart Button: [할당]
├─ Title Button: [할당]
├─ Settings Button: [할당]
└─ Quit Button: [할당]

✅ Settings
└─ Settings Panel: [할당]

✅ Scene Names
└─ Title Scene Name: "Title"

✅ Guide Message Animation (새로 추가!) ⭐
├─ Fade In Duration: 0.12
├─ Hold Duration: 0.8
├─ Fade Out Duration: 0.25
└─ Move Offset: X=0, Y=40
```

### 3단계: GuideMessagePanel 구조

**Canvas 안에 만들기:**

```
Canvas
└─ GuideMessagePanel (Panel 또는 Image)
    ├─ Background (Image, 선택사항)
    └─ Text (TextMeshProUGUI)
```

**GuideMessagePanel 설정:**
```
Inspector:
- 처음엔 비활성화 ☐ (체크 해제)
- CanvasGroup: 자동으로 추가됨! ✅
```

---

## 🎨 GuideMessagePanel 만들기

### 방법 1: 간단하게
```
1. Canvas 우클릭 → UI → Panel
2. 이름: "GuideMessagePanel"
3. 배경색: 약간 투명한 검정색
4. 위치: 화면 상단 중앙
```

### 방법 2: 예쁘게
```
GuideMessagePanel
├─ Background (Image)
│   - Sprite: UI/Skin/Background
│   - Color: rgba(0, 0, 0, 150)
│   - RectTransform:
│     - Width: 400
│     - Height: 100
│     - Anchor: Top Center
│     - Position: (0, -100, 0)
│
└─ Text (TextMeshProUGUI)
    - Font: 원하는 폰트
    - Font Size: 36
    - Color: White
    - Alignment: Center
    - RectTransform: Stretch (부모 크기에 맞춤)
    - Padding: 20
```

---

## 🎮 사용 방법

### 기본 사용
```csharp
// GameUIController 찾기
GameUIController uiController = FindObjectOfType<GameUIController>();

// 메시지 표시
uiController.ShowGuideMessage("게임 시작!");
```

### 체크포인트에서
```csharp
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        GameUIController uiController = FindObjectOfType<GameUIController>();
        if (uiController != null)
        {
            uiController.ShowCheckpointMessage("체크포인트!");
        }
    }
}
```

### 튜토리얼에서
```csharp
void Start()
{
    GameUIController uiController = FindObjectOfType<GameUIController>();
    if (uiController != null)
    {
        uiController.ShowGuideMessage("WASD로 이동하세요!");
    }
}
```

---

## ⚙️ 커스터마이징

### Inspector에서 조정
```
GameUIController:

Guide Message Animation
├─ Fade In Duration: 0.12   ← 빠르게 나타남
├─ Hold Duration: 0.8        ← 오래 보임
├─ Fade Out Duration: 0.25   ← 천천히 사라짐
└─ Move Offset: 
    - X: 0   ← 좌우 이동 없음
    - Y: 40  ← 위로 40px 이동
```

### 예쁘게 만들기 팁
```
짧고 강렬: fadeIn=0.1, hold=0.5, fadeOut=0.2
부드럽고 우아: fadeIn=0.3, hold=1.0, fadeOut=0.5
느리고 여유: fadeIn=0.5, hold=1.5, fadeOut=0.8

위로 많이: Y=80
위로 조금: Y=20
옆으로: X=50, Y=0
```

---

## ✨ 특징

### 1. Time.unscaledDeltaTime 사용
```
일시정지(Time.timeScale = 0) 중에도 작동!
ESC 눌러도 메시지 볼 수 있음!
```

### 2. CanvasGroup 자동 추가
```
GuideMessagePanel에 CanvasGroup이 없으면
자동으로 추가해줌! 편리!
```

### 3. 중복 방지
```
메시지 표시 중 새 메시지가 오면
기존 애니메이션 중단하고 새로 시작
```

### 4. 안전한 null 체크
```
모든 참조에 null 체크 포함
오류 발생 시 Console에 명확한 메시지
```

---

## 🐛 문제 해결

### 메시지가 안 보여요
```
✅ GuideMessagePanel 할당됨?
✅ GuideMessageText 할당됨?
✅ GuideMessagePanel이 Canvas 안에 있음?
✅ Canvas가 활성화되어 있음?
```

### 애니메이션이 이상해요
```
✅ CanvasGroup이 추가되었는지 확인
   (자동 추가되지만 수동으로도 추가 가능)
✅ RectTransform이 있는지 확인
✅ Move Offset 값 확인 (너무 크면 이상함)
```

### 일시정지 중에 안 보여요
```
✅ Time.unscaledDeltaTime 사용 확인
   (코드에 이미 적용되어 있음)
✅ GuideMessagePanel이 Pause 패널보다
   Hierarchy 아래에 있는지 확인
   (나중에 그려짐 = 위에 보임)
```

---

## 📊 Inspector 스크린샷 예시

```
GameUIController
┌─────────────────────────────────────┐
│ Game UI Elements                    │
│  Pause Menu Panel:  [PauseMenu]     │
│  Guide Message Panel: [GuideMsg] ⭐ │
│  Guide Message Text:  [Text]     ⭐ │
│                                     │
│ Pause Menu Buttons                  │
│  Resume Button:  [ResumeBtn]        │
│  Restart Button: [RestartBtn]       │
│  Title Button:   [TitleBtn]         │
│  Settings Button:[SettingsBtn]      │
│  Quit Button:    [QuitBtn]          │
│                                     │
│ Settings                            │
│  Settings Panel: [SettingsPanel]    │
│                                     │
│ Scene Names                         │
│  Title Scene Name: Title            │
│                                     │
│ Guide Message Animation          ⭐ │
│  Fade In Duration:  0.12            │
│  Hold Duration:     0.8             │
│  Fade Out Duration: 0.25            │
│  Move Offset:       X:0  Y:40       │
└─────────────────────────────────────┘
```

---

## ✅ 최종 체크리스트

### 필수 설정
- [ ] GameUIController.cs 업데이트됨
- [ ] GuideMessagePanel 만들었음
- [ ] GuideMessageText 추가했음
- [ ] Inspector에 모두 할당했음

### 선택 설정
- [ ] 애니메이션 타이밍 조정
- [ ] Move Offset 조정
- [ ] 패널 디자인 꾸미기

### 테스트
- [ ] 메시지 표시 작동 확인
- [ ] 애니메이션 부드러움 확인
- [ ] 일시정지 중에도 작동 확인

---

## 🎯 결과

**기존 가이드 메시지 패널에 체크포인트 수준의 애니메이션 추가!**

- ✅ CheckpointPopupUI 필요 없음!
- ✅ 기존 패널 그대로 사용
- ✅ 더 예쁜 애니메이션
- ✅ 일시정지 중에도 작동
- ✅ 커스터마이징 쉬움

완성! 🎉
