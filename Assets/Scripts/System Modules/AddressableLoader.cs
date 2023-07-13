using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
 
public class AddressableLoader 
{
    public bool Loaded;
    public string Key;
    public string Lable;
    public GameObject RandomPrefabs => prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
    GameObject[] prefabs;

    [Obsolete]
    public virtual void LoadAssetAsync(string key)
    {
        Addressables.LoadAssetAsync<GameObject>(key).Completed += (handle) =>
        {
            // 预制体
            GameObject prefabObj = handle.Result;
            // 实例化
        };
    }

    // 加载单个资源
    private void OnResLoadAsset(string key) 
    { 
        Addressables.LoadAssetAsync<GameObject>(key).Completed += OnCompleteLoad;
    } 

    private void OnCompleteLoad(AsyncOperationHandle<GameObject> asyncOperationHandle) 
    { 
        GameObject go = GameObject.Instantiate(asyncOperationHandle.Result); 
    }
    private void OnResInstantiate(string key) 
    { 
        Addressables.InstantiateAsync(key); 
    }

    // 加载多个资源
    [Obsolete]
    public void OnResLoadAsset(string key, string lable) 
    { 
        Addressables.LoadAssetsAsync<GameObject>(
            new List<object> { key, lable }, 
            null, 
            Addressables.MergeMode.Intersection).Completed += OnCompleteLoadAssets; 
    } 

    public virtual void OnCompleteLoadAssets(AsyncOperationHandle<IList<GameObject>> asyncOperationHandle) 
    { 
        prefabs = new GameObject[asyncOperationHandle.Result.Count];
        Pool[] pool = new Pool[asyncOperationHandle.Result.Count];

        for (int i=0; i < asyncOperationHandle.Result.Count; i ++)
        {
            GameObject go = asyncOperationHandle.Result[i].gameObject;

            if (go == null) continue;

            prefabs[i] = go;

            Pool p = new Pool();
            p.Prefab =  go;
            p.Size = 10;
            pool[i] = p;
        }

        PoolManager.Instance.Initialized(pool);

        Loaded = true;
    }
}
