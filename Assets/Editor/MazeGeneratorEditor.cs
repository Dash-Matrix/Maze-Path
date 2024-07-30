using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MazeGenerator mazeGenerator = (MazeGenerator)target;
        if (GUILayout.Button("Generate Maze"))
        {
            mazeGenerator.GenerateMazeWrapper();
        }
        if (GUILayout.Button("Clear Maze"))
        {
            mazeGenerator.ClearMaze();
        }
    }
}
