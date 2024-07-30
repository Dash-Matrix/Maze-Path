using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public Transform targetPosition; // Set this in the Inspector or programmatically

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        MoveToTargetPosition();
    }

    void MoveToTargetPosition()
    {
        if (targetPosition != null)
        {
            navMeshAgent.SetDestination(targetPosition.position);
        }
        else
        {
            Debug.LogWarning("Target position not set!");
        }
    }

}
