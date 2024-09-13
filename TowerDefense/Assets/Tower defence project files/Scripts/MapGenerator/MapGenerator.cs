using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public static Action onMapGenerated;

    public GameObject obstaclePrefab;
    public GameObject startPointPrefab;
    public GameObject endPointPrefab;
    public GameObject pathPrefab;
    public int maxObstacles = 10;
    public int numberOfPaths = 3;
    public float minDistanceBetweenStartPoints = 5f; // Adjustable distance in Inspector
    public bool generateObstacles = true;  // Option to generate obstacles

    public List<Transform> StartPoints => startPoints;
    public List<GameObject> PathObjects => pathObjects;
    public List<GameObject> Obstacles => obstacles;
    public GameObject EndPointObject { get => endPointObj; set => endPointObj = value; }

    public AstarPath astar;

    List<GraphNode> nodes;
    List<Transform> startPoints = new List<Transform>();
    List<GameObject> obstacles = new List<GameObject>();
    List<GameObject> pathObjects = new List<GameObject>();

    GridGraph gridGraph;
    GameObject endPointObj;

    int width;
    int height;

    void Start() => InitializeMap();

    public void Clear() => ClearMap();

    [Button("Clear and Regenerate Map")]
    void ClearAndRegenerateMap()
    {
        ClearMap();
        InitializeMap();
    }

    void InitializeMap()
    {
        gridGraph = AstarPath.active.data.gridGraph;
        width = gridGraph.width;
        height = gridGraph.depth;

        PlaceStartPointsInFirstColumn();
        PlaceEndPointInLastColumn();

        nodes = new List<GraphNode>();
        gridGraph.GetNodes(node => nodes.Add(node));

        if (generateObstacles)
        {
            GenerateMap();
        }
        astar.Scan();  // Re-scan the grid with obstacles and points
        GeneratePaths(() =>
        {
            onMapGenerated?.Invoke();
        });
    }

    void ClearMap()
    {
        // Remove all obstacles, start points, and end points from the scene
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        foreach (Transform startPoint in startPoints)
        {
            Destroy(startPoint.gameObject);
        }

        if (endPointObj != null)
        {
            Destroy(endPointObj);
        }

        foreach (GameObject path in pathObjects)
        {
            Destroy(path);
        }
        obstacles.Clear();
        startPoints.Clear();
        pathObjects.Clear();
    }

    void PlaceStartPointsInFirstColumn()
    {
        for (int i = 0; i < numberOfPaths; i++)
        {
            bool placed = false;
            int attempts = 0;
            const int maxAttempts = 100;  // Maximum attempts to place a start point

            while (!placed && attempts < maxAttempts)
            {
                attempts++;
                int randomY = Random.Range(0, gridGraph.depth);  // Random Y position within grid height
                int x = 0;  // First column (x == 0)

                // Get node at this position
                GraphNode node = gridGraph.GetNode(x, randomY);

                if (node != null/* && node.Walkable*/)
                {
                    Vector3 nodePosition = (Vector3)node.position;

                    // Check if this position is at least `minDistanceBetweenStartPoints` away from existing start points
                    bool tooClose = false;
                    foreach (Transform existingStartPoint in startPoints)
                    {
                        if (Vector3.Distance(existingStartPoint.position, nodePosition) < minDistanceBetweenStartPoints)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        GameObject gm = Instantiate(startPointPrefab, nodePosition, Quaternion.identity);
                        startPoints.Add(gm.transform);
                        placed = true;  // Successfully placed the start point
                    }
                }
                else
                {
                    Debug.LogError("No walkable node found in the first column.");
                }
            }

            if (!placed)
            {
                Debug.LogWarning("Failed to place a start point after maximum attempts.");
            }
        }
    }
    void PlaceEndPointInLastColumn()
    {
        int randomZ = Random.Range(0, gridGraph.depth);  // Random Y position within grid height
        int x = gridGraph.width - 1;  // Last column (x == width - 1)

        // Get node at this position
        GraphNode node = gridGraph.GetNode(x, randomZ);

        if (node != null/* && node.Walkable*/)
        {
            Vector3 nodePosition = (Vector3)node.position;
            endPointObj = Instantiate(endPointPrefab, nodePosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No walkable node found in the last column.");
        }
    }

    void GenerateMap()
    {
        HashSet<Vector3> reservedPositions = new HashSet<Vector3>();
        foreach (Transform startPoint in startPoints)
        {
            reservedPositions.Add(startPoint.position);
        }
        if (endPointObj != null)
        {
            reservedPositions.Add(endPointObj.transform.position);
        }

        for (int i = 0; i < maxObstacles; i++)
        {
            int randomIndex = Random.Range(0, nodes.Count);
            Vector3 position = (Vector3)nodes[randomIndex].position;

            // Ensure position is not reserved
            if (!reservedPositions.Contains(position))
            {
                GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);
                obstacles.Add(obstacle);
                reservedPositions.Add(position);
            }
        }
    }

    void GeneratePaths(Action onDone)
    {
        foreach (Transform startPoint in startPoints)
        {
            Seeker seeker = startPoint.GetComponent<Seeker>();
            if (seeker == null)
            {
                Debug.LogError("Seeker component is missing on the start point!");
                continue;
            }

            seeker.StartPath(startPoint.position, endPointObj.transform.position, OnPathComplete);
        }
        onDone?.Invoke();
    }

    void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            Debug.Log("Path found from start to end!");
            foreach (Vector3 point in path.vectorPath)
            {
                GridNode node = gridGraph.GetNearest(point).node as GridNode;
                if (node != null)
                {
                    node.Walkable = true;
                }
            }
            VisualizePath(path);
        }
        else
        {
            Debug.LogError("No path found. Path error: " + path.errorLog);
        }
    }

    void VisualizePath(Path path)
    {
        for (int i = 0; i < path.vectorPath.Count - 1; i++)
        {
            Vector3 startPoint = path.vectorPath[i];
            Vector3 endPoint = path.vectorPath[i + 1];

            GameObject pathObj = Instantiate(pathPrefab, startPoint, Quaternion.identity);
            pathObjects.Add(pathObj);
        }
    }
}
