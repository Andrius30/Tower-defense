using Pathfinding;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    MapGenerator mapGenerator;
    Seeker seeker;
    AIPath aiPath;
    Transform destinationPoint;


    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        destinationPoint = mapGenerator.EndPointObject.transform;
        seeker.StartPath(mapGenerator.StartPoints[0].position, destinationPoint.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            aiPath.destination = destinationPoint.position;  // Set the destination for AIPath
            aiPath.canMove = true;                   // Allow movement
        }
        else
        {
            Debug.LogError("Failed to find path: " + p.errorLog);
        }
    }
}
