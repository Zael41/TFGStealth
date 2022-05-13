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

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        myState = State.Chase;
    }

    private void Update()
    {
        if (myState == State.Chase)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = playerPosition.position;
        }
        else
        {
            navMeshAgent.isStopped = true;
        }
    }
}
