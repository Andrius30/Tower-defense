using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

// TODO: Spawn enemies at every start point
// TODO: Load spawning data from level data
// TODO: 


public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private MapSaverLoader mapSaverLoader;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    /// <summary>
    /// Load from saved map data and spawn enemies random
    /// </summary>
    /// <param name="enemyTypeDataList"></param>
    public void StartSpawn()
    {
        MapData data = mapSaverLoader.LoadByFileIndex(GameManager.Instance.currentMapIndex);
        if(data == null)
        {
            Debug.LogError($"Data not loaded!");
            return;
        }
        for (int i = 0; i < data.enemySpawnerData.enemiesData.Count; i++)
        {
            EnemyData enemyData = data.enemySpawnerData.enemiesData[i];
            StartCoroutine(SpawnEnemies(enemyData.enemyReference, data.enemySpawnerData));
        }
    }
    IEnumerator<WaitForSeconds> SpawnEnemies(AssetReference enemyReference, EnemySpawnerData enemySpawnerData)
    {
        GameObject prefab = null;
        StartCoroutine(GetPrefabForType(enemyReference, e =>
        {
            prefab = e.gameObject;
            if (prefab == null)
            {
                Debug.LogWarning($"No prefab found for enemy type: {enemyReference}");
                return;
            }

        }));
        while(prefab == null)
        {
            yield return null;
        }
        for (int i = 0; i < enemySpawnerData.enemiesPerWave; i++)
        {
            GameObject enemy = Instantiate(prefab, GetRandomSpawnPosition(), Quaternion.identity);
            spawnedEnemies.Add(enemy);

            // Wait for the interval before spawning the next enemy
            yield return new WaitForSeconds(enemySpawnerData.spawnInterval);
        }
    }

    IEnumerator GetPrefabForType(AssetReference enemyReference, Action<GameObject> onSuccess)
    {
        AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(enemyReference);
        yield return asyncOperationHandle;
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            onSuccess?.Invoke(asyncOperationHandle.Result);
        }
    }
    Vector3 GetRandomSpawnPosition()
    {
        int random = Random.Range(0, mapGenerator.StartPoints.Count - 1);
        return mapGenerator.StartPoints[random].position;
    }
}

