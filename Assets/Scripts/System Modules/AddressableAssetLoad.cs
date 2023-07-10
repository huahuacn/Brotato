using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
 
public class AddressableAssetLoad
{
    private string tempKey;
    internal Dictionary<string, GameObject> cachePrefabDic = new Dictionary<string, GameObject>();

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
        //DebugTools.Log(asyncOperationHandle.Result.Count); 
    }

}
