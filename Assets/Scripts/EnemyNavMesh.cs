using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    public enum State { Patrol, Chase, Investigate};

    [SerializeField] private Transform playerPosition;
    private NavMeshAgent navMeshAgent;
    private Animator myAnimator;
    public int targetWaypoint;

    public Transform pathHolder;
    public State myState;
    public Transform[] waypoints;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
        myState = State.Patrol;
        targetWaypoint = 1;
        StartCoroutine(FollowPath());
    }

    private void Update()
    {
        if (myState == State.Chase)
        {
            navMeshAgent.isStopped = false;
            myAnimator.SetBool("isWalking", true);
            navMeshAgent.destination = playerPosition.position;
        }
        else if (myState == State.Patrol)
        {
            /*myAnimator.SetBool("isWalking", true);
            navMeshAgent.destination = waypoints[actualWaypoint].position;
            float dist = Vector3.Distance(this.transform.position, waypoints[actualWaypoint].position);
            if (dist < 0.2f)
            {
                actualWaypoint = (actualWaypoint + 1) % waypoints.Length;
            }*/
        }
    }

    IEnumerator FollowPath()
    {
        while (true)
        {
            if (myState == State.Patrol)
            {
                myAnimator.SetBool("isWalking", true);
                navMeshAgent.destination = waypoints[targetWaypoint].position;
                float dist = Vector3.Distance(this.transform.position, waypoints[targetWaypoint].position);
                if (dist < 0.2f)
                {
                    myAnimator.SetBool("isWalking", false);
                    navMeshAgent.isStopped = true;
                    yield return new WaitForSeconds(2f);
                    targetWaypoint = (targetWaypoint + 1) % waypoints.Length;
                    navMeshAgent.isStopped = false;
                }
            }
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
        }
    }
}
