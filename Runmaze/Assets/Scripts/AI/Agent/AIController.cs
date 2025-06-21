using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [Header("타겟 설정")]
    public Transform target;                    // 추적할 타겟

    [Header("거리별 동작 설정")]
    public float chaseDistance = 15f;          // 추적 시작 거리
    public float attackDistance = 2f;          // 공격 거리
    public float retreatDistance = 1f;         // 후퇴 거리
    public float patrolRadius = 10f;           // 순찰 반경

    [Header("속도 설정")]
    public float chaseSpeed = 8f;              // 추적 속도
    public float patrolSpeed = 3f;             // 순찰 속도
    public float retreatSpeed = 6f;            // 후퇴 속도

    [Header("AI 설정")]
    public float rotationSpeed = 5f;           // 회전 속도
    public float stoppingDistance = 0.5f;      // 정지 거리
    public bool enablePatrol = true;           // 순찰 활성화
    public float patrolWaitTime = 2f;          // 순찰 지점 대기 시간

    private NavMeshAgent agent;
    private Vector3 startPosition;
    private Vector3 currentPatrolPoint;
    private float lastPatrolTime;
    private bool isPatrolling = false;

    // AI 상태 열거형
    public enum AIState
    {
        Idle,       // 대기
        Patrol,     // 순찰
        Chase,      // 추적
        Attack,     // 공격
        Retreat     // 후퇴
    }

    private AIState currentState = AIState.Idle;

    private void Start()
    {
        // NavMeshAgent 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        // 초기 설정
        startPosition = transform.position;
        currentPatrolPoint = startPosition;

        // NavMeshAgent 기본 설정
        SetupNavMeshAgent();

        // 초기 상태 설정
        if (enablePatrol)
        {
            SetState(AIState.Patrol);
        }
        else
        {
            SetState(AIState.Idle);
        }
    }

    private void Update()
    {
        if (target == null)
        {
            // 타겟이 없으면 순찰 또는 대기
            if (enablePatrol && currentState == AIState.Idle)
            {
                SetState(AIState.Patrol);
            }
            return;
        }

        // 타겟과의 거리 계산
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // 거리에 따른 상태 결정
        DetermineState(distanceToTarget);

        // 현재 상태에 따른 동작 수행
        ExecuteCurrentState(distanceToTarget);
    }

    private void SetupNavMeshAgent()
    {
        agent.stoppingDistance = stoppingDistance;
        agent.angularSpeed = rotationSpeed;
        agent.updateRotation = true;
        agent.updateUpAxis = false;
    }

    private void DetermineState(float distanceToTarget)
    {
        AIState newState = currentState;

        // 거리에 따른 상태 전환
        if (distanceToTarget <= retreatDistance)
        {
            newState = AIState.Retreat;
        }
        else if (distanceToTarget <= attackDistance)
        {
            newState = AIState.Attack;
        }
        else if (distanceToTarget <= chaseDistance)
        {
            newState = AIState.Chase;
        }
        else
        {
            // 타겟이 감지 범위를 벗어나면 순찰로 전환
            if (enablePatrol)
            {
                newState = AIState.Patrol;
            }
            else
            {
                newState = AIState.Idle;
            }
        }

        // 상태가 변경되면 새로운 상태로 설정
        if (newState != currentState)
        {
            SetState(newState);
        }
    }

    private void ExecuteCurrentState(float distanceToTarget)
    {
        switch (currentState)
        {
            case AIState.Idle:
                ExecuteIdle();
                break;
            case AIState.Patrol:
                ExecutePatrol();
                break;
            case AIState.Chase:
                ExecuteChase();
                break;
            case AIState.Attack:
                ExecuteAttack();
                break;
            case AIState.Retreat:
                ExecuteRetreat();
                break;
        }
    }

    private void SetState(AIState newState)
    {
        currentState = newState;

        // 상태별 NavMeshAgent 설정
        switch (newState)
        {
            case AIState.Idle:
                agent.speed = 0f;
                agent.isStopped = true;
                break;
            case AIState.Patrol:
                agent.speed = patrolSpeed;
                agent.isStopped = false;
                SetNewPatrolPoint();
                break;
            case AIState.Chase:
                agent.speed = chaseSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attack:
                agent.speed = 0f;
                agent.isStopped = true;
                break;
            case AIState.Retreat:
                agent.speed = retreatSpeed;
                agent.isStopped = false;
                break;
        }

        Debug.Log($"{gameObject.name}: 상태 변경 - {newState}");
    }

    private void ExecuteIdle()
    {
        // 대기 상태에서는 아무것도 하지 않음
        // 타겟을 바라보기만 함
        if (target != null)
        {
            LookAtTarget();
        }
    }

    private void ExecutePatrol()
    {
        // 순찰 지점에 도달했는지 확인
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (Time.time - lastPatrolTime >= patrolWaitTime)
            {
                SetNewPatrolPoint();
                lastPatrolTime = Time.time;
            }
        }
    }

    private void ExecuteChase()
    {
        // 타겟을 추적
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    private void ExecuteAttack()
    {
        // 공격 상태에서는 타겟을 바라보고 공격 로직 실행
        if (target != null)
        {
            LookAtTarget();
            // 여기에 공격 로직을 추가할 수 있습니다
            // 예: 애니메이션 재생, 데미지 처리 등
        }
    }

    private void ExecuteRetreat()
    {
        // 타겟으로부터 멀어지는 방향으로 이동
        if (target != null)
        {
            Vector3 retreatDirection = (transform.position - target.position).normalized;
            Vector3 retreatPosition = transform.position + retreatDirection * 5f;

            // NavMesh 위의 유효한 위치로 조정
            NavMeshHit hit;
            if (NavMesh.SamplePosition(retreatPosition, out hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    private void SetNewPatrolPoint()
    {
        // 순찰 반경 내에서 랜덤한 지점 선택
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;

        // NavMesh 위의 유효한 위치로 조정
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            currentPatrolPoint = hit.position;
            agent.SetDestination(currentPatrolPoint);
        }
    }

    private void LookAtTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // Y축 회전만 고려

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // 타겟 설정 메서드
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // 타겟 제거 메서드
    public void ClearTarget()
    {
        target = null;
    }

    // 현재 상태 반환
    public AIState GetCurrentState()
    {
        return currentState;
    }

    // 디버그용 기즈모 그리기
    private void OnDrawGizmosSelected()
    {
        // 감지 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        // 공격 범위 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // 후퇴 범위 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);

        // 순찰 범위 표시
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPosition, patrolRadius);

            // 현재 순찰 지점 표시
            if (isPatrolling)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(currentPatrolPoint, 0.5f);
            }
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, patrolRadius);
        }
    }
}
