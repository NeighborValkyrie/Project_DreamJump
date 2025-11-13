# Dream Jump UI 시스템 설정 가이드

## 📋 목차
1. [개요](#개요)
2. [필요한 스크립트](#필요한-스크립트)
3. [타이틀 씬 설정](#타이틀-씬-설정)
4. [게임 씬 설정](#게임-씬-설정)
5. [오디오 시스템 설정](#오디오-시스템-설정)
6. [페이드 효과 설정](#페이드-효과-설정)

---

## 개요

이 가이드는 Dream Jump 프로젝트의 UI 시스템을 설정하는 방법을 설명합니다.

### 주요 기능
- ✅ 타이틀 씬 버튼 연결 (게임 시작, 설정, 종료)
- ✅ 씬 전환 페이드 효과 (페이드아웃 → 블랙 → 페이드인)
- ✅ 게임 씬 일시정지 (ESC 키)
- ✅ 설정 패널 (볼륨 조절)
- ✅ 배경음악 자동 재생 및 전환
- ✅ 스토리 씬 지원 (추후 추가)

---

## 필요한 스크립트

### 1. UI 시스템
- `TitleUIManager.cs` - 타이틀 씬 UI 관리
- `GamePauseManagerUpdated.cs` - 게임 씬 일시정지 관리
- `SettingsPanel.cs` - 설정 패널 UI 관리

### 2. 오디오 시스템
- `AudioManager.cs` - 배경음악 및 효과음 관리
- `SceneBGMPlayer.cs` - 씬별 배경음악 자동 재생

### 3. 씬 시스템
- `SceneController.cs` - 씬 전환 및 페이드 효과 (이미 존재)
- `FadePanel.cs` - UI 페이드 효과 (이미 존재)

---

## 타이틀 씬 설정

### 1. Hierarchy 구조 만들기

```
01_TitleScene
├── GameManagers (빈 GameObject)
│   ├── SceneController (Prefab으로 만들 것)
│   └── AudioManager
├── Canvas
│   ├── FadePanel (검은색 Image)
│   ├── TitleUI
│   │   ├── Logo (이미지)
│   │   ├── StartButton
│   │   ├── SettingsButton
│   │   └── QuitButton
│   └── SettingsPanel (처음엔 비활성화)
│       ├── BGM Slider
│       ├── SFX Slider
│       └── CloseButton
└── SceneBGMPlayer (빈 GameObject)
```

### 2. GameManagers 설정

#### SceneController 설정
1. 빈 GameObject 생성: "SceneController"
2. `SceneController.cs` 컴포넌트 추가
3. **중요: Prefab으로 만들어서 모든 씬에서 재사용**
4. Inspector 설정:
   - Fade Panel: `FadePanel` 할당
   - Fade Duration: `1.0` (기본값)

#### AudioManager 설정
1. 빈 GameObject 생성: "AudioManager"
2. `AudioManager.cs` 컴포넌트 추가
3. **이 GameObject는 DontDestroyOnLoad로 자동 유지됨**
4. Inspector 설정:
   - Title BGM: 타이틀 배경음악 AudioClip 할당
   - Game BGM: 게임 배경음악 AudioClip 할당
   - Story BGM: 스토리 배경음악 AudioClip 할당 (나중에)
   - BGM Volume: `0.7`
   - SFX Volume: `1.0`
   - Fade Duration: `1.0`

### 3. Canvas 설정

#### FadePanel 만들기
1. Canvas에 Image 추가: "FadePanel"
2. 설정:
   - Color: 검은색 (R:0, G:0, B:0, A:255)
   - Anchor: Stretch (전체 화면)
   - Raycast Target: **체크 해제** (클릭 방지)
3. `CanvasGroup` 컴포넌트 추가
4. `FadePanel.cs` 컴포넌트 추가

#### TitleUI 설정
1. 빈 GameObject: "TitleUI"
2. 버튼 3개 생성:
   - StartButton - "게임 시작"
   - SettingsButton - "설정"
   - QuitButton - "종료"
3. TitleUI에 `TitleUIManager.cs` 컴포넌트 추가
4. Inspector 설정:
   - Start Button: StartButton 할당
   - Settings Button: SettingsButton 할당
   - Quit Button: QuitButton 할당
   - Game Scene Name: "02_MainGame"
   - Story Scene Name: "03_StoryScene"
   - Use Story Scene: 체크 해제 (나중에 스토리 씬 추가 시 체크)

#### SettingsPanel 만들기
1. Panel 생성: "SettingsPanel"
2. 배경 이미지 추가 (어두운 반투명)
3. 내용 추가:
   ```
   SettingsPanel
   ├── Title (Text: "설정")
   ├── BGM Volume
   │   ├── Label (Text: "배경음악")
   │   ├── Slider
   │   └── ValueText (Text: "70%")
   ├── SFX Volume
   │   ├── Label (Text: "효과음")
   │   ├── Slider
   │   └── ValueText (Text: "100%")
   ├── TestButton (Text: "효과음 테스트")
   └── CloseButton (Text: "닫기")
   ```
4. SettingsPanel에 `SettingsPanel.cs` 컴포넌트 추가
5. Inspector 설정:
   - BGM Slider: BGM Slider 할당
   - SFX Slider: SFX Slider 할당
   - BGM Volume Text: BGM ValueText 할당
   - SFX Volume Text: SFX ValueText 할당
   - SFX Test Button: TestButton 할당
   - Test SFX: 테스트용 효과음 AudioClip 할당
6. **SettingsPanel GameObject 비활성화** (처음에는 숨김)

### 4. SceneBGMPlayer 설정
1. 빈 GameObject 생성: "SceneBGMPlayer"
2. `SceneBGMPlayer.cs` 컴포넌트 추가
3. Inspector 설정:
   - Play On Start: 체크
   - Fade In: 체크

---

## 게임 씬 설정

### 1. Hierarchy 구조

```
02_MainGame
├── Canvas
│   ├── FadePanel (타이틀에서 복사)
│   ├── GameUI
│   │   ├── HP Bar
│   │   ├── Score
│   │   └── Timer
│   ├── PausePanel (처음엔 비활성화)
│   │   ├── Title (Text: "일시정지")
│   │   ├── ResumeButton (Text: "계속하기")
│   │   ├── SettingsButton (Text: "설정")
│   │   ├── TitleButton (Text: "타이틀로")
│   │   └── QuitButton (Text: "종료")
│   └── SettingsPanel (타이틀에서 복사, 비활성화)
│       └── CloseButton (Text: "돌아가기")
├── GameManager (빈 GameObject)
│   └── GamePauseManager 컴포넌트
└── SceneBGMPlayer (타이틀에서 복사)
```

### 2. PausePanel 설정
1. Panel 생성: "PausePanel"
2. 배경 이미지 추가 (어두운 반투명)
3. 버튼 4개 추가:
   - ResumeButton - "계속하기"
   - SettingsButton - "설정"
   - TitleButton - "타이틀로"
   - QuitButton - "종료"
4. **PausePanel GameObject 비활성화**

### 3. GamePauseManager 설정
1. 빈 GameObject 생성: "GameManager"
2. `GamePauseManagerUpdated.cs` 컴포넌트 추가
3. Inspector 설정:
   - Pause Panel: PausePanel 할당
   - Settings Panel: SettingsPanel 할당
   - Resume Button: ResumeButton 할당
   - Settings Button: SettingsButton (PausePanel) 할당
   - Title Button: TitleButton 할당
   - Quit Button: QuitButton 할당
   - Close Settings Button: CloseButton (SettingsPanel) 할당
   - Title Scene Name: "01_TitleScene"
   - Pause Key: Escape

### 4. SettingsPanel 설정
- 타이틀 씬의 SettingsPanel을 복사
- CloseButton을 "돌아가기"로 텍스트 변경
- **GamePauseManager의 Close Settings Button에 할당**

---

## 오디오 시스템 설정

### 1. AudioManager는 한 번만!
- AudioManager는 **타이틀 씬에만** 존재
- DontDestroyOnLoad로 자동 유지됨
- 다른 씬에는 추가하지 말 것!

### 2. SceneBGMPlayer 사용
- 각 씬마다 SceneBGMPlayer를 배치
- 씬 로드 시 자동으로 해당 씬의 BGM 재생
- AudioManager에 설정된 BGM 사용

### 3. BGM 파일 준비
1. Assets/UISystem/Audio 폴더에 배경음악 추가
2. AudioClip으로 Import Settings 설정:
   - Load Type: Streaming (용량 큰 BGM)
   - Compression Format: Vorbis
   - Quality: 70-100
3. AudioManager Inspector에 할당

### 4. 효과음 추가 (선택사항)
```csharp
// 게임 코드에서 효과음 등록
AudioManager.Instance.RegisterSFX("jump", jumpSoundClip);
AudioManager.Instance.RegisterSFX("coin", coinSoundClip);

// 효과음 재생
AudioManager.Instance.PlaySFX("jump");
```

---

## 페이드 효과 설정

### FadePanel 설정 체크리스트
- ✅ Image 컴포넌트 존재
- ✅ Color: 검은색 (A: 255)
- ✅ Anchor: Stretch (전체 화면)
- ✅ CanvasGroup 컴포넌트 추가
- ✅ FadePanel.cs 컴포넌트 추가
- ✅ Raycast Target: 체크 해제
- ✅ SceneController에 할당

---

## Build Settings 설정

### Scenes In Build 추가
1. File > Build Settings
2. Scenes In Build에 씬 추가 (순서대로):
   - 01_TitleScene
   - 02_MainGame
   - 03_StoryScene (나중에 추가)

---

## 테스트 체크리스트

### 타이틀 씬
- [ ] 씬 시작 시 타이틀 BGM 재생
- [ ] 게임 시작 버튼 클릭 → 페이드아웃 → 게임 씬 로드 → 페이드인
- [ ] 설정 버튼 클릭 → 설정 패널 열림 (추후 구현)
- [ ] 종료 버튼 클릭 → 게임 종료

### 게임 씬
- [ ] 씬 시작 시 게임 BGM 재생
- [ ] ESC 키 → 게임 일시정지 + 일시정지 패널 표시
- [ ] 일시정지 중 BGM 일시정지
- [ ] 계속하기 버튼 → 게임 재개 + BGM 재개
- [ ] 설정 버튼 → 설정 패널 열림
- [ ] 설정 패널에서 볼륨 조절 가능
- [ ] 타이틀로 버튼 → 타이틀 씬 이동 (페이드 효과 포함)

### 오디오
- [ ] 씬 전환 시 BGM 페이드 전환
- [ ] 볼륨 설정 저장 및 로드
- [ ] 효과음 테스트 버튼 작동

---

## 추가 작업 (나중에)

### 스토리 씬 추가 시
1. 03_StoryScene.unity 생성
2. SceneController Prefab 배치
3. SceneBGMPlayer 배치
4. FadePanel 추가
5. AudioManager에 Story BGM 할당
6. TitleUIManager에서 "Use Story Scene" 체크
7. Build Settings에 씬 추가

### 추가 UI 기능
- 게임 오버 패널
- 스테이지 선택 화면
- 로딩 화면
- 대화 시스템

---

## 문제 해결

### BGM이 재생되지 않을 때
1. AudioManager가 씬에 존재하는지 확인
2. AudioClip이 할당되었는지 확인
3. AudioSource의 Volume이 0이 아닌지 확인
4. SceneBGMPlayer가 활성화되어 있는지 확인

### 씬 전환이 안 될 때
1. SceneController가 DontDestroyOnLoad인지 확인
2. Build Settings에 씬이 추가되었는지 확인
3. 씬 이름이 정확한지 확인

### 일시정지가 안 될 때
1. GamePauseManager가 활성화되어 있는지 확인
2. PausePanel과 버튼들이 올바르게 할당되었는지 확인
3. 다른 스크립트가 Input을 가로채고 있지 않은지 확인

### 페이드 효과가 안 보일 때
1. FadePanel이 Canvas의 최상단에 있는지 확인
2. CanvasGroup 컴포넌트가 있는지 확인
3. Image Color의 Alpha가 255인지 확인
4. Canvas Render Mode 확인 (Screen Space - Overlay)

---

## 파일 구조

```
Assets/
├── Scenes/
│   ├── 01_TitleScene.unity
│   ├── 02_MainGame.unity
│   └── 03_StoryScene.unity (나중에)
├── Scripts/
│   └── Systems/
│       ├── UI/
│       │   ├── TitleUIManager.cs
│       │   ├── GamePauseManagerUpdated.cs
│       │   └── SettingsPanel.cs
│       ├── Audio/
│       │   ├── AudioManager.cs
│       │   └── SceneBGMPlayer.cs
│       ├── Scene/
│       │   └── SceneController.cs (기존)
│       └── Utilities/
│           └── FadePanel.cs (기존)
└── UISystem/
    └── Audio/
        ├── Title_BGM.mp3
        ├── Game_BGM.mp3
        └── Story_BGM.mp3 (나중에)
```

---

## 마무리

모든 설정이 완료되면:
1. 타이틀 씬에서 플레이 테스트
2. 게임 시작 버튼으로 게임 씬 이동 확인
3. ESC 키로 일시정지 확인
4. 볼륨 조절 확인
5. 타이틀로 돌아가기 확인

문제가 있으면 위의 체크리스트와 문제 해결 섹션을 참고하세요!
