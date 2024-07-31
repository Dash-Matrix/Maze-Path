using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public int ID;

    public NavMeshAgent navMeshAgent;

    private LevelManager lm;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void MoveToTargetPosition(LevelManager LM)
    {
        lm = LM;

        if (lm.target != null)
        {
            navMeshAgent.SetDestination(lm.target.position);
        }
        else
        {
            Debug.LogWarning("Target position not set!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WinArea")
        {
            lm.CheckWin(ID);
        }
    }
}
