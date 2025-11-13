using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 30초 분량 + 전체 페이드인/아웃 효과가 추가된 버전
public class TitleScreenAnimator : MonoBehaviour
{
    [Header("전체 페이드 효과")]
    [Tooltip("화면 전체 페이드인/아웃에 사용할 이미지(fade_img)의 CanvasGroup")]
    public CanvasGroup globalFadeCanvasGroup; 

    [Header("연결할 UI 요소")]
    [Tooltip("배경 이미지의 CanvasGroup")]
    public CanvasGroup backgroundCanvasGroup;
    
    [Tooltip("캐릭터 이미지의 CanvasGroup")]
    public CanvasGroup characterCanvasGroup;
    
    [Tooltip("타이틀 로고의 CanvasGroup")]
    public CanvasGroup titleLogoCanvasGroup;

    [Header("로고 바운스 효과용")]
    [Tooltip("타이틀 로고의 RectTransform")]
    public RectTransform titleLogoRect;

    [Header("파티클 효과")]
    [Tooltip("캐릭터 등장 시 재생할 파티클 시스템")]
    public ParticleSystem characterParticles;

    // --- 애니메이션 타이밍 및 값 (30초 버전) ---

    [Header("전체 페이드 타이밍")]
    [Tooltip("시작 시, 검은 화면에서 밝아지는 시간")]
    public float globalFadeInDuration = 1.0f; // 0.0s -> 2.0s
    
    [Tooltip("종료 시, 검은 화면으로 어두워지기 시작하는 시간")]
    public float globalFadeOutStartTime = 28.0f; // 30초 중 28초 시점

    [Tooltip("검은 화면으로 어두워지는 데 걸리는 시간")]
    public float globalFadeOutDuration = 2.0f; // 28.0s -> 30.0s

    [Header("콘텐츠 애니메이션 타이밍")]
    // 배경
    private float bgFadeDuration = 0.0f; // 0.0s -> 5.0s

    // 캐릭터
    private float characterStartTime = 4.0f; // 4.0s에 시작
    private float characterFadeDuration = 2.0f; // 4.0s -> 7.0s

    // 로고
    private float logoStartTime = 10.0f; // 7.0s에 시작
    private float logoFadeDuration = 3.0f; // 7.0s -> 10.0s
    private float logoBounceDuration1 = 2.0f; // 7.0s -> 9.0s
    private float logoBounceDuration2 = 1.0f; // 9.0s -> 10.0s

    private Vector3 logoStartScale = new Vector3(0.5f, 0.5f, 1f);
    private Vector3 logoMidScale = new Vector3(1.1f, 1.1f, 1f);
    private Vector3 logoEndScale = new Vector3(1.0f, 1.0f, 1f);


    void Start()
    {
        InitializeElements();
        
        // 메인 콘텐츠 애니메이션 시작
        StartCoroutine(PlayTitleSequence());
        
        // 전체 페이드인/아웃 애니메이션 시작
        StartCoroutine(RunGlobalFade());
    }

    void InitializeElements()
    {
        // 모든 콘텐츠는 투명하게 시작
        if(characterCanvasGroup != null) characterCanvasGroup.alpha = 0f;
        if(titleLogoCanvasGroup != null) titleLogoCanvasGroup.alpha = 0f;
        
        // 로고 크기 초기화
        if(titleLogoRect != null) titleLogoRect.localScale = logoStartScale;
        
        // 파티클 정지
        if(characterParticles != null) characterParticles.Stop();

        // ★★★ 추가된 부분 ★★★
        // globalFadeCanvasGroup는 불투명하게(검은 화면) 시작
        if(globalFadeCanvasGroup != null) globalFadeCanvasGroup.alpha = 1f;
    }

    // 전체 화면 페이드인/아웃을 관리하는 코루틴
    IEnumerator RunGlobalFade()
    {
        // 1. Global Fade In (시작 시 검은 화면 -> 투명하게)
        if(globalFadeCanvasGroup != null)
        {
            StartCoroutine(FadeCanvas(globalFadeCanvasGroup, 0f, globalFadeInDuration));
        }

        // 2. Fade Out 시작 시간까지 대기
        yield return new WaitForSeconds(globalFadeOutStartTime);

        // 3. Global Fade Out (투명 -> 검은 화면)
        if(globalFadeCanvasGroup != null)
        {
            StartCoroutine(FadeCanvas(globalFadeCanvasGroup, 1f, globalFadeOutDuration));
        }
    }

    // 배경, 캐릭터, 로고의 등장을 관리하는 코루틴 (기존과 동일)
    IEnumerator PlayTitleSequence()
    {
        // 0.0초: 배경 페이드인 시작 (5초간)
        if(backgroundCanvasGroup != null)
        {
            StartCoroutine(FadeCanvas(backgroundCanvasGroup, 1f, bgFadeDuration));
        }

        // 4.0초까지 대기
        yield return new WaitForSeconds(characterStartTime);

        // 4.0초: 캐릭터 파티클 재생 및 페이드인 시작 (3초간)
        if(characterParticles != null)
        {
            characterParticles.Play();
        }
        if(characterCanvasGroup != null)
        {
            StartCoroutine(FadeCanvas(characterCanvasGroup, 1f, characterFadeDuration));
        }

        // 7.0초까지 대기
        yield return new WaitForSeconds(logoStartTime - characterStartTime);

        // 7.0초: 로고 페이드인 및 바운스 시작 (3초간)
        if(titleLogoCanvasGroup != null)
        {
            StartCoroutine(FadeCanvas(titleLogoCanvasGroup, 1f, logoFadeDuration));
        }
        if(titleLogoRect != null)
        {
            StartCoroutine(BounceLogo(titleLogoRect));
        }
    }

    // CanvasGroup 알파값 조절 코루틴 (기존과 동일)
    IEnumerator FadeCanvas(CanvasGroup cg, float targetAlpha, float duration)
    {
        float timer = 0f;
        float startAlpha = cg.alpha;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            yield return null; 
        }
        cg.alpha = targetAlpha; 
    }

    // 로고 바운스 코루틴 (기존과 동일)
    IEnumerator BounceLogo(RectTransform rect)
    {
        float timer = 0f;
        while (timer < logoBounceDuration1)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / logoBounceDuration1);
            rect.localScale = Vector3.Lerp(logoStartScale, logoMidScale, progress);
            yield return null;
        }
        timer = 0f; 
        while (timer < logoBounceDuration2)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / logoBounceDuration2);
            rect.localScale = Vector3.Lerp(logoMidScale, logoEndScale, progress);
            yield return null;
        }
        rect.localScale = logoEndScale; 
    }
}