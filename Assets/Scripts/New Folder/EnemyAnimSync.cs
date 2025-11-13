using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyChaseNav))]
public class EnemyAnimSync : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;            // [변경가능]
    public NavMeshAgent agent;           // [변경가능]
    public EnemyChaseNav chase;          // [변경가능]

    [Header("Move Speeds (agent.speed를 자동 전환)")]
    public float patrolSpeed = 1.8f;     // [변경가능] 배회/복귀 속도
    public float runSpeed = 3.5f;        // [변경가능] 추격 속도

    [Header("Animation Damping")]
    public string speedParam = "Speed";  // [변경가능] BlendTree 파라미터명
    public float dampTime = 0.12f;       // [변경가능]
    public float idleThreshold = 0.05f;  // [변경가능] 거의 정지로 보는 속도

    float _speedSmooth;

    void Reset()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        chase = GetComponent<EnemyChaseNav>();
    }

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!chase) chase = GetComponent<EnemyChaseNav>();
    }

    void Update()
    {
        if (!animator || !agent || !chase) return;

        // 1) 상태별 에이전트 속도 전환
        switch (chase.CurrentState)
        {
            case EnemyChaseNav.State.Chase:
                if (!Mathf.Approximately(agent.speed, runSpeed)) agent.speed = runSpeed;
                break;

            case EnemyChaseNav.State.Patrol:
            case EnemyChaseNav.State.ReturnHome:
                if (!Mathf.Approximately(agent.speed, patrolSpeed)) agent.speed = patrolSpeed;
                break;
        }

        // 2) 실제 이동 속도를 애니메이터에 전달 (BlendTree 구동)
        //    - remainingDistance가 매우 작으면 멈춘 것으로 간주
        float raw = agent.velocity.magnitude;
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            raw = 0f;

        _speedSmooth = Mathf.Lerp(_speedSmooth, raw, Time.deltaTime / Mathf.Max(0.0001f, dampTime));
        if (_speedSmooth < idleThreshold) _speedSmooth = 0f;

        animator.SetFloat(speedParam, _speedSmooth);
    }
}
