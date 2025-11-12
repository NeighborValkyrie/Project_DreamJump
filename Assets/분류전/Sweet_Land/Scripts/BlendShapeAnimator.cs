using UnityEngine;
using System.Collections;

namespace ithappy
{
    public class BlendShapeAnimator : MonoBehaviour
    {
        private SkinnedMeshRenderer skinnedMeshRenderer;
        
        // [Header("Bounce Animation (밟았을 때)")]
        [Header("Bounce Animation (밟았을 때)")]
        public int blendShapeIndex = 0;
        public float maxBlendShapeValue = 100f; // 밟았을 때의 강도
        public float animationSpeed = 1f;     // 밟았을 때의 속도
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        // [Header("Idle Animation (평소에)")]
        [Header("Idle Animation (평소에)")]
        public bool playIdleAnimation = true; // 평소에 흔들거릴지 여부
        public float idleMaxValue = 20f;      // 평소 흔들림의 강도
        public float idleSpeed = 0.5f;        // 평소 흔들림의 속도
        public float idleDelay = 0.1f;        // 흔들림 사이의 딜레이

        private bool isAnimating = false; // 현재 '바운스' 애니메이션 중인지 확인
        private Coroutine idleCoroutine;  // '대기' 애니메이션 코루틴을 저장할 변수

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

        private void Start()
        {
            // 게임이 시작되면 '대기' 애니메이션을 시작합니다.
            if (playIdleAnimation && skinnedMeshRenderer != null)
            {
                idleCoroutine = StartCoroutine(IdleLoop());
            }
        }

        /// <summary>
        /// (JellyPlatform 스크립트 등이 호출) 
        /// 밟았을 때 '바운스' 애니메이션을 1회 재생합니다.
        /// </summary>
        public void PlayBounceAnimation()
        {
            if (isAnimating || skinnedMeshRenderer == null)
            {
                return; // 이미 바운스 중이면 중복 실행 안 함
            }

            // 1. '대기' 애니메이션을 하고 있었다면 즉시 중지시킵니다.
            if (idleCoroutine != null)
            {
                StopCoroutine(idleCoroutine);
                idleCoroutine = null;
            }

            // 2. '바운스' 애니메이션을 시작합니다.
            StartCoroutine(AnimateBounce());
        }

        // '바운스' 애니메이션 코루틴 (밟혔을 때)
        private IEnumerator AnimateBounce()
        {
            isAnimating = true; // 바운스 시작

            yield return AnimateToValue(maxBlendShapeValue, animationSpeed); // 밟는 속도
            yield return AnimateToValue(0f, animationSpeed); // 튕겨나는 속도

            isAnimating = false; // 바운스 끝

            // 3. '바운스'가 끝났으니 다시 '대기' 애니메이션을 시작합니다.
            if (playIdleAnimation)
            {
                idleCoroutine = StartCoroutine(IdleLoop());
            }
        }

        // '대기' 애니메이션 코루틴 (평소)
        private IEnumerator IdleLoop()
        {
            while (true)
            {
                // 평소 속도로 idleMaxValue까지 이동
                yield return AnimateToValue(idleMaxValue, idleSpeed);
                
                // 평소 속도로 0f까지 복귀
                yield return AnimateToValue(0f, idleSpeed);

                // 잠깐 대기
                yield return new WaitForSeconds(idleDelay);
            }
        }

        // [변경]
        // AnimateToValue가 'speed'를 파라미터로 받도록 수정
        // (바운스 속도와 대기 속도를 구분하기 위함)
        private IEnumerator AnimateToValue(float targetValue, float speed)
        {
            float elapsedTime = 0f;
            float initialBlendShapeValue = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);

            // 속도가 0 이하면 오류가 날 수 있으므로 방지
            float duration = (speed > 0f) ? (1f / speed) : 0f;

            if (duration == 0f)
            {
                // 속도가 0이면 즉시 값 설정 후 종료
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, targetValue);
                yield break;
            }

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