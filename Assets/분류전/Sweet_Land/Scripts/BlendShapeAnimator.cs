using UnityEngine;
using System.Collections;

namespace ithappy
{
    [DisallowMultipleComponent]
    public class BlendShapeAnimator : MonoBehaviour
    {
        [Header("Target")]
        public SkinnedMeshRenderer target;               // [변경가능] 직접 지정 권장

        [Header("Bounce Animation (밟았을 때)")]
        public int blendShapeIndex = 0;                  // [변경가능]
        public float maxBlendShapeValue = 100f;          // [변경가능]
        public float animationSpeed = 1f;                // [변경가능]
        public AnimationCurve animationCurve =           // [변경가능]
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Idle Animation (평소에)")]
        public bool playIdleAnimation = true;            // [변경가능]
        public float idleMaxValue = 20f;                 // [변경가능]
        public float idleSpeed = 0.5f;                   // [변경가능]
        public float idleDelay = 0.1f;                   // [변경가능]

        [Header("Safety")]
        public bool autoDisableIfMissing = true;         // [변경가능] SMR/블렌드셰이프 없으면 자동 비활성화
        public bool warnOnceIfMissing = true;            // [변경가능] 경고 1회만 표시

        bool hasRenderer;
        bool hasBlendShapes;
        bool isAnimating;
        Coroutine idleCoroutine;

        void Awake()
        {
            // 타겟 자동 탐색 (직접 할당이 최우선)
            if (!target) target = GetComponent<SkinnedMeshRenderer>();
            if (!target) target = GetComponentInChildren<SkinnedMeshRenderer>();
            hasRenderer = target != null;

            // 블렌드셰이프 존재 여부 확인
            hasBlendShapes = hasRenderer && target.sharedMesh && target.sharedMesh.blendShapeCount > 0;
            if (hasBlendShapes)
            {
                // 인덱스 범위 클램프
                blendShapeIndex = Mathf.Clamp(blendShapeIndex, 0, target.sharedMesh.blendShapeCount - 1);
            }

            // 없으면 자동 비활성화/경고 처리
            if (!hasRenderer || !hasBlendShapes)
            {
                if (warnOnceIfMissing)
                {
                    string why = !hasRenderer ? "SkinnedMeshRenderer 없음" : "블렌드셰이프 없음";
                    Debug.LogWarning($"[BlendShapeAnimator] 비활성화: {why}. (오브젝트: {name})");
                }
                if (autoDisableIfMissing) enabled = false;
            }
        }

        void OnEnable()
        {
            // 안전 가드
            if (!hasRenderer || !hasBlendShapes || target == null)
            {
                enabled = false;
                return;
            }

            if (playIdleAnimation)
            {
                // 시작 시 아이들 루프
                if (idleCoroutine != null) StopCoroutine(idleCoroutine);
                idleCoroutine = StartCoroutine(IdleLoop());
            }
        }

        void OnDisable()
        {
            if (idleCoroutine != null) { StopCoroutine(idleCoroutine); idleCoroutine = null; }
            isAnimating = false;
        }

        // 외부(예: JellyPlatform)가 호출: 밟혔을 때 1회 바운스
        public void PlayBounceAnimation()
        {
            if (!enabled || !hasRenderer || !hasBlendShapes || target == null) return;
            if (isAnimating) return;

            if (idleCoroutine != null)
            {
                StopCoroutine(idleCoroutine);
                idleCoroutine = null;
            }
            StartCoroutine(AnimateBounce());
        }

        IEnumerator AnimateBounce()
        {
            isAnimating = true;
            yield return AnimateToValue(maxBlendShapeValue, animationSpeed);
            yield return AnimateToValue(0f, animationSpeed);
            isAnimating = false;

            if (playIdleAnimation && enabled && target != null)
            {
                idleCoroutine = StartCoroutine(IdleLoop());
            }
        }

        IEnumerator IdleLoop()
        {
            while (enabled && target != null)
            {
                yield return AnimateToValue(idleMaxValue, idleSpeed);
                if (!enabled || target == null) yield break;

                yield return AnimateToValue(0f, idleSpeed);
                if (!enabled || target == null) yield break;

                yield return new WaitForSeconds(idleDelay);
            }
        }

        IEnumerator AnimateToValue(float targetValue, float speed)
        {
            if (target == null) yield break; // 런타임에 제거/파괴된 경우 안전 탈출

            float duration = (speed > 0f) ? (1f / speed) : 0f;
            if (duration <= 0f)
            {
                SafeSetWeight(targetValue);
                yield break;
            }

            float elapsed = 0f;
            float startValue = SafeGetWeight();

            while (elapsed < duration && enabled && target != null)
            {
                float t = elapsed / duration;
                float k = animationCurve != null ? animationCurve.Evaluate(t) : t;
                SafeSetWeight(Mathf.Lerp(startValue, targetValue, k));
                elapsed += Time.deltaTime;
                yield return null;
            }
            SafeSetWeight(targetValue);
        }

        float SafeGetWeight()
        {
            if (target == null) return 0f;
            int count = target.sharedMesh ? target.sharedMesh.blendShapeCount : 0;
            if (count == 0) return 0f;
            int idx = Mathf.Clamp(blendShapeIndex, 0, count - 1);
            return target.GetBlendShapeWeight(idx);
        }

        void SafeSetWeight(float value)
        {
            if (target == null) return;
            int count = target.sharedMesh ? target.sharedMesh.blendShapeCount : 0;
            if (count == 0) return;
            int idx = Mathf.Clamp(blendShapeIndex, 0, count - 1);
            target.SetBlendShapeWeight(idx, Mathf.Clamp(value, 0f, 100f));
        }
    }
}
