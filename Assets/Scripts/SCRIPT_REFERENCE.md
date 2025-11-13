# Dream Jump UI 시스템 - 스크립트 레퍼런스

## 📚 스크립트 개요

### UI 시스템
1. **TitleUIManager** - 타이틀 화면 버튼 관리
2. **GamePauseManagerUpdated** - 게임 일시정지 및 설정
3. **SettingsPanel** - 볼륨 설정 UI

### 오디오 시스템
4. **AudioManager** - BGM/SFX 통합 관리
5. **SceneBGMPlayer** - 씬별 BGM 자동 재생

---

## 1. TitleUIManager

### 용도
타이틀 화면의 버튼들을 관리하고 씬 전환을 처리합니다.

### Inspector 설정
```
UI Buttons:
- Start Button: 게임 시작 버튼
- Settings Button: 설정 버튼
- Quit Button: 종료 버튼

Scene Settings:
- Game Scene Name: "02_MainGame"
- Story Scene Name: "03_StoryScene"

Settings:
- Use Story Scene: false (스토리 씬 사용 여부)
```

### 주요 기능
- **게임 시작**: 게임 씬 또는 스토리 씬으로 전환 (페이드 효과 포함)
- **설정**: 설정 패널 열기 (추후 구현)
- **종료**: 게임 종료 (에디터에서는 플레이 모드 종료)

### 사용 예시
```csharp
// 코드에서 스토리 씬 사용 여부 변경
TitleUIManager titleUI = FindObjectOfType<TitleUIManager>();
titleUI.SetUseStoryScene(true);
```

---

## 2. GamePauseManagerUpdated

### 용도
게임 중 ESC 키로 일시정지하고 설정을 변경할 수 있게 합니다.

### Inspector 설정
```
UI Panels:
- Pause Panel: 일시정지 패널 GameObject
- Settings Panel: 설정 패널 GameObject

Pause Panel Buttons:
- Resume Button: 계속하기 버튼
- Settings Button: 설정 버튼
- Title Button: 타이틀로 버튼
- Quit Button: 종료 버튼

Settings Panel Buttons:
- Close Settings Button: 설정 닫기 버튼

Settings:
- Title Scene Name: "01_TitleScene"
- Pause Key: Escape
```

### 주요 기능
- **일시정지 (ESC)**: Time.timeScale = 0, BGM 일시정지
- **재개**: Time.timeScale = 1, BGM 재개
- **설정**: 일시정지 중 설정 패널 열기
- **타이틀로**: 타이틀 씬으로 이동 (페이드 효과)

### 사용 예시
```csharp
// 코드에서 일시정지
GamePauseManager pauseManager = FindObjectOfType<GamePauseManager>();
pauseManager.Pause();

// 일시정지 상태 확인
if (pauseManager.IsPaused())
{
    // 일시정지 중
}
```

---

## 3. SettingsPanel

### 용도
BGM과 SFX 볼륨을 조절하는 UI를 제공합니다.

### Inspector 설정
```
Volume Sliders:
- BGM Slider: BGM 볼륨 슬라이더 (0-1)
- SFX Slider: SFX 볼륨 슬라이더 (0-1)

Volume Text:
- BGM Volume Text: BGM 볼륨 표시 텍스트 (예: "70%")
- SFX Volume Text: SFX 볼륨 표시 텍스트 (예: "100%")

Test Button:
- SFX Test Button: 효과음 테스트 버튼
- Test SFX: 테스트용 효과음 AudioClip
```

### 주요 기능
- **볼륨 조절**: 슬라이더로 BGM/SFX 볼륨 변경
- **자동 저장**: PlayerPrefs로 볼륨 설정 저장
- **자동 로드**: 시작 시 저장된 볼륨 로드
- **효과음 테스트**: 버튼으로 효과음 재생 테스트

### 슬라이더 설정
```
Slider 컴포넌트:
- Min Value: 0
- Max Value: 1
- Whole Numbers: false
- Value: 0.7 (BGM) / 1.0 (SFX)
```

---

## 4. AudioManager

### 용도
게임 전체의 배경음악과 효과음을 관리하는 싱글톤 매니저입니다.

### Inspector 설정
```
Audio Sources:
- BGM Source: (자동 생성됨, 수동 할당 가능)
- SFX Source: (자동 생성됨, 수동 할당 가능)

Background Music:
- Title BGM: 타이틀 화면 배경음악
- Game BGM: 게임 플레이 배경음악
- Story BGM: 스토리 씬 배경음악

Volume Settings:
- BGM Volume: 0.7 (0-1 범위)
- SFX Volume: 1.0 (0-1 범위)

Fade Settings:
- Fade Duration: 1.0 (초)
```

### 주요 기능

#### BGM 제어
```csharp
// 특정 AudioClip 재생
AudioManager.Instance.PlayBGM(bgmClip, fade: true);

// 씬별 BGM 자동 재생
AudioManager.Instance.PlayBGMForScene("01_TitleScene", fade: true);

// BGM 정지
AudioManager.Instance.StopBGM(fade: true);

// BGM 일시정지/재개
AudioManager.Instance.PauseBGM();
AudioManager.Instance.UnpauseBGM();
```

#### SFX 제어
```csharp
// AudioClip으로 재생
AudioManager.Instance.PlaySFX(jumpSound);

// 등록된 효과음 재생
AudioManager.Instance.RegisterSFX("jump", jumpSound);
AudioManager.Instance.PlaySFX("jump");
```

#### 볼륨 제어
```csharp
// 볼륨 설정 (0-1)
AudioManager.Instance.SetBGMVolume(0.5f);
AudioManager.Instance.SetSFXVolume(0.8f);

// 볼륨 가져오기
float bgmVol = AudioManager.Instance.GetBGMVolume();
float sfxVol = AudioManager.Instance.GetSFXVolume();

// 저장된 볼륨 로드
AudioManager.Instance.LoadVolumeSettings();
```

### 특징
- **싱글톤**: 게임 전체에서 하나만 존재
- **DontDestroyOnLoad**: 씬 전환 시에도 유지
- **자동 페이드**: BGM 전환 시 부드러운 페이드 효과
- **설정 저장**: PlayerPrefs로 볼륨 설정 자동 저장

---

## 5. SceneBGMPlayer

### 용도
씬이 로드될 때 자동으로 해당 씬의 BGM을 재생합니다.

### Inspector 설정
```
Settings:
- Play On Start: true (시작 시 자동 재생)
- Fade In: true (페이드 인 효과)
```

### 작동 방식
1. 씬 시작 시 `Start()` 호출
2. `SceneManager.GetActiveScene().name`으로 현재 씬 이름 확인
3. `AudioManager.Instance.PlayBGMForScene()` 호출
4. AudioManager가 씬 이름에 맞는 BGM 자동 재생

### 지원하는 씬
- "01_TitleScene" → Title BGM
- "02_MainGame" → Game BGM
- "03_StoryScene" → Story BGM

### 사용 예시
```csharp
// 수동으로 현재 씬의 BGM 재생
SceneBGMPlayer bgmPlayer = FindObjectOfType<SceneBGMPlayer>();
bgmPlayer.PlayBGMForCurrentScene();
```

---

## 🔗 스크립트 간 연결

### 씬 전환 흐름
```
TitleUIManager
    ↓ (StartButton 클릭)
SceneController.LoadScene()
    ↓ (페이드 아웃)
    ↓ (씬 로드)
SceneBGMPlayer.Start()
    ↓ (BGM 재생)
AudioManager.PlayBGMForScene()
    ↓ (페이드 인)
게임 씬 활성화
```

### 일시정지 흐름
```
ESC 키 입력
    ↓
GamePauseManager.Pause()
    ↓ (Time.timeScale = 0)
    ↓ (PausePanel 활성화)
AudioManager.PauseBGM()
    ↓
일시정지 상태
```

### 설정 변경 흐름
```
SettingsPanel (Slider 변경)
    ↓
OnBGMVolumeChanged()
    ↓
AudioManager.SetBGMVolume()
    ↓
PlayerPrefs.SetFloat() (자동 저장)
```

---

## 📝 코드 예시

### 게임 내에서 효과음 재생
```csharp
using PlatformerGame.Systems.Audio;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    
    private void Start()
    {
        // 효과음 등록 (한 번만)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RegisterSFX("jump", jumpSound);
            AudioManager.Instance.RegisterSFX("land", landSound);
        }
    }
    
    private void Jump()
    {
        // 효과음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("jump");
        }
    }
}
```

### 커스텀 씬 전환
```csharp
using PlatformerGame.Systems.Scene;

public class StageSelector : MonoBehaviour
{
    public void LoadStage(string stageName)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadScene(stageName);
        }
    }
}
```

### 일시정지 상태에서 특정 동작 막기
```csharp
using PlatformerGame.Systems.UI;

public class PlayerController : MonoBehaviour
{
    private GamePauseManager pauseManager;
    
    private void Start()
    {
        pauseManager = FindObjectOfType<GamePauseManager>();
    }
    
    private void Update()
    {
        // 일시정지 중에는 입력 무시
        if (pauseManager != null && pauseManager.IsPaused())
        {
            return;
        }
        
        // 일반 입력 처리
        HandleInput();
    }
}
```

---

## 🎯 베스트 프랙티스

### 1. AudioManager 접근
```csharp
// ✅ 좋은 예: null 체크
if (AudioManager.Instance != null)
{
    AudioManager.Instance.PlaySFX(sound);
}

// ❌ 나쁜 예: null 체크 없음
AudioManager.Instance.PlaySFX(sound); // NullReferenceException 가능
```

### 2. 씬 전환
```csharp
// ✅ 좋은 예: SceneController 사용 (페이드 효과)
SceneController.Instance.LoadScene("02_MainGame");

// ❌ 나쁜 예: 직접 로드 (페이드 없음)
SceneManager.LoadScene("02_MainGame");
```

### 3. 일시정지 처리
```csharp
// ✅ 좋은 예: Time.timeScale 고려
void Update()
{
    if (Time.timeScale == 0) return; // 일시정지 중 무시
    
    // 게임 로직
}

// ✅ 더 좋은 예: GamePauseManager 활용
void Update()
{
    if (pauseManager != null && pauseManager.IsPaused()) return;
    
    // 게임 로직
}
```

---

## 🐛 디버깅 팁

### 로그 메시지
모든 스크립트는 주요 동작 시 Debug.Log를 출력합니다:
- `[TitleUIManager]` - 타이틀 UI 동작
- `[GamePauseManager]` - 일시정지 관련
- `[AudioManager]` - 오디오 관련
- `[SceneController]` - 씬 전환
- `[SceneBGMPlayer]` - BGM 재생

### 확인 사항
1. AudioManager.Instance가 null인지 확인
2. SceneController.Instance가 null인지 확인
3. Build Settings에 씬이 추가되었는지 확인
4. AudioClip이 할당되었는지 확인
5. UI 버튼이 올바르게 할당되었는지 확인

---

## 🔧 확장 가능성

### 새로운 씬 추가
AudioManager.cs의 `PlayBGMForScene()` 메서드에 case 추가:
```csharp
case "04_BossStage":
    clipToPlay = bossBGM;
    break;
```

### 새로운 효과음 추가
```csharp
// Start에서 등록
AudioManager.Instance.RegisterSFX("explosion", explosionSound);

// 사용
AudioManager.Instance.PlaySFX("explosion");
```

### 커스텀 일시정지 동작
GamePauseManager 상속:
```csharp
public class CustomPauseManager : GamePauseManager
{
    public override void Pause()
    {
        base.Pause();
        // 추가 동작
    }
}
```

---

모든 스크립트는 네임스페이스를 사용합니다:
- `PlatformerGame.Systems.UI`
- `PlatformerGame.Systems.Audio`
- `PlatformerGame.Systems.Scene`
- `PlatformerGame.Utilities`
