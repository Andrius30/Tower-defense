using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Andrius.Core.PoolingSystem
{
    [Serializable]
    public class ObjectPool
    {
        public GameObject prefab;
        Queue<GameObject> pool;
        [SerializeField] List<GameObject> visualList = new List<GameObject>();

        GameObject obj;
        string pathToParent;
        GameObject poolParent;

        public void InitializePool(MonoBehaviour mono, string pathToParent, GameObject prefab, Action onsuccess)
        {
            mono.StartCoroutine(InitializePool(prefab.name, pathToParent, onsuccess));
        }
        public IEnumerator InitializePool(string assetKey, string pathToParent, Action onSuccess)
        {
            if (pool == null)
            {
                pool = new Queue<GameObject>();
            }
            poolParent = GameObject.Find("Pool");
            if (poolParent == null)
            {
                poolParent = new GameObject("Pool");
            }
            if (string.IsNullOrEmpty(assetKey))
            {
                Debug.LogError($"Asset key null!");
                yield break;
            }
            if (!string.IsNullOrEmpty(pathToParent))
            {
                this.pathToParent = pathToParent;
            }
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(assetKey);
            yield return asyncOperationHandle;
            if (asyncOperationHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                prefab = asyncOperationHandle.Result;
                onSuccess.Invoke();
                Debug.Log($"Operation Succeed! Object {asyncOperationHandle.Result.name} loaded successfully.");
            }
            else
            {
                Debug.LogError($"[ERROR] When trying to load asset! Status: {asyncOperationHandle.Status}.");
            }
        }
        public IEnumerator InitializePool(AssetReference assetReference, string pathToParent, Action onSuccess)
        {
            if (pool == null)
            {
                pool = new Queue<GameObject>();
            }
            poolParent = GameObject.Find("Pool");
            if (poolParent == null)
            {
                poolParent = new GameObject("Pool");
            }
            if (!string.IsNullOrEmpty(pathToParent))
            {
                this.pathToParent = pathToParent;
            }
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            yield return asyncOperationHandle;
            if (asyncOperationHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                prefab = asyncOperationHandle.Result;
                Debug.Log($"Operation Succeed! Object {asyncOperationHandle.Result} loaded successfully.");
                onSuccess.Invoke();
            }
            else
            {
                Debug.LogError($"[ERROR] When trying to load asset! Status: {asyncOperationHandle.Status}.");
            }
        }
        public void PreloadAssetsToPool(int countToPreload) => AddToPool(countToPreload);
        public void AddToPool(int count)
        {
            if (pool.Count <= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Create();
                    if (obj != null)
                    {
                        obj.SetActive(false);
                        pool.Enqueue(obj);
                        visualList.Add(obj);
                    }
                }
            }
        }
        public GameObject GetFromPool()
        {
            if (prefab == null) return null;
            if (pool.Count == 0)
            {
                AddToPool(1);
            }
            if (pool.Count == 0)
            {
                Debug.LogError($"Pool count == 0");
                return null;
            }
            return pool.Dequeue();
        }
        public void ReturnToPool(GameObject obj)
        {
            pool.Enqueue(obj);
            obj.SetActive(false);
        }

        void Create()
        {
            GameObject parent = null;
            if (prefab == null)
            {
                Debug.LogError($"Prefab null!");
                return;
            }
            //Debug.Log($"Prefab to create {prefab.name}");
            var asyncHandle = Addressables.InstantiateAsync(prefab.name);
            if (asyncHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                obj = asyncHandle.Result;
            }
            else
            {
                Debug.LogError($"[ERROR] Status: {asyncHandle.Status}");
                return;
            }
            parent = GameObject.Find(pathToParent);
            if (parent == null)
            {
                parent = new GameObject(pathToParent);
            }
            parent.transform.SetParent(poolParent.transform);
            obj.transform.SetParent(parent.transform);
        }
    }
}
