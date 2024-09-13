using System.Collections.Generic;
using UnityEngine;

// TODO: Spawn enemies at every start point
// TODO: Load spawning data from level data
// TODO: 

public enum EnemyType
{
    Goblin,
    Orc,
    Troll,
    Dragon
    // Add more enemy types as needed
}

[System.Serializable]
public class EnemyTypeData
{
    public EnemyType enemyType;
    public int count;
    public float spawnInterval;  // Fixed typo from "spawnInterwal"
}

public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject orcPrefab;
    [SerializeField] private GameObject trollPrefab;
    [SerializeField] private GameObject dragonPrefab;

    private Dictionary<EnemyType, GameObject> prefabLookup;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        // Initialize the dictionary with enemy type to prefab mapping
        prefabLookup = new Dictionary<EnemyType, GameObject>
        {
            { EnemyType.Goblin, goblinPrefab },
            { EnemyType.Orc, orcPrefab },
            { EnemyType.Troll, trollPrefab },
            { EnemyType.Dragon, dragonPrefab }
        };
    }

    public void StartSpawn(List<EnemyTypeData> enemyTypeDataList)
    {
        foreach (var enemyTypeData in enemyTypeDataList)
        {
            StartCoroutine(SpawnEnemies(enemyTypeData));
        }
    }

    private IEnumerator<WaitForSeconds> SpawnEnemies(EnemyTypeData enemyTypeData)
    {
        GameObject prefab = GetPrefabForType(enemyTypeData.enemyType);
        if (prefab == null)
        {
            Debug.LogWarning($"No prefab found for enemy type: {enemyTypeData.enemyType}");
            yield break;
        }

        for (int i = 0; i < enemyTypeData.count; i++)
        {
            GameObject enemy = Instantiate(prefab, GetRandomSpawnPosition(), Quaternion.identity);
            spawnedEnemies.Add(enemy);

            // Wait for the interval before spawning the next enemy
            yield return new WaitForSeconds(enemyTypeData.spawnInterval);
        }
    }

    private GameObject GetPrefabForType(EnemyType enemyType)
    {
        return prefabLookup.TryGetValue(enemyType, out GameObject prefab) ? prefab : null;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Implement your logic to get a random spawn position
        // For now, it returns a placeholder position
        return Vector3.zero;
    }
}

