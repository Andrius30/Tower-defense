using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapSaverLoader : MonoBehaviour
{
    private string folderPath = "/SavedMaps/";
    string s = ".txt";

    public MapGenerator mapGenerator;

    [SerializeField] List<EnemySpawnerData> enemiesData;

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

        foreach (var data in enemiesData)
        {
            EnemySpawnerData spawnerData = new EnemySpawnerData
            {
                waveCount = data.waveCount,
                enemiesPerWave = data.enemiesPerWave,
                spawnInterval = data.spawnInterval,
                enemiesData = data.enemiesData,
            };
            mapData.enemySpawnerData = spawnerData;
        }

        // Konvertuojame į JSON ir išsaugome į failą
        string json = JsonUtility.ToJson(mapData, true);
        string filePath = $"{Application.dataPath}{folderPath}{fileName}{s}";
        File.WriteAllText(filePath, json);
        Debug.Log("Map saved to: " + filePath);
    }

    [Button("Load Map")]
    public void LoadMap(string pathToFile)
    {
        string filePath = $"{Application.dataPath}{folderPath}{pathToFile}{s}";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            MapData mapData = JsonUtility.FromJson<MapData>(json);

            mapGenerator.Clear();
            foreach (var obstacleData in mapData.obstacles)
            {
                GameObject obstacle = Instantiate(mapGenerator.obstaclePrefab, obstacleData.position, Quaternion.identity);
                mapGenerator.Obstacles.Add(obstacle);
            }
            mapGenerator.astar.Scan();
            foreach (var startPointData in mapData.startPoints)
            {
                GameObject startPoint = Instantiate(mapGenerator.startPointPrefab, startPointData.position, Quaternion.identity);
                mapGenerator.StartPoints.Add(startPoint.transform);
            }
            if (mapData.endPoint != null)
            {
                mapGenerator.EndPointObject = Instantiate(mapGenerator.endPointPrefab, mapData.endPoint.position, Quaternion.identity);
            }

            foreach (var path in mapData.pathData)
            {
                GameObject gm = Instantiate(mapGenerator.pathPrefab, path.position, Quaternion.identity);
                mapGenerator.PathObjects.Add(gm);
            }

            Debug.Log("Map loaded from: " + filePath);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
    public MapData LoadByFileIndex(int index)
    {
        string[] files = GetSaveFiles();
        Debug.Log($"Loaded files {files.Length}");
        if (index >= 0 && index < files.Length)
        {
            string filePath = files[index];
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<MapData>(json);
        }
        else
        {
            Debug.LogError("Invalid file index.");
            return default;
        }
    }
    // Get all save file paths
    string[] GetSaveFiles()
    {
        string filePath = $"{Application.dataPath}{folderPath}";
        if (Directory.Exists(filePath))
        {
            return Directory.GetFiles(filePath);
        }
        else
        {
            Debug.LogWarning("Save directory not found.");
            return new string[0];
        }
    }
}
