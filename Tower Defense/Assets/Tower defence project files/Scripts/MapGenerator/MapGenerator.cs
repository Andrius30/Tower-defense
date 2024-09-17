using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using Random = UnityEngine.Random;
using System.Collections;

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
    public int pathWidth = 2; // Width of the path in nodes
    public float obstacleDistanceFromPath = 2f; // Adjustable distance between obstacles and the path

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
    List<Vector3> reservedPositions = new List<Vector3>();

    void Start() => StartCoroutine(InitializeMap());

    public void Clear() => ClearMap();

    [Button("Clear and Regenerate Map")]
    void ClearAndRegenerateMap()
    {
        ClearMap();
        StartCoroutine(InitializeMap());
    }

    IEnumerator InitializeMap()
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
        yield return new WaitForSeconds(1);
        GeneratePaths(() =>
        {
            onMapGenerated?.Invoke();
        });
    }

    void ClearMap()
    {
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
        reservedPositions.Clear();
    }

    void PlaceStartPointsInFirstColumn()
    {
        for (int i = 0; i < numberOfPaths; i++)
        {
            bool placed = false;
            int attempts = 0;
            const int maxAttempts = 100;

            while (!placed && attempts < maxAttempts)
            {
                attempts++;
                int randomY = Random.Range(0, gridGraph.depth);
                int x = 0;

                GraphNode node = gridGraph.GetNode(x, randomY);

                if (node != null)
                {
                    Vector3 nodePosition = (Vector3)node.position;

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
                        reservedPositions.Add(gm.transform.position);
                        placed = true;
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
        int randomZ = Random.Range(0, gridGraph.depth);
        int x = gridGraph.width - 1;

        GraphNode node = gridGraph.GetNode(x, randomZ);

        if (node != null)
        {
            Vector3 nodePosition = (Vector3)node.position;
            endPointObj = Instantiate(endPointPrefab, nodePosition, Quaternion.identity);
            reservedPositions.Add(nodePosition);
        }
        else
        {
            Debug.LogError("No walkable node found in the last column.");
        }
    }

    void GenerateMap()
    {
        foreach (Transform startPoint in startPoints)
        {
            reservedPositions.Add(startPoint.position);
        }
        if (endPointObj != null)
        {
            reservedPositions.Add(endPointObj.transform.position);
        }
        int maxTry = 1000;
        for (int i = 0; i < maxObstacles; i++)
        {
            int randomIndex = Random.Range(0, nodes.Count);
            Vector3 position = (Vector3)nodes[randomIndex].position;
            if (maxTry <= 0) return;
            // Ensure obstacle is at a minimum distance from the path
            if (!reservedPositions.Contains(position) && IsFarEnoughFromPath(position))
            {
                Debug.Log($"IF");
                var node = astar.GetNearest(position);
                GameObject obstacle = Instantiate(obstaclePrefab, new Vector3(position.x, 0, position.z), Quaternion.identity);
                obstacles.Add(obstacle);
                reservedPositions.Add(position);
                node.node.Walkable = false;
            }
            else
            {
                Debug.Log($"ELSE");
                i--;
                maxTry--;
            }
        }

        foreach (var item in astar.ScanAsync())
        {
            Debug.Log($"Progress {item.progress}");
        }
    }

    bool IsFarEnoughFromPath(Vector3 obstaclePosition)
    {
        foreach (Vector3 reservedPos in reservedPositions)
        {
            if (Vector3.Distance(obstaclePosition, reservedPos) < obstacleDistanceFromPath)
            {
                return false; // Too close to the path
            }
        }
        return true; // Far enough from the path
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
            foreach (Vector3 point in path.vectorPath)
            {
                GridNode node = gridGraph.GetNearest(point).node as GridNode;
                if (node != null)
                {
                    reservedPositions.Add((Vector3)node.position);
                    node.Walkable = true;

                    // Reserve nearby nodes to make path wider
                    ReserveNearbyNodes((Vector3)node.position);
                }
            }
            VisualizePath(path);
        }
        else
        {
            Debug.LogError("No path found. Path error: " + path.errorLog);
        }
    }

    void ReserveNearbyNodes(Vector3 center)
    {
        for (int x = -pathWidth; x <= pathWidth; x++)
        {
            for (int z = -pathWidth; z <= pathWidth; z++)
            {
                Vector3 nearbyPos = center + new Vector3(x * gridGraph.nodeSize, 0, z * gridGraph.nodeSize);
                GraphNode nearbyNode = gridGraph.GetNearest(nearbyPos).node;
                if (nearbyNode != null)
                {
                    reservedPositions.Add((Vector3)nearbyNode.position);
                    nearbyNode.Walkable = true;
                }
            }
        }
    }

    void VisualizePath(Path path)
    {
        for (int i = 0; i < path.vectorPath.Count - 1; i++)
        {
            Vector3 startPoint = path.vectorPath[i];
            Vector3 endPoint = path.vectorPath[i + 1];

            // Create the central path visual
            GameObject pathObj = Instantiate(pathPrefab, startPoint, Quaternion.identity);
            pathObjects.Add(pathObj);

            // Create additional visuals for the wider path
            for (int x = -pathWidth; x <= pathWidth; x++)
            {
                for (int z = -pathWidth; z <= pathWidth; z++)
                {
                    if (x == 0 && z == 0) continue; // Skip the central path
                    Vector3 sidePoint = startPoint + new Vector3(x * gridGraph.nodeSize, 0, z * gridGraph.nodeSize);
                    GameObject sidePathObj = Instantiate(pathPrefab, sidePoint, Quaternion.identity);
                    pathObjects.Add(sidePathObj);
                }
            }
        }
    }
}
