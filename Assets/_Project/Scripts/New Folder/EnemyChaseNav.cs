using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyConeSight))]
public class EnemyChaseNav : MonoBehaviour
{
    public enum State { Patrol, Chase, ReturnHome }

    [Header("=== [TUNE] 참조 ===")]
    public EnemyConeSight sight;                 // [변경가능] 시야 컴포넌트
    public NavMeshAgent agent;                   // [변경가능] NavMesh 에이전트

    [Header("=== [TUNE] 배회 ===")]
    [Tooltip("배회 반경")]
    public float patrolRadius = 8f;              // [변경가능]
    [Tooltip("웨이포인트 도착 후 머무는 시간(초)")]
    public float dwellTime = 1.2f;               // [변경가능]
    [Tooltip("웨이포인트 도착 판정 거리")]
    public float waypointTolerance = 0.8f;       // [변경가능]
    [Tooltip("배회 시 다음 웨이포인트를 찾을 때 실패 허용 횟수")]
    public int patrolSampleTries = 6;            // [변경가능]

    [Header("=== [TUNE] 추격 ===")]
    [Tooltip("목표 재계산 주기(초)")]
    public float repathInterval = 0.1f;          // [변경가능]
    [Tooltip("정지 거리(플레이어 접근 시 어느 정도 앞에서 멈출지)")]
    public float stopDistance = 1.2f;            // [변경가능]
    [Tooltip("플레이어를 못 본 상태 유지 시간(초) 초과 시 복귀")]
    public float lostHoldTime = 2.0f;            // [변경가능]

    [Header("=== [TUNE] 복귀 ===")]
    [Tooltip("집(시작 위치) 도착 판정 거리")]
    public float homeArriveDistance = 0.8f;      // [변경가능]

    public State CurrentState { get; private set; }

    Vector3 homePos;
    float lastSeenTime = -999f;
    Coroutine fsm;

    void Reset()
    {
        sight = GetComponent<EnemyConeSight>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Awake()
    {
        if (!sight) sight = GetComponent<EnemyConeSight>();
        if (!agent) agent = GetComponent<NavMeshAgent>();

        // 가장 단순한 세팅: 에이전트가 회전도 처리
        agent.updateRotation = true;
        agent.stoppingDistance = stopDistance;

        homePos = transform.position;

        // sight.player 자동 할당이 안되어 있다면 태그로 찾기
        if (!sight.player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) sight.player = p.transform;
        }
    }

    void OnEnable()
    {
        fsm = StartCoroutine(FSM());
    }

    void OnDisable()
    {
        if (fsm != null) StopCoroutine(fsm);
    }

    IEnumerator FSM()
    {
        SetState(State.Patrol);
        while (true)
        {
            switch (CurrentState)
            {
                case State.Patrol: yield return PatrolLoop(); break;
                case State.Chase: yield return ChaseLoop(); break;
                case State.ReturnHome: yield return ReturnHomeLoop(); break;
            }
            yield return null;
        }
    }

    void SetState(State s) => CurrentState = s;

    // ----------------- Patrol -----------------
    IEnumerator PatrolLoop()
    {
        agent.stoppingDistance = 0f;
        Vector3 nextWp = PickPatrolPoint();
        agent.SetDestination(nextWp);
        float dwellUntil = float.NegativeInfinity;

        while (CurrentState == State.Patrol)
        {
            // 플레이어를 보면 추격 전환
            if (sight.CanSeePlayer && sight.player)
            {
                SetState(State.Chase);
                yield break;
            }

            // 웨이포인트 도착 체크
            if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
            {
                // 도착 후 잠시 머물기
                if (float.IsNegativeInfinity(dwellUntil))
                    dwellUntil = Time.time + dwellTime;

                if (Time.time >= dwellUntil)
                {
                    nextWp = PickPatrolPoint();
                    agent.SetDestination(nextWp);
                    dwellUntil = float.NegativeInfinity;
                }
            }

            yield return null;
        }
    }

    // ----------------- Chase -----------------
    IEnumerator ChaseLoop()
    {
        agent.stoppingDistance = stopDistance;

        while (CurrentState == State.Chase)
        {
            if (sight.player && sight.CanSeePlayer)
            {
                lastSeenTime = Time.time;

                Vector3 goal = sight.player.position;
                if (Vector3.Distance(transform.position, goal) > stopDistance)
                    agent.SetDestination(goal);
            }
            else
            {
                // 못 본 시간이 임계치를 넘으면 복귀
                if (Time.time - lastSeenTime > lostHoldTime)
                {
                    SetState(State.ReturnHome);
                    yield break;
                }
            }

            yield return new WaitForSeconds(repathInterval);
        }
    }

    // ----------------- Return Home -----------------
    IEnumerator ReturnHomeLoop()
    {
        agent.stoppingDistance = 0f;
        agent.SetDestination(homePos);

        while (CurrentState == State.ReturnHome)
        {
            // 복귀 도중 플레이어를 보면 바로 추격
            if (sight.CanSeePlayer && sight.player)
            {
                SetState(State.Chase);
                yield break;
            }

            if (!agent.pathPending && agent.remainingDistance <= homeArriveDistance)
            {
                SetState(State.Patrol);
                yield break;
            }

            yield return null;
        }
    }

    // ----------------- Utilities -----------------
    Vector3 PickPatrolPoint()
    {
        for (int i = 0; i < patrolSampleTries; i++)
        {
            Vector2 r = Random.insideUnitCircle * patrolRadius;
            Vector3 cand = homePos + new Vector3(r.x, 0f, r.y);
            if (NavMesh.SamplePosition(cand, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }
        // 실패 시 집 위치로
        return homePos;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 p = Application.isPlaying ? homePos : transform.position;
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.25f);
        Gizmos.DrawWireSphere(p, patrolRadius);
        Gizmos.DrawSphere(p, 0.12f);
    }
#endif
}
