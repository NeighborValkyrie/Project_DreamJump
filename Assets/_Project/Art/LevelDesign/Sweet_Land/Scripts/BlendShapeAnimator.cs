using UnityEngine;
using System.Collections;

namespace ithappy
{
    public class BlendShapeAnimator : MonoBehaviour
    {
        private SkinnedMeshRenderer skinnedMeshRenderer;
        public int blendShapeIndex = 0;
        public float maxBlendShapeValue = 100f;
        public float animationSpeed = 1f;
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private bool isAnimating = false; // 중복 실행 방지

        private void Awake()
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            }
            if (skinnedMeshRenderer == null)
            {
                Debug.LogError("SkinnedMeshRenderer not found.");
            }
        }

        // [변경점]
        // Start()와 무한루프(while(true))를 제거했습니다.
        // 대신, 다른 스크립트에서 호출할 수 있는 public 함수를 만듭니다.

        /// <summary>
        /// 블렌드 셰이프 애니메이션을 1회 재생합니다.
        /// </summary>
        public void PlayBounceAnimation()
        {
            if (isAnimating || skinnedMeshRenderer == null)
            {
                return; // 애니메이션 중이거나 렌더러가 없으면 실행 안 함
            }
            StartCoroutine(AnimateBounce());
        }

        private IEnumerator AnimateBounce()
        {
            isAnimating = true;

            // 0 -> Max (찌그러짐)
            yield return AnimateToValue(maxBlendShapeValue);
            
            // Max -> 0 (복귀)
            yield return AnimateToValue(0f);

            isAnimating = false;
        }

        // AnimateToValue 함수는 원본 코드를 그대로 사용합니다.
        private IEnumerator AnimateToValue(float targetValue)
        {
            float elapsedTime = 0f;
            float initialBlendShapeValue = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
            float duration = 1f / animationSpeed;

            while (elapsedTime < duration)
            {
                float normalizedTime = elapsedTime / duration;
                float curveValue = animationCurve.Evaluate(normalizedTime);
                float newBlendShapeValue = Mathf.Lerp(initialBlendShapeValue, targetValue, curveValue);
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, newBlendShapeValue);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, targetValue);
        }

#if UNITY_EDITOR
        // (OnValidate는 그대로 둡니다)
#endif
    }
}