using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PoolManager :  PersistenSingleton<PoolManager>
{
    // [SerializeField] Pool[] enemyPools;
    // [SerializeField] Pool[] playerProjectilePools;
    // [SerializeField] Pool[] enemyProjectilePools;
    // [SerializeField] Pool[] vFXPools;

    static Dictionary<GameObject, Pool> dictionary;

    protected override void Awake()
    {
        base.Awake();
        dictionary = new Dictionary<GameObject, Pool>();
    }

    // void LoadResource()
    // {
    //     Initialized(enemyPools);
    //     Initialized(playerProjectilePools);
    //     Initialized(enemyProjectilePools);
    //     Initialized(vFXPools); 
    // }

    // #if UNITY_EDITOR
    // void OnDestroy() 
    // {
    //     CheckPoolSize(enemyPools);
    //     CheckPoolSize(playerProjectilePools);
    //     CheckPoolSize(enemyProjectilePools);
    //     CheckPoolSize(vFXPools);
    // }
    // #endif

    void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning(
                    string.Format("Pool: {0} has a runtime size {1} bigger than its initial size {2}!",
                    pool.Prefab.name,
                    pool.RuntimeSize,
                    pool.Size)
                );
            }
        }
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
