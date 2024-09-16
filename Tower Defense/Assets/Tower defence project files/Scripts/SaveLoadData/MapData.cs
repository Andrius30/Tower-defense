using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class ObstacleData
{
    public Vector3 position;
}

[Serializable]
public class StartPointData
{
    public Vector3 position;
}

[Serializable]
public class EndPointData
{
    public Vector3 position;
}

[Serializable]
public class PathData
{
    public Vector3 position;
}
[System.Serializable]
public class EnemyData
{
    public AssetReference enemyReference;
}

[System.Serializable]
public class EnemySpawnerData
{
    public int waveCount;
    public int enemiesPerWave;
    public float spawnInterval;
    public List<EnemyData> enemiesData;
   
}

[Serializable]
public class MapData
{
    public List<ObstacleData> obstacles = new List<ObstacleData>();
    public List<StartPointData> startPoints = new List<StartPointData>();
    public List<PathData> pathData = new List<PathData>();
    public EnemySpawnerData enemySpawnerData;
    public EndPointData endPoint;
}
