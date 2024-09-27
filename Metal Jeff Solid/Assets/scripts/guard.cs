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
    [SerializeField] GameObject player;
    GuardStates state = GuardStates.Patrol;
    Rigidbody rb;
    [SerializeField] float speed;
    NavMeshPath navPath;
    Queue<Vector3> remainingPoints;
    Vector3 currentTargetPoint;
    bool lineOfSight = false;
    [SerializeField] float radius;
    [Range(0,360)]
    [SerializeField] float angle;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] Transform[] points;
    [SerializeField] int current;
    bool pursuing = false;
    float distToPoint;
    bool pathCrated = false;
    float distanceToTarget = 0;
    bool sneaking;
    float startSpeed;

    // Start is called before the first frame update
    void Start()
    {
        lineOfSight=false;
        current = 0;
        rb = GetComponent<Rigidbody>();
        navPath = new NavMeshPath();
        remainingPoints = new Queue<Vector3>();
        state = GuardStates.Patrol;
        startSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        sneaking = player.GetComponent<Player>().sneak;

        if(!lineOfSight && !pursuing)
        {
            state = GuardStates.Patrol;
        }
        else if(lineOfSight)
        {
            state  = GuardStates.Pursue;
        }
        else
        {
            state = GuardStates.Investigate;
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
    private void OnDrawGizmos()
    {
        if(navPath == null)
            return;

        Gizmos.color = Color.red;
        foreach(Vector3 nodes in navPath.corners)
        {
            Gizmos.DrawWireSphere(nodes, 0.5f);
        }
        OnDrawFov();
    }
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
        FieldOfViewCheck();
    }

    void UpdatePatrol()
    {
        distToPoint = Vector3.Distance(transform.position, points[current].position);
        if (distToPoint > 1.1)
        {
            var new_forward = (points[current].position - transform.position).normalized;
            new_forward.y = 0;
            transform.forward = new_forward;
        }
        else
        {
            current = (current + 1) % points.Length;
        }
    }
    void UpdatePursue()
    {
        if (!pathCrated)
        {
            if (agent.CalculatePath(target.position, navPath))
            {
                foreach (Vector3 p in navPath.corners)
                {
                    remainingPoints.Enqueue(p);
                }
                currentTargetPoint = remainingPoints.Dequeue();
            }
            pathCrated = true;
        }
        pursuing = true;
        var new_forward = (currentTargetPoint-transform.position).normalized;
        new_forward.y = 0;
        transform.forward = new_forward;

        distToPoint = Vector3.Distance(transform.position, currentTargetPoint);

        if (distToPoint < 1.1)
        {
            if (remainingPoints.Count > 0)
            {
                currentTargetPoint = remainingPoints.Dequeue();
            }
            else {
                if (agent.CalculatePath(target.position, navPath))
                {
                    foreach (Vector3 p in navPath.corners)
                    {
                        remainingPoints.Enqueue(p);
                    }
                    currentTargetPoint = remainingPoints.Dequeue();
                }

            }
        }
    }
    void UpdateInvestigate()
    {
        var new_forward = (currentTargetPoint - transform.position).normalized;
        new_forward.y = 0;
        transform.forward = new_forward;

        distToPoint = Vector3.Distance(transform.position, currentTargetPoint);

        if (distToPoint < 1.1)
        {
            if (remainingPoints.Count > 0)
            {
                currentTargetPoint = remainingPoints.Dequeue();
            }
            else
            {
                speed = 0;
                Invoke("resetState", 3);
            }
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    lineOfSight = true;
                }
                else
                {
                    lineOfSight = false;
                }
            }
            else
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask) && !sneaking)
                {
                    lineOfSight = true;
                }
                else
                {
                    lineOfSight = false;
                }
            }
        }
        else if (lineOfSight)
        {
            lineOfSight = false;
        }
    }

    private void OnDrawFov()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);

        Vector3 viewAngle1 = DirectionFromAngle(transform.eulerAngles.y, -angle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(transform.eulerAngles.y, angle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position+viewAngle1 * radius);
        
        Gizmos.DrawLine(transform.position, transform.position+viewAngle2 * radius);

        if (lineOfSight)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    
    void resetState()
    {
        pursuing = false;
        pathCrated = false;
        speed = startSpeed;
    }
}
