using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(EnemyConeSight))]
public class EnemySightDebugCone : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] EnemyConeSight sight;
    [SerializeField] Transform eyesOverride;
    [SerializeField] Transform player;

    [Header("Visual")]
    [SerializeField, Range(8, 128)] int segments = 48;
    [SerializeField] Color normalColor = new Color(1, 1, 0, 0.18f);
    [SerializeField] Color alertColor = new Color(1, 0, 0, 0.30f);
    [SerializeField] Material overrideMaterial;
    [SerializeField] bool showCap = false;
    [SerializeField] bool drawAxis = true;

    [Header("Stabilize")]
    [SerializeField] bool smoothColor = true;
    [SerializeField] float colorLerpSpeed = 12f;   // 색 전환 속도
    [SerializeField] float stateHold = 0.10f;      // 히스테리시스(초)

    MeshFilter mf; MeshRenderer mr; Mesh coneMesh; Material runtimeMat;
    MaterialPropertyBlock mpb;

    // 이전 값 캐시(불필요한 재빌드 방지)
    float lastRange, lastAngle; int lastSegments; Transform lastEyes;
    bool lastShowCap;
    // 색 안정화
    bool lastAlert; float lastFlipTime; Color currentColor;

    void Reset() => sight = GetComponent<EnemyConeSight>();

    void OnEnable()
    {
        Ensure();
        ForceRebuild();
        InitColor();
        UpdateTransform(true);
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && runtimeMat) DestroyImmediate(runtimeMat);
        if (!Application.isPlaying && coneMesh) DestroyImmediate(coneMesh);
#endif
    }

    void Update()
    {
        Ensure();
        BuildIfChanged();
        UpdateTransform(false);
        UpdateColorStabilized();
    }

    // ---------- Ensure / Setup ----------
    void Ensure()
    {
        if (!sight) sight = GetComponent<EnemyConeSight>();

        var child = transform.Find("__ViewCone");
        GameObject go;
        if (!child)
        {
            go = new GameObject("__ViewCone");
            go.transform.SetParent(transform, false);
            go.hideFlags = HideFlags.DontSave;
            mf = go.AddComponent<MeshFilter>();
            mr = go.AddComponent<MeshRenderer>();
        }
        else
        {
            go = child.gameObject;
            mf = go.GetComponent<MeshFilter>() ?? go.AddComponent<MeshFilter>();
            mr = go.GetComponent<MeshRenderer>() ?? go.AddComponent<MeshRenderer>();
        }

        if (!coneMesh)
        {
            coneMesh = new Mesh { name = "ViewConeMesh" };
            coneMesh.MarkDynamic();
        }
        mf.sharedMesh = coneMesh;

        if (overrideMaterial) mr.sharedMaterial = overrideMaterial;
        else
        {
            if (!runtimeMat)
            {
                // 투명 Unlit 권장(URP: Unlit/Color 또는 Universal Render Pipeline/Lit(Transparent))
                runtimeMat = new Material(Shader.Find("Unlit/Color")) { name = "ViewConeRuntimeMat" };
                runtimeMat.renderQueue = 3000; // Transparent
            }
            mr.sharedMaterial = runtimeMat;
        }

        if (mpb == null) mpb = new MaterialPropertyBlock();

        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        mr.allowOcclusionWhenDynamic = false; // 동적 오클루전으로 깜빡임 방지
    }

    Transform Eyes()
    {
        if (eyesOverride) return eyesOverride;
        if (sight && sight.eyes) return sight.eyes;
        return transform;
    }

    void UpdateTransform(bool force)
    {
        var e = Eyes(); if (!e) return;
        var t = mf.transform;

        // 부모를 눈에 붙여 버리면 월드→로컬 변환 과정에서 오차 적음
        if (force || t.parent != e)
            t.SetParent(e, false);

        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    // ---------- Mesh Rebuild ----------
    void ForceRebuild()
    {
        lastRange = lastAngle = -999f;
        lastSegments = -999;
        lastEyes = null;
        lastShowCap = !showCap;
        BuildIfChanged();
    }

    void BuildIfChanged()
    {
        if (!sight || segments < 3) return;

        var e = Eyes(); if (!e) return;
        float range = sight.viewRange;
        float angle = sight.viewAngle;

        bool need =
            !Mathf.Approximately(range, lastRange) ||
            !Mathf.Approximately(angle, lastAngle) ||
            segments != lastSegments ||
            e != lastEyes ||
            showCap != lastShowCap;

        if (!need) return;

        BuildMesh(range, angle, segments, showCap);

        lastRange = range;
        lastAngle = angle;
        lastSegments = segments;
        lastEyes = e;
        lastShowCap = showCap;
    }

    void BuildMesh(float range, float angle, int segments, bool showCap)
    {
        float half = Mathf.Deg2Rad * (angle * 0.5f);
        float radius = Mathf.Tan(half) * range;

        int sideApex = 0;
        int sideCircleStart = 1;
        int sideVerts = 1 + segments;

        int capCenterIndex = -1, capCircleStart = -1;
        int capVerts = showCap ? (1 + segments) : 0;

        Vector3[] v = new Vector3[sideVerts + capVerts];
        Vector3[] n = new Vector3[v.Length];
        Vector2[] u = new Vector2[v.Length];

        v[sideApex] = Vector3.zero;

        float startDeg = -angle * 0.5f;
        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            float theta = (startDeg + t * angle) * Mathf.Deg2Rad;
            float x = Mathf.Sin(theta) * radius;
            float z = Mathf.Cos(theta) * range;
            v[sideCircleStart + i] = new Vector3(x, 0f, z);
        }

        for (int i = 0; i < v.Length; i++)
            n[i] = (i == sideApex) ? Vector3.back : (v[i] - v[sideApex]).normalized;

        for (int i = 0; i < v.Length; i++) u[i] = Vector2.zero;

        if (showCap)
        {
            capCenterIndex = sideVerts;
            capCircleStart = capCenterIndex + 1;
            v[capCenterIndex] = new Vector3(0, 0, range);
            n[capCenterIndex] = Vector3.back;

            for (int i = 0; i < segments; i++)
            {
                float t = (float)i / (segments - 1);
                float theta = (startDeg + t * angle) * Mathf.Deg2Rad;
                float x = Mathf.Sin(theta) * radius;
                float z = Mathf.Cos(theta) * range;
                v[capCircleStart + i] = new Vector3(x, 0f, z);
                n[capCircleStart + i] = Vector3.back;
            }
        }

        int sideTris = (segments - 1) * 3;
        int capTris = showCap ? (segments - 1) * 3 : 0;
        int[] tri = new int[sideTris + capTris];

        int ti = 0;
        for (int i = 0; i < segments - 1; i++)
        {
            tri[ti++] = sideApex;
            tri[ti++] = sideCircleStart + i;
            tri[ti++] = sideCircleStart + i + 1;
        }
        if (showCap)
        {
            for (int i = 0; i < segments - 1; i++)
            {
                tri[ti++] = capCenterIndex;
                tri[ti++] = capCircleStart + i + 1;
                tri[ti++] = capCircleStart + i;
            }
        }

        coneMesh.Clear();
        coneMesh.vertices = v;
        coneMesh.normals = n;
        coneMesh.uv = u;
        coneMesh.triangles = tri;
        coneMesh.RecalculateBounds();
    }

    // ---------- Color (stabilized) ----------
    void InitColor()
    {
        bool alert = (sight && sight.CanSeePlayer);
        lastAlert = alert;
        lastFlipTime = Time.unscaledTime;
        currentColor = alert ? alertColor : normalColor;
        ApplyColor(currentColor);
    }

    void UpdateColorStabilized()
    {
        if (!mr) return;

        bool desired = (sight && sight.CanSeePlayer);

        // 히스테리시스: 상태가 바뀌더라도 stateHold 동안 유지되면 드디어 전환
        if (desired != lastAlert)
        {
            if (Time.unscaledTime - lastFlipTime >= stateHold)
            {
                lastAlert = desired;
                lastFlipTime = Time.unscaledTime;
            }
        }
        else
        {
            lastFlipTime = Time.unscaledTime;
        }

        Color target = lastAlert ? alertColor : normalColor;

        if (smoothColor)
            currentColor = Color.Lerp(currentColor, target, Time.unscaledDeltaTime * colorLerpSpeed);
        else
            currentColor = target;

        ApplyColor(currentColor);
    }

    void ApplyColor(Color c)
    {
        if (!mr) return;

        mr.GetPropertyBlock(mpb);

        // URP 기본: _BaseColor, 표준 Unlit/Color: _Color
        if (mr.sharedMaterial && mr.sharedMaterial.HasProperty("_BaseColor"))
            mpb.SetColor("_BaseColor", c);
        else
            mpb.SetColor("_Color", c);

        mr.SetPropertyBlock(mpb);
    }

    void OnDrawGizmos()
    {
        if (!drawAxis) return;
        var e = Eyes(); if (!e) return;

        Gizmos.color = Color.cyan; Gizmos.DrawRay(e.position, e.forward * 0.6f);
        Gizmos.color = Color.magenta; Gizmos.DrawRay(e.position, e.right * 0.3f);
        Gizmos.color = Color.green; Gizmos.DrawRay(e.position, e.up * 0.3f);
    }
}
