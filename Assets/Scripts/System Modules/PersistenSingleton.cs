using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance {get; private set;}

    protected virtual void Awake() 
    {
        if (Instance == null) Instance = this as T;
        else if (Instance != null) Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // 不要摧毁参数所传入的对象
    }
}
