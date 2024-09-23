using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class TowerLoaderUI : MonoBehaviour
{
    public string label = "Tower_Prefabs"; // Label for the Towers prefabs

    [SerializeField] Transform content;
    [SerializeField] GameObject towerUIPrefab;

    List<GameObject> loadedTowers = new List<GameObject>();

    void Start()
    {
        LoadAllTowers();
        // LoadTowerByAddress("Assets/Tower defence project files/Prefabs/Towers/Test_1_Tower.prefab");
    }

    void LoadAllTowers()
    {
        // Load all towers that match the label
        Addressables.LoadAssetsAsync<GameObject>(label, null).Completed += OnLoadCompleted;
    }
    void LoadTowerByAddress(string address)
    {
        Addressables.LoadAssetAsync<GameObject>(address).Completed += OnTowerLoaded;
    }

    private void OnTowerLoaded(AsyncOperationHandle<GameObject> handle)
    {
        Debug.Log($" {handle.Status}");
    }

    // Callback when all assets have been loaded
    void OnLoadCompleted(AsyncOperationHandle<IList<GameObject>> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("All tower prefabs loaded. Total count: " + obj.Result.Count);

            foreach (var towerPrefab in obj.Result)
            {
                // Instantiate the UI for each loaded tower
                LoadTowerToUI(towerPrefab);
            }
        }
        else
        {
            Debug.LogError("[OnLoadCompleted] Failed to load tower prefabs.");
        }
    }

    void LoadTowerToUI(GameObject towerObj)
    {
        // Assuming each tower object has a script with tower data
        TowerData data = towerObj.GetComponent<BaseTower>().towerData;
        if (data != null)
        {
            GameObject uiElement = Instantiate(towerUIPrefab, content);
            uiElement.GetComponentInChildren<Image>().sprite = data.towerSprite; // Assuming towerSprite is a sprite in TowerData
            Debug.Log("UI element created for tower: " + data.towerName);
        }
        else
        {
            Debug.LogError("Tower object missing TowerData component.");
        }
    }
}
