using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    static Dictionary<GameObject, Pool> dictionary;

    protected override void Awake()
    {
        base.Awake();
        dictionary = new Dictionary<GameObject, Pool>();
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