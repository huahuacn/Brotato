using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
 
public class AddressableManager : Singleton<AddressableManager>
{
    static Dictionary<GameObject, Pool> dictionary;

    public bool EnemyLoaded;
    public GameObject RandomEnemyPrefabs => enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
    GameObject[] enemyPrefabs;

    public bool PlayerProjectilesed;
    GameObject[] playerProjectilesPrefabs;
    

    protected override void Awake()
    {
        base.Awake();
        dictionary = new Dictionary<GameObject, Pool>();
    }

    [Obsolete]
    void Start()
    {
        OnResLoadAsset("Enemy", "Enemy");
        OnResLoadAsset("PlayerProjectile", "Projectile");
    }

    void LoadAssetAsync(string key)
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
    private void OnResLoadAsset(string key, string lable) 
    { 
        Addressables.LoadAssetsAsync<GameObject>(
            new List<object> { key, lable }, 
            null, 
            Addressables.MergeMode.Intersection).Completed += OnCompleteLoadAssets; 
    } 

    private void OnCompleteLoadAssets(AsyncOperationHandle<IList<GameObject>> asyncOperationHandle) 
    { 
        enemyPrefabs = new GameObject[asyncOperationHandle.Result.Count];
        Pool[] pool = new Pool[asyncOperationHandle.Result.Count];

        for (int i=0; i < asyncOperationHandle.Result.Count; i ++)
        {
            GameObject go = asyncOperationHandle.Result[i].gameObject;

            if (go == null) continue;

            enemyPrefabs[i] = go;

            Pool p = new Pool();
            p.Prefab =  go;
            p.Size = 10;
            pool[i] = p;
        }

        Initialized(pool);

        EnemyLoaded = true;
    }


    public void Initialized(Pool[] pools) 
    {
        foreach (var pool in pools)
        {
            #if UNITY_EDITOR
                if (dictionary.ContainsKey(pool.Prefab)) 
                {
                    Debug.LogError("Same prefab in multiple pool! Prefab: " + pool.Prefab.name);
                    continue;
                }
            #endif

            dictionary.Add(pool.Prefab, pool);

            Transform poolParent = new GameObject("Pool: " + pool.Prefab.name).transform;

            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }


    public static GameObject Release(GameObject prefab)
    {
        return dictionary[prefab].PreparedObject();
    }

    public static GameObject Release(GameObject prefab, Vector3 position)
    {
        return dictionary[prefab].PreparedObject(position);
    }

    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return dictionary[prefab].PreparedObject(position, rotation);
    }

    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        return dictionary[prefab].PreparedObject(position, rotation, localScale);
    }
}
