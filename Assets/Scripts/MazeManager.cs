using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    public Transform validEntryPoint;
    private List<Transform> entryPoints;

    private void Start()
    {
        entryPoints = FindObjectOfType<MazeGenerator>().GetEntryPoints();
    }

    private void Update()
    {
        for (int i = 0; i < entryPoints.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                CheckEntry(entryPoints[i].position);
            }
        }
    }

    private void CheckEntry(Vector3 entryPosition)
    {
        if (entryPosition == validEntryPoint.position)
        {
            Debug.Log("Correct Entry Point!");
        }
        else
        {
            Debug.Log("Incorrect Entry Point!");
        }
    }
}
