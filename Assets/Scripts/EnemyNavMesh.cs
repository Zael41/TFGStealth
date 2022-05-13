using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    public enum State { Patrol, Chase, Investigate, ReturnPatrol };

    [SerializeField] private Transform playerPosition;
    private NavMeshAgent navMeshAgent;
    public State myState;
    private Animator myAnimator;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
        myState = State.Chase;
    }

    private void Update()
    {
        if (myState == State.Chase)
        {
            navMeshAgent.isStopped = false;
            myAnimator.SetBool("isWalking", true);
            navMeshAgent.destination = playerPosition.position;
        }
        else
        {
            navMeshAgent.isStopped = true;
            myAnimator.SetBool("isWalking", false);
        }
    }
}
