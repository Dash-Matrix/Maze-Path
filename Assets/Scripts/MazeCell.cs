using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public bool IsVisited = false;
    public GameObject NorthWall;
    public GameObject SouthWall;
    public GameObject EastWall;
    public GameObject WestWall;

    public void RemoveWall(GameObject wall)
    {
        wall.SetActive(false);
    }
}
