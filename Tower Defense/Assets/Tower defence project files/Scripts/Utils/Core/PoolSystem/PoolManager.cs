using Andrius.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Andrius.Core.PoolingSystem
{
    public class PoolManager : Singleton<PoolManager>
    {

        public Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

        public void CreatePool(string poolKey, string pathToParent, GameObject gm, Action onSuccess, bool preload = false, int count = 0)
        {
            ObjectPool pool = new ObjectPool();
            pool.InitializePool(this, pathToParent, gm, () =>
            {
                if (preload)
                    pool.PreloadAssetsToPool(count);
                pools.Add(poolKey, pool);
                onSuccess?.Invoke();
            });
        }
        public void CreatePool(string poolKey, string pathToParent, string assetName, Action onSuccess, bool preload = false, int count = 0)
        {
            ObjectPool pool = new ObjectPool();
            StartCoroutine(pool.InitializePool(assetName, pathToParent, () =>
             {
                 if (preload)
                     pool.PreloadAssetsToPool(count);
                 pools.Add(poolKey, pool);
                 onSuccess?.Invoke();
             }));
        }
        public void CreatePool(string poolKey, string pathToParent, AssetReference assetReference, Action onSuccess, bool preload = false, int count = 0)
        {
            ObjectPool pool = new ObjectPool();
            StartCoroutine(pool.InitializePool(assetReference, pathToParent, () =>
             {
                 if (preload)
                 {
                     pool.PreloadAssetsToPool(count);
                 }
                 pools.Add(poolKey, pool);
                 onSuccess?.Invoke();
             }));
        }
        public GameObject Get(string key, ref ObjectPool cachedPool)
        {
            ObjectPool pool = cachedPool;
            GameObject gm = null;
            if (cachedPool == null)
            {
                if (pools.TryGetValue(key, out pool))
                {
                    cachedPool = pool;
                    gm = GetFromPool(pool);
                    return gm;
                }
            }
            else
            {
                gm = GetFromPool(pool);
                return gm;
            }
            return null;
        }

        GameObject GetFromPool(ObjectPool pool)
        {
            var gm = pool.GetFromPool();
            gm.SetActive(true);
            return gm;
        }

        public void ReturnToPool(string key, GameObject gm, ref ObjectPool cachedPool)
        {
            ObjectPool pool = cachedPool;
            if (cachedPool == null)
            {
                if (pools.TryGetValue(key, out pool))
                {
                    cachedPool = pool;
                    pool.ReturnToPool(gm);
                }
            }
            else
            {
                pool.ReturnToPool(gm);
            }
        }
    }
}