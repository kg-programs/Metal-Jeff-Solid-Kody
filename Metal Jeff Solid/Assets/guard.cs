using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GuardStates
{
    Patrol,
    Investigate,
    Pursue
}

public class guard : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    GuardStates state = GuardStates.Patrol;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(target.position);
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;

        float dot = Vector3.Dot(directionToTarget, forwardDirection);

        if(dot > 0.5f)
        {
            state = GuardStates.Pursue;
            //front
        }
        else //if(dot < -0.5f)
        {
            state  = GuardStates.Patrol;
            //back
        }

        switch (state)
        {
            case GuardStates.Pursue:
                UpdatePursue();
                break;
            case GuardStates.Patrol:
                UpdatePatrol();
                break;
            case GuardStates.Investigate:
                UpdateInvestigate();
                break;

        }
    }

    void UpdatePatrol()
    {
        Debug.Log("lost them");
    }
    void UpdatePursue()
    {
        Debug.Log("Enemy spotted");
    }
    void UpdateInvestigate()
    {

    }
}
