using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject LoseScreen;


    public void Won()
    {
        WinScreen.SetActive(true);
    }
    public void Lost()
    {
        LoseScreen.SetActive(true);
    }
}
