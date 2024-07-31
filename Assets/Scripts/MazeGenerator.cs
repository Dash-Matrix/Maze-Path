using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PriorityQueue<T>
{
    private List<KeyValuePair<T, int>> elements = new List<KeyValuePair<T, int>>();

    public int Count => elements.Count;

    public void Enqueue(T item, int priority)
    {
        elements.Add(new KeyValuePair<T, int>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance;

    public LevelManager level;

    public int mazeWidth, mazeHeight;
    public GameObject cellPrefab;
    public GameObject cellFloorPrefab;
    private MazeCell[,] mazeCells;

    public int numberOfEntryPoints = 3; // Number of entry points
    public GameObject entryPointPrefab; // Prefab for entry points
    private List<Transform> entryPoints = new List<Transform>();
    private Transform validEntryPoint;
    public GameObject goalPrefab; // Prefab for the goal
    public Transform PP;

    public GameObject playerPrefab; // Prefab for player
    public Vector3 playerOffset; // Offset for player position

    private List<GameObject> spawnedObjects = new List<GameObject>(); // Track spawned objects for cleanup

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateMazeWrapper()
    {
        ClearMaze();
        GenerateMaze();
        SetEntryPoints();
        CreateGoal();
        SpawnPlayers(); // Spawn players after setting entry points and goals
    }

    private void GenerateMaze()
    {
        mazeCells = new MazeCell[mazeWidth, mazeHeight];

        // Calculate the offset to center the maze
        Vector3 mazeOffset = new Vector3(-mazeWidth / 2f, 0, -mazeHeight / 2f);

        // Initialize the maze cells
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Vector3 cellPosition = new Vector3(x, 0, y) + mazeOffset;
                GameObject newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                mazeCells[x, y] = newCell.GetComponent<MazeCell>();
                spawnedObjects.Add(newCell); // Track spawned objects
            }
        }

        // Start generating the maze from the center
        GenerateMazeRecursive(mazeWidth / 2, mazeHeight / 2);
    }


    private void GenerateMazeRecursive(int x, int y)
    {
        mazeCells[x, y].IsVisited = true;
        List<int> directions = new List<int> { 0, 1, 2, 3 };
        Shuffle(directions);

        foreach (int direction in directions)
        {
            int nx = x, ny = y;

            switch (direction)
            {
                case 0: // North
                    if (IsInBounds(x, y + 1) && !mazeCells[x, y + 1].IsVisited)
                    {
                        ny++;
                    }
                    break;
                case 1: // East
                    if (IsInBounds(x + 1, y) && !mazeCells[x + 1, y].IsVisited)
                    {
                        nx++;
                    }
                    break;
                case 2: // South
                    if (IsInBounds(x, y - 1) && !mazeCells[x, y - 1].IsVisited)
                    {
                        ny--;
                    }
                    break;
                case 3: // West
                    if (IsInBounds(x - 1, y) && !mazeCells[x - 1, y].IsVisited)
                    {
                        nx--;
                    }
                    break;
            }

            if (nx != x || ny != y) // If we found a valid neighbor
            {
                RemoveWallBetween(mazeCells[x, y], mazeCells[nx, ny], direction);
                GenerateMazeRecursive(nx, ny);
            }
        }
    }

    private void RemoveWallBetween(MazeCell current, MazeCell next, int direction)
    {
        switch (direction)
        {
            case 0: // North
                current.RemoveWall(current.NorthWall);
                next.RemoveWall(next.SouthWall);
                break;
            case 1: // East
                current.RemoveWall(current.EastWall);
                next.RemoveWall(next.WestWall);
                break;
            case 2: // South
                current.RemoveWall(current.SouthWall);
                next.RemoveWall(next.NorthWall);
                break;
            case 3: // West
                current.RemoveWall(current.WestWall);
                next.RemoveWall(next.EastWall);
                break;
        }
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < mazeWidth && y >= 0 && y < mazeHeight;
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // Swap
        }
    }

    private void SetEntryPoints()
    {
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();
        Vector2Int center = new Vector2Int(mazeWidth / 2, mazeHeight / 2);
        Vector2Int validEntryPosition = Vector2Int.zero;
        int shortestDistance = int.MaxValue;

        Vector3 mazeOffset = new Vector3(-mazeWidth / 2f, 0, -mazeHeight / 2f);

        for (int i = 0; i < numberOfEntryPoints; i++)
        {
            Vector3 position = Vector3.zero;
            Vector2Int cellPosition = Vector2Int.zero;

            while (true)
            {
                int edge = Random.Range(0, 4); // 0 = top, 1 = right, 2 = bottom, 3 = left
                switch (edge)
                {
                    case 0: // Top edge
                        cellPosition = new Vector2Int(Random.Range(0, mazeWidth), mazeHeight - 1);
                        break;
                    case 1: // Right edge
                        cellPosition = new Vector2Int(mazeWidth - 1, Random.Range(0, mazeHeight));
                        break;
                    case 2: // Bottom edge
                        cellPosition = new Vector2Int(Random.Range(0, mazeWidth), 0);
                        break;
                    case 3: // Left edge
                        cellPosition = new Vector2Int(0, Random.Range(0, mazeHeight));
                        break;
                }

                if (!usedPositions.Contains(cellPosition))
                {
                    usedPositions.Add(cellPosition);
                    position = new Vector3(cellPosition.x, 0, cellPosition.y) + mazeOffset;
                    break;
                }
            }

            int distance = GetShortestPathDistance(cellPosition, center);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                validEntryPosition = cellPosition;
            }

            RemoveWallForEntryPoint(cellPosition.x, cellPosition.y);
            GameObject entryPoint = Instantiate(entryPointPrefab, position, Quaternion.identity, PP);
            entryPoints.Add(entryPoint.transform);
            spawnedObjects.Add(entryPoint); // Track spawned objects
        }

        // Set the valid entry point
        Vector3 validPosition = new Vector3(validEntryPosition.x, 0, validEntryPosition.y) + mazeOffset;
        validEntryPoint = Instantiate(entryPointPrefab, validPosition, Quaternion.identity, PP).transform;
        RemoveWallForEntryPoint(validEntryPosition.x, validEntryPosition.y);
        validEntryPoint.name = "validEntryPoint";
        spawnedObjects.Add(validEntryPoint.gameObject); // Track spawned objects
    }


    private int GetShortestPathDistance(Vector2Int start, Vector2Int goal)
    {
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        PriorityQueue<Vector2Int> priorityQueue = new PriorityQueue<Vector2Int>();

        distances[start] = 0;
        priorityQueue.Enqueue(start, 0);

        while (priorityQueue.Count > 0)
        {
            Vector2Int current = priorityQueue.Dequeue();

            if (current == goal)
            {
                return distances[current];
            }

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                int newDistance = distances[current] + 1;
                if (!distances.ContainsKey(neighbor) || newDistance < distances[neighbor])
                {
                    distances[neighbor] = newDistance;
                    priorityQueue.Enqueue(neighbor, newDistance);
                }
            }
        }

        return int.MaxValue; // Should not happen if there's a valid path
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (IsInBounds(cell.x, cell.y + 1) && !mazeCells[cell.x, cell.y].NorthWall.activeSelf)
            neighbors.Add(new Vector2Int(cell.x, cell.y + 1));
        if (IsInBounds(cell.x + 1, cell.y) && !mazeCells[cell.x, cell.y].EastWall.activeSelf)
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (IsInBounds(cell.x, cell.y - 1) && !mazeCells[cell.x, cell.y].SouthWall.activeSelf)
            neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (IsInBounds(cell.x - 1, cell.y) && !mazeCells[cell.x, cell.y].WestWall.activeSelf)
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y));

        return neighbors;
    }

    private void RemoveWallForEntryPoint(int x, int y)
    {
        // Only remove the wall corresponding to the edge or corner
        if (x == 0 && y == 0) // Bottom-left corner
        {
            mazeCells[x, y].RemoveWall(mazeCells[x, y].WestWall); // Remove West Wall
            mazeCells[x, y].RemoveWall(mazeCells[x, y].SouthWall); // Remove South Wall
        }
        else if (x == mazeWidth - 1 && y == 0) // Bottom-right corner
        {
            mazeCells[x, y].RemoveWall(mazeCells[x, y].EastWall); // Remove East Wall
            mazeCells[x, y].RemoveWall(mazeCells[x, y].SouthWall); // Remove South Wall
        }
        else if (x == 0 && y == mazeHeight - 1) // Top-left corner
        {
            mazeCells[x, y].RemoveWall(mazeCells[x, y].WestWall); // Remove West Wall
            mazeCells[x, y].RemoveWall(mazeCells[x, y].NorthWall); // Remove North Wall
        }
        else if (x == mazeWidth - 1 && y == mazeHeight - 1) // Top-right corner
        {
            mazeCells[x, y].RemoveWall(mazeCells[x, y].EastWall); // Remove East Wall
            mazeCells[x, y].RemoveWall(mazeCells[x, y].NorthWall); // Remove North Wall
        }
        else if (x == 0) // Left edge (not a corner)
        {
            mazeCells[x, y].RemoveWall(mazeCells[x, y].WestWall); // Remove West Wall
        }
        else if (x == mazeWidth - 1) // Right edge (not a corner)
        {
            mazeCells[x, y].RemoveWall(mazeCells[x, y].EastWall); // Remove East Wall
        }
        else if (y == 0) // Bottom edge (not a corner)
        {
            mazeCells[x, y].RemoveWall(mazeCells[x, y].SouthWall); // Remove South Wall
        }
        else if (y == mazeHeight - 1) // Top edge (not a corner)
        {
            mazeCells[x, y].RemoveWall(mazeCells[x, y].NorthWall); // Remove North Wall
        }
    }

    private void CreateGoal()
    {
        Vector3 centerPosition = new Vector3(mazeWidth / 2, 0, mazeHeight / 2) + new Vector3(-mazeWidth / 2f, 0, -mazeHeight / 2f);
        GameObject goal = Instantiate(goalPrefab, centerPosition, Quaternion.identity, PP);
        level.target = goal.transform;
        goal.name = "Center Goal";
        spawnedObjects.Add(goal); // Track spawned objects
    }


    private void SpawnPlayers()
    {
        Vector3 mazeOffset = new Vector3(-mazeWidth / 2f, 0, -mazeHeight / 2f);
        int ids = 0;
        // Spawn players at all entry points
        foreach (Transform entry in entryPoints)
        {
            Vector3 playerPosition = entry.position + playerOffset;

            // Determine the direction of the entry point and adjust the position accordingly
            Vector2Int cellPosition = new Vector2Int(Mathf.RoundToInt(entry.position.x - mazeOffset.x), Mathf.RoundToInt(entry.position.z - mazeOffset.z));

            if (cellPosition.y == mazeHeight - 1) // Top edge
            {
                playerPosition += new Vector3(0, 0, 1) + playerOffset; // Move the player forward (into the maze)
            }
            else if (cellPosition.x == mazeWidth - 1) // Right edge
            {
                playerPosition += new Vector3(1, 0, 0) + playerOffset; // Move the player right
            }
            else if (cellPosition.y == 0) // Bottom edge
            {
                playerPosition += new Vector3(0, 0, -1) + playerOffset; // Move the player backward
            }
            else if (cellPosition.x == 0) // Left edge
            {
                playerPosition += new Vector3(-1, 0, 0) + playerOffset; // Move the player left
            }

            GameObject player = Instantiate(playerPrefab, playerPosition, Quaternion.identity, PP);
            player.GetComponent<PlayerMovement>().ID = ids;
            GameObject cell = Instantiate(cellFloorPrefab, playerPosition - playerOffset * 2, Quaternion.identity, transform);
            cell.GetComponent<MazeCell>().SetNumber(ids);
            spawnedObjects.Add(player); // Track spawned objects
            spawnedObjects.Add(cell); // Track spawned objects

            ids++;
        }


        /*
        // Spawn player at the valid entry point
        Vector3 validPlayerPosition = validEntryPoint.position + playerOffset;

        Vector2Int validCellPosition = new Vector2Int(Mathf.RoundToInt(validEntryPoint.position.x - mazeOffset.x), Mathf.RoundToInt(validEntryPoint.position.z - mazeOffset.z));
        if (validCellPosition.y == mazeHeight - 1) // Top edge
        {
            validPlayerPosition += new Vector3(0, 0, 1) + playerOffset; // Move the player forward
        }
        else if (validCellPosition.x == mazeWidth - 1) // Right edge
        {
            validPlayerPosition += new Vector3(1, 0, 0) + playerOffset; // Move the player right
        }
        else if (validCellPosition.y == 0) // Bottom edge
        {
            validPlayerPosition += new Vector3(0, 0, -1) + playerOffset; // Move the player backward
        }
        else if (validCellPosition.x == 0) // Left edge
        {
            validPlayerPosition += new Vector3(-1, 0, 0) + playerOffset; // Move the player left
        }

        GameObject validPlayer = Instantiate(playerPrefab, validPlayerPosition, Quaternion.identity, PP);
        spawnedObjects.Add(validPlayer); // Track spawned objects

        */
    }


    public void ClearMaze()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            DestroyImmediate(obj);
        }
        spawnedObjects.Clear();
        entryPoints.Clear();
    }

    public MazeCell[,] GetMazeCells()
    {
        return mazeCells;
    }

    public List<Transform> GetEntryPoints()
    {
        return entryPoints;
    }
}
