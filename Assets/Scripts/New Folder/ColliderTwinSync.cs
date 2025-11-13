// ColliderTwinSync.cs
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
[DisallowMultipleComponent]
public class ColliderTwinSync : MonoBehaviour
{
    [Header("=== [TUNE] 기본 설정 ===")]
    [SerializeField] Collider source;                 // [변경가능] 원본(비-트리거) 콜라이더
    [SerializeField] string childName = "__TriggerTwin"; // [변경가능] 자식 트리거 이름
    [SerializeField] bool keepSyncedInEdit = true;    // [변경가능] 에디터에서 자동 동기화

    [Header("=== [TUNE] 옵션 ===")]
    [SerializeField] bool setChildLayer = false;      // [변경가능] 자식 레이어 강제 지정
    [SerializeField] string childLayerName = "Triggers"; // [변경가능] 레이어명
    [SerializeField] bool meshTriggerConvex = true;   // [변경가능] MeshCollider 트리거는 convex 필수
    [SerializeField] bool showGizmo = true;           // [변경가능] 기즈모 표시

    Collider childTrigger;
    Transform childTf;

    void Reset()
    {
        source = GetComponent<Collider>();
    }

    void OnEnable()
    {
        EnsureRefs();
        CreateOrRefresh();
    }

    void OnValidate()
    {
        EnsureRefs();
        CreateOrRefresh();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying && keepSyncedInEdit)
            CreateOrRefresh();
    }
#endif

    void EnsureRefs()
    {
        if (!source) source = GetComponent<Collider>();

        // 자식 찾기/생성
        if (!childTf)
        {
            var t = transform.Find(childName);
            if (!t)
            {
                var go = new GameObject(childName);
                childTf = go.transform;
                childTf.SetParent(transform, false);
            }
            else childTf = t;
        }

        // 자식 트랜스폼 정렬
        childTf.localPosition = Vector3.zero;
        childTf.localRotation = Quaternion.identity;
        childTf.localScale = Vector3.one;

        if (setChildLayer)
        {
            int layer = LayerMask.NameToLayer(childLayerName);
            if (layer >= 0) childTf.gameObject.layer = layer;
        }

        // 자식 콜라이더 타입 맞춰 붙이기
        if (source is BoxCollider)
            childTrigger = GetOrSwap<Collider, BoxCollider>();
        else if (source is SphereCollider)
            childTrigger = GetOrSwap<Collider, SphereCollider>();
        else if (source is CapsuleCollider)
            childTrigger = GetOrSwap<Collider, CapsuleCollider>();
        else if (source is MeshCollider)
            childTrigger = GetOrSwap<Collider, MeshCollider>();
        else
            childTrigger = GetOrSwap<Collider, BoxCollider>(); // fallback
    }

    TOut GetOrSwap<T, TOut>() where TOut : Component
    {
        var cur = childTf.GetComponent<TOut>();
        if (!cur)
        {
            // 기존 다른 콜라이더 제거
            foreach (var c in childTf.GetComponents<Collider>())
                DestroyImmediate(c);
            cur = childTf.gameObject.AddComponent<TOut>();
        }
        return cur;
    }

    [ContextMenu("Create / Refresh Trigger Twin")]
    public void CreateOrRefresh()
    {
        if (!source || !childTf) return;

        // 항상 트리거로
        if (childTrigger) childTrigger.isTrigger = true;

        // 타입별 파라미터 복사
        if (source is BoxCollider srcB && childTrigger is BoxCollider dstB)
        {
            dstB.center = srcB.center;
            dstB.size = srcB.size;
            dstB.contactOffset = srcB.contactOffset;
        }
        else if (source is SphereCollider srcS && childTrigger is SphereCollider dstS)
        {
            dstS.center = srcS.center;
            dstS.radius = srcS.radius;
            dstS.contactOffset = srcS.contactOffset;
        }
        else if (source is CapsuleCollider srcC && childTrigger is CapsuleCollider dstC)
        {
            dstC.center = srcC.center;
            dstC.radius = srcC.radius;
            dstC.height = srcC.height;
            dstC.direction = srcC.direction;
            dstC.contactOffset = srcC.contactOffset;
        }
        else if (source is MeshCollider srcM && childTrigger is MeshCollider dstM)
        {
            dstM.sharedMesh = srcM.sharedMesh;
            dstM.convex = meshTriggerConvex;
            dstM.inflateMesh = srcM.inflateMesh;
            dstM.cookingOptions = srcM.cookingOptions;
            // isTrigger는 위에서 이미 true로 설정
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showGizmo || !source) return;
        Gizmos.color = new Color(1, 0.92f, 0.2f, 0.25f);
        var b = source.bounds;
        Gizmos.DrawWireCube(b.center, b.size);
    }
#endif
}
