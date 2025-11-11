using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Effects")]
    public float hoverScale = 1.1f;
    public float clickScale = 0.95f;
    public float animationSpeed = 8f;
    
    [Header("Sound Effects")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    private Vector3 originalScale;
    private Vector3 targetScale;
    private AudioSource audioSource;
    
    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        // AudioSource 컴포넌트 추가 (선택사항)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (hoverSound != null || clickSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
    
    void Update()
    {
        // 부드러운 스케일 애니메이션
        if (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
        
        // 호버 사운드 재생
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * clickScale;
        
        // 클릭 사운드 재생
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }
}
