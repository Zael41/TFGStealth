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
    public Timer timer;
    public ArrayList scripts = new ArrayList();

    public Transform pathHolder;
    public State myState;
    public Transform[] waypoints;

    public Light spotLight;
    public float viewDistance;
    public LayerMask viewMask;
    public static float timeToSpotPlayer = 5f;
    static float playerVisibleTimer;
    float viewAngle;
    Color originalSpotlightColor;
    bool playerDisabled;
    static EnemyNavMesh lastGuard;
    private Vector3 investigationSpot;
    bool waitingForClues;
    Coroutine co;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
        myState = State.Patrol;
        targetWaypoint = 1;
        viewAngle = spotLight.spotAngle;
        originalSpotlightColor = spotLight.color;
        StartCoroutine(FollowPath());
        playerDisabled = false;

        GameObject[] guards = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in guards)
        {
            scripts.Add(enemy.GetComponent<EnemyNavMesh>());
        }
        lastGuard = (EnemyNavMesh)scripts[0];
    }

    private void Update()
    {
        if (Vector3.Distance(this.transform.position, playerPosition.position) < playerPosition.GetComponentInParent<PlayerMovement>().detectionRange)
        {
            if (myState == State.Patrol)
            {
                myState = State.Investigate;
                investigationSpot = playerPosition.position;
            }
            else if (myState == State.Investigate && waitingForClues)
            {
                StopCoroutine(co);
                investigationSpot = playerPosition.position;
                waitingForClues = false;
            }
        }
        if (myState == State.Chase)
        {
            StopCoroutine(co);
            waitingForClues = false;
            navMeshAgent.isStopped = false;
            myAnimator.SetBool("isWalking", true);
            navMeshAgent.destination = playerPosition.position;
        }
        else if (myState == State.Investigate && !waitingForClues)
        {
            navMeshAgent.isStopped = false;
            myAnimator.SetBool("isWalking", true);
            navMeshAgent.destination = investigationSpot;
            if (Vector3.Distance(this.transform.position, investigationSpot) < 0.2f)
            {
                co = StartCoroutine(WaitForClues());
                waitingForClues = true;
            }
        }
        if (CanSeePlayer())
        {
            //spotLight.color = Color.red;
            playerVisibleTimer += Time.deltaTime;
            lastGuard = this;
        }
        else if (this == lastGuard)
        {
            //spotLight.color = originalSpotlightColor;
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        ManageTimer();
        if (this == lastGuard) spotLight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
        else spotLight.color = Color.Lerp(originalSpotlightColor, Color.red, 0f / timeToSpotPlayer);
        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
                playerDisabled = true;
            }
        }
    }

    void ManageTimer()
    {
        if (playerDisabled)
        {
            return;
        }
        if (CanSeePlayer())
        {
            timer.DisplayTime(timeToSpotPlayer - playerVisibleTimer, true);
        }
        else if (playerVisibleTimer > 0 && this == lastGuard)
        {
            timer.DisplayTime(timeToSpotPlayer - playerVisibleTimer, false);
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

    IEnumerator WaitForClues()
    {
        myAnimator.SetBool("isWalking", false);
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(3f);
        if (myState == State.Investigate) myState = State.Patrol;
        navMeshAgent.isStopped = false;
        waitingForClues = false;
        yield break;
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
