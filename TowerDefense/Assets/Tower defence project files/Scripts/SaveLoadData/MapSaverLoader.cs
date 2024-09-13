using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;

public class MapSaverLoader : MonoBehaviour
{
    public MapGenerator mapGenerator;

    // TODO: Add Enemy Spawner data

    [Button("Save Map")]
    public void SaveMap(string fileName)
    {
        MapData mapData = new MapData();

        // Išsaugome kliūtis
        foreach (var obstacle in mapGenerator.Obstacles)
        {
            ObstacleData obstacleData = new ObstacleData
            {
                position = obstacle.transform.position
            };
            mapData.obstacles.Add(obstacleData);
        }

        // Išsaugome pradžios taškus
        foreach (var startPoint in mapGenerator.StartPoints)
        {
            StartPointData startPointData = new StartPointData
            {
                position = startPoint.position
            };
            mapData.startPoints.Add(startPointData);
        }

        // Išsaugome pabaigos tašką
        if (mapGenerator.EndPointObject != null)
        {
            mapData.endPoint = new EndPointData
            {
                position = mapGenerator.EndPointObject.transform.position
            };
        }

        foreach (var path in mapGenerator.PathObjects)
        {
            PathData pathData = new PathData
            {
                position = path.transform.position
            };
            mapData.pathData.Add(pathData);
        }

        // Konvertuojame į JSON ir išsaugome į failą
        string json = JsonUtility.ToJson(mapData, true);
        string filePath = $"{Application.dataPath}/SavedMaps/{fileName}.txt";
        File.WriteAllText(filePath, json);
        Debug.Log("Map saved to: " + filePath);
    }

    [Button("Load Map")]
    public void LoadMap(string pathToFile)
    {
        string filePath = $"{Application.dataPath}/SavedMaps/{pathToFile}.txt";
        if (File.Exists(filePath))
        {
            // Perskaitome failą ir konvertuojame į MapData objektą
            string json = File.ReadAllText(filePath);
            MapData mapData = JsonUtility.FromJson<MapData>(json);

            // Išvalome esamus objektus
            mapGenerator.Clear();

            // Atkuriame kliūtis
            foreach (var obstacleData in mapData.obstacles)
            {
                GameObject obstacle = Instantiate(mapGenerator.obstaclePrefab, obstacleData.position, Quaternion.identity);
                mapGenerator.Obstacles.Add(obstacle);
            }
            mapGenerator.astar.Scan();
            // Atkuriame pradžios taškus
            foreach (var startPointData in mapData.startPoints)
            {
                GameObject startPoint = Instantiate(mapGenerator.startPointPrefab, startPointData.position, Quaternion.identity);
                mapGenerator.StartPoints.Add(startPoint.transform);
            }

            // Atkuriame pabaigos tašką
            if (mapData.endPoint != null)
            {
                mapGenerator.EndPointObject = Instantiate(mapGenerator.endPointPrefab, mapData.endPoint.position, Quaternion.identity);
            }

            foreach (var path in mapData.pathData)
            {
                GameObject gm = Instantiate(mapGenerator.pathPrefab,path.position, Quaternion.identity);
                mapGenerator.PathObjects.Add(gm);
            }

            Debug.Log("Map loaded from: " + filePath);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}
