using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [Header("Ÿ�� ����")]
    public Transform target;                    // ������ Ÿ��

    [Header("�Ÿ��� ���� ����")]
    public float chaseDistance = 15f;          // ���� ���� �Ÿ�
    public float attackDistance = 2f;          // ���� �Ÿ�
    public float retreatDistance = 1f;         // ���� �Ÿ�
    public float patrolRadius = 10f;           // ���� �ݰ�

    [Header("�ӵ� ����")]
    public float chaseSpeed = 8f;              // ���� �ӵ�
    public float patrolSpeed = 3f;             // ���� �ӵ�
    public float retreatSpeed = 6f;            // ���� �ӵ�

    [Header("AI ����")]
    public float rotationSpeed = 5f;           // ȸ�� �ӵ�
    public float stoppingDistance = 0.5f;      // ���� �Ÿ�
    public bool enablePatrol = true;           // ���� Ȱ��ȭ
    public float patrolWaitTime = 2f;          // ���� ���� ��� �ð�

    private NavMeshAgent agent;
    private Vector3 startPosition;
    private Vector3 currentPatrolPoint;
    private float lastPatrolTime;
    private bool isPatrolling = false;

    // AI ���� ������
    public enum AIState
    {
        Idle,       // ���
        Patrol,     // ����
        Chase,      // ����
        Attack,     // ����
        Retreat     // ����
    }

    private AIState currentState = AIState.Idle;

    private void Start()
    {
        // NavMeshAgent ������Ʈ ��������
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        // �ʱ� ����
        startPosition = transform.position;
        currentPatrolPoint = startPosition;

        // NavMeshAgent �⺻ ����
        SetupNavMeshAgent();

        // �ʱ� ���� ����
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
            // Ÿ���� ������ ���� �Ǵ� ���
            if (enablePatrol && currentState == AIState.Idle)
            {
                SetState(AIState.Patrol);
            }
            return;
        }

        // Ÿ�ٰ��� �Ÿ� ���
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // �Ÿ��� ���� ���� ����
        DetermineState(distanceToTarget);

        // ���� ���¿� ���� ���� ����
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

        // �Ÿ��� ���� ���� ��ȯ
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
            // Ÿ���� ���� ������ ����� ������ ��ȯ
            if (enablePatrol)
            {
                newState = AIState.Patrol;
            }
            else
            {
                newState = AIState.Idle;
            }
        }

        // ���°� ����Ǹ� ���ο� ���·� ����
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

        // ���º� NavMeshAgent ����
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

        Debug.Log($"{gameObject.name}: ���� ���� - {newState}");
    }

    private void ExecuteIdle()
    {
        // ��� ���¿����� �ƹ��͵� ���� ����
        // Ÿ���� �ٶ󺸱⸸ ��
        if (target != null)
        {
            LookAtTarget();
        }
    }

    private void ExecutePatrol()
    {
        // ���� ������ �����ߴ��� Ȯ��
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
        // Ÿ���� ����
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    private void ExecuteAttack()
    {
        // ���� ���¿����� Ÿ���� �ٶ󺸰� ���� ���� ����
        if (target != null)
        {
            LookAtTarget();
            // ���⿡ ���� ������ �߰��� �� �ֽ��ϴ�
            // ��: �ִϸ��̼� ���, ������ ó�� ��
        }
    }

    private void ExecuteRetreat()
    {
        // Ÿ�����κ��� �־����� �������� �̵�
        if (target != null)
        {
            Vector3 retreatDirection = (transform.position - target.position).normalized;
            Vector3 retreatPosition = transform.position + retreatDirection * 5f;

            // NavMesh ���� ��ȿ�� ��ġ�� ����
            NavMeshHit hit;
            if (NavMesh.SamplePosition(retreatPosition, out hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    private void SetNewPatrolPoint()
    {
        // ���� �ݰ� ������ ������ ���� ����
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;

        // NavMesh ���� ��ȿ�� ��ġ�� ����
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
            direction.y = 0; // Y�� ȸ���� ���

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // Ÿ�� ���� �޼���
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Ÿ�� ���� �޼���
    public void ClearTarget()
    {
        target = null;
    }

    // ���� ���� ��ȯ
    public AIState GetCurrentState()
    {
        return currentState;
    }

    // ����׿� ����� �׸���
    private void OnDrawGizmosSelected()
    {
        // ���� ���� ǥ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        // ���� ���� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // ���� ���� ǥ��
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);

        // ���� ���� ǥ��
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPosition, patrolRadius);

            // ���� ���� ���� ǥ��
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
