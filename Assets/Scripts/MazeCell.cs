using TMPro;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public bool IsVisited = false;
    public GameObject NorthWall;
    public GameObject SouthWall;
    public GameObject EastWall;
    public GameObject WestWall;

    public TextMeshPro text;

    public void RemoveWall(GameObject wall)
    {
        wall.SetActive(false);
    }
    public void SetNumber(int number)
    {
        text.text = (number + 1).ToString();
    }
}
