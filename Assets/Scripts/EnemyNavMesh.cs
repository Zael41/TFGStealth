using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    public enum State { Patrol, Chase, Investigate};

    public static event System.Action OnGuardHasSpottedPlayer;

    [SerializeField] private Transform playerPosition;
    private NavMeshAgent navMeshAgent;
    private Animator myAnimator;
    public int targetWaypoint;

    public Transform pathHolder;
    public State myState;
    public Transform[] waypoints;

    public Light spotLight;
    public float viewDistance;
    public LayerMask viewMask;
    public float timeToSpotPlayer = .5f;
    float playerVisibleTimer;
    float viewAngle;
    Color originalSpotlightColor;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
        myState = State.Patrol;
        targetWaypoint = 1;
        viewAngle = spotLight.spotAngle;
        originalSpotlightColor = spotLight.color;
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
        if (CanSeePlayer())
        {
            //spotLight.color = Color.red;
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            //spotLight.color = originalSpotlightColor;
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotLight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
            }
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

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, playerPosition.position) < viewDistance)
        {
            Vector3 dirToPlayer = (playerPosition.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, playerPosition.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position, transform.forward * viewDistance);
    }
}
