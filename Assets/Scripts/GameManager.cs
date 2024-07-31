using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public UIManager UImanager;
    public GameObject test;
    private void Awake()
    {
        instance = this;
    }

    public void WonLose(bool won)
    {
        if (won)
        {
            UImanager.Won();
        }
        else
        {
            UImanager.Lost();
        }

        Time.timeScale = 0;
    }









    public void NextLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);

    }
    public void RetryLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
