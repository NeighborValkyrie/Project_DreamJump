# Dream Jump UI 시스템 - 빠른 시작 가이드

## 🚀 3단계로 설정하기

### 1️⃣ 타이틀 씬 설정 (5분)

#### GameManagers 오브젝트 만들기
```
1. 빈 GameObject 생성: "GameManagers"
2. 자식으로 "SceneController" 생성
   → SceneController.cs 추가
   → FadePanel 할당
3. 자식으로 "AudioManager" 생성
   → AudioManager.cs 추가
   → BGM 3개 할당 (Title, Game, Story)
```

#### UI 만들기
```
1. Canvas에 검은색 Image 추가: "FadePanel"
   → CanvasGroup 추가
   → FadePanel.cs 추가
   
2. TitleUI 만들기
   → StartButton, SettingsButton, QuitButton
   → TitleUIManager.cs 추가 및 버튼 할당
   
3. SceneBGMPlayer 오브젝트 생성
   → SceneBGMPlayer.cs 추가
```

### 2️⃣ 게임 씬 설정 (5분)

#### UI 만들기
```
1. FadePanel 복사해오기 (타이틀에서)

2. PausePanel 만들기
   → ResumeButton, SettingsButton, TitleButton, QuitButton
   → 처음엔 비활성화!
   
3. SettingsPanel 복사해오기 (타이틀에서)
   → 처음엔 비활성화!
   
4. GameManager 오브젝트 생성
   → GamePauseManagerUpdated.cs 추가
   → 모든 버튼과 패널 할당
   
5. SceneBGMPlayer 복사해오기
```

### 3️⃣ Build Settings (1분)

```
File > Build Settings
→ 01_TitleScene 추가
→ 02_MainGame 추가
```

---

## 📁 필요한 파일들

### 스크립트 위치
```
Assets/Scripts/Systems/
├── UI/
│   ├── TitleUIManager.cs ✅
│   ├── GamePauseManagerUpdated.cs ✅
│   └── SettingsPanel.cs ✅
└── Audio/
    ├── AudioManager.cs ✅
    └── SceneBGMPlayer.cs ✅
```

### 이미 있는 파일
```
Assets/Scripts/Systems/
├── Scene/
│   └── SceneController.cs (기존)
└── Utilities/
    └── FadePanel.cs (기존)
```

---

## 🎵 BGM 파일 준비

```
Assets/UISystem/Audio/
├── Title_BGM.mp3 (타이틀 화면)
├── Game_BGM.mp3 (게임 플레이)
└── Story_BGM.mp3 (스토리, 나중에)
```

---

## ✅ 빠른 체크리스트

### 타이틀 씬
- [ ] SceneController + AudioManager 생성
- [ ] FadePanel (검은색 Image + CanvasGroup + FadePanel.cs)
- [ ] TitleUI (버튼 3개 + TitleUIManager.cs)
- [ ] SceneBGMPlayer
- [ ] BGM 3개 AudioManager에 할당

### 게임 씬
- [ ] FadePanel 복사
- [ ] PausePanel 생성 (비활성화)
- [ ] SettingsPanel 복사 (비활성화)
- [ ] GameManager + GamePauseManagerUpdated.cs
- [ ] 모든 UI 요소 할당
- [ ] SceneBGMPlayer 복사

### Build Settings
- [ ] 01_TitleScene 추가
- [ ] 02_MainGame 추가

---

## 🎮 테스트

### 타이틀 씬
1. ▶️ 재생
2. BGM 들리는지 확인
3. 게임 시작 → 페이드 → 게임 씬 이동

### 게임 씬
1. BGM 바뀌는지 확인
2. ESC → 일시정지 (BGM도 멈춤)
3. 계속하기 → 재개 (BGM 재개)
4. 설정 → 볼륨 조절
5. 타이틀로 → 타이틀 이동 (페이드)

---

## 🔧 자주 하는 실수

### ❌ AudioManager를 여러 씬에 만들지 마세요!
→ ✅ 타이틀 씬에만 있으면 자동으로 유지됩니다

### ❌ PausePanel을 활성화 상태로 두지 마세요!
→ ✅ Inspector에서 비활성화 해야 합니다

### ❌ FadePanel에 Raycast Target 켜두지 마세요!
→ ✅ 체크 해제해야 클릭을 방해하지 않습니다

### ❌ Build Settings에 씬 안 넣으면 로드 안 됩니다!
→ ✅ 꼭 추가하세요

---

## 💡 나중에 추가할 것

### 스토리 씬 추가 시
1. 03_StoryScene.unity 만들기
2. SceneController Prefab 배치
3. SceneBGMPlayer 배치
4. FadePanel 추가
5. Story BGM 할당
6. TitleUIManager → Use Story Scene 체크
7. Build Settings 추가

---

## 📞 문제 해결

**BGM 안 나와요**
→ AudioManager 있나요? BGM 할당했나요?

**씬 전환 안 돼요**
→ Build Settings에 씬 추가했나요?

**ESC 안 먹혀요**
→ GamePauseManager 있나요? 패널 할당했나요?

**페이드 안 보여요**
→ FadePanel에 CanvasGroup 있나요? 색이 검은색인가요?

---

자세한 내용은 `UI_SETUP_GUIDE.md` 참고!
