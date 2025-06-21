using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleStaticAgent : BaseAgent
{
    [SerializeField] Transform target;

    // Start is called before the first frame update
    void Start()
    {
        agent.SetDestination(target.position);
    }
}
