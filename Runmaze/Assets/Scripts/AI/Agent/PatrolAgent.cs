using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAgent : BaseAgent
{
    [SerializeField] Transform[] targets;
    int targetIndex = -1;

    Action onComplete;

    private void Start()
    {
        onComplete += SetTarget;
        SetTarget();
    }

    void SetTarget()
    {
        targetIndex = (targetIndex + 1) % targets.Length;
        agent.SetDestination(targets[targetIndex].position);
    }

    private void Update()
    {
        // ������Ʈ�� ��θ� �Ϸ��ߴ��� Ȯ��
        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            // ���� �Ÿ� ���
            float remainingDistance = agent.remainingDistance;

            // stoppingDistance�� ��
            if (remainingDistance <= agent.stoppingDistance)
            {
                // ���� Ÿ������ �̵��ϴ� ����
                onComplete?.Invoke();
            }
        }
    }
}
