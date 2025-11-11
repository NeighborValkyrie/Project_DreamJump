using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerGame.Utilities
{
    /// <summary>
    /// UI 페이드 인/아웃 효과
    /// v7.0: 새로 추가됨
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FadePanel : MonoBehaviour
    {
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                Debug.LogError("[FadePanel] CanvasGroup 컴포넌트가 없습니다!");
                enabled = false;
            }
        }

        private void Start()
        {
            // 초기 상태: 완전히 투명
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// 페이드 아웃 (검게 변함)
        /// </summary>
        public IEnumerator FadeOut(float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// 페이드 인 (투명해짐)
        /// </summary>
        public IEnumerator FadeIn(float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        /// <summary>
        /// 즉시 페이드 상태 설정
        /// </summary>
        public void SetAlpha(float alpha)
        {
            canvasGroup.alpha = Mathf.Clamp01(alpha);
        }
    }
}