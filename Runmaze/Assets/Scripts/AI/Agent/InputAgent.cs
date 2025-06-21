using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputAgent : BaseAgent
{
    Transform cam;
    Vector2 inputVec;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        inputVec = new(horizontal, vertical);

        Vector3 dirVec = cam.right * inputVec.x + cam.forward * inputVec.y;
        agent.SetDestination(transform.position + dirVec.normalized);
    }

    
}
