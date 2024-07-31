using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private GameObject[] players;
    [HideInInspector] public Transform target;


    public int chosenID;


    public void CheckWin(int ID)
    {
        if (chosenID == ID)
        {
            //Won
            Debug.Log("Won");
            GameManager.instance.WonLose(true);
        }
        else
        {
            //Lose
            Debug.Log("Lose");
            GameManager.instance.WonLose(false);
        }
    }

    public void IDChosen(int id)
    {
        chosenID = id;
        Debug.Log("Button Pressed");

        if (players != null)
        {
            for (int i = 0; players.Length > i; i++)
            {
                Debug.Log(players[i].name);

                players[i].SetActive(true);
                players[i].GetComponent<PlayerMovement>().MoveToTargetPosition(gameObject.GetComponent<LevelManager>());
            }
        }
        else
        {
            Debug.Log(players.Length + " : No Player");
        }


    }
}
