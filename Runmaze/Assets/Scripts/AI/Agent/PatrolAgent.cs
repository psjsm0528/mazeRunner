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
        // 에이전트가 경로를 완료했는지 확인
        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            // 남은 거리 계산
            float remainingDistance = agent.remainingDistance;

            // stoppingDistance와 비교
            if (remainingDistance <= agent.stoppingDistance)
            {
                // 다음 타겟으로 이동하는 로직
                onComplete?.Invoke();
            }
        }
    }
}
