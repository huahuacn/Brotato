using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.Entities;
using UnityEngine.Events;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject RandomEnemy => enemyList.Count == 0 ? null : enemyList[UnityEngine.Random.Range(0, enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;
    [SerializeField] bool spawnEnemy = true; // 产生敌人开关
    [SerializeField] GameObject waveUI;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] float timeBetweenSpawns = 1f; // 敌人生成时间间隔
    [SerializeField] float timeBetweenWaves = 1f; // 等待下一波时间
    [SerializeField] int minEnemyAmount = 4;
    [SerializeField] int maxEnemyAmount = 10;

    public event UnityAction enemyLoad = delegate{};

    int waveNumber = 1;
    int enemyAmount;

    List<GameObject> enemyList;

    WaitForSeconds waitTimeBetweenSpawns; // 等待生成间隔时间
    WaitForSeconds waitTimeBetweenWaves; // 等待下一波
    WaitUntil waitUnitlNoEnemy;

    NativeArray<float3> TargetPositions;
    NativeArray<float3> SeekerPositions;
    NativeArray<float3> NearestTargetPositions;

    protected override void Awake()
    {
        base.Awake();

        enemyList = new List<GameObject>();

        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
        waitUnitlNoEnemy = new WaitUntil(() => enemyList.Count == 0);

        
        OnResLoadAsset("Enemy", "Enemy");
    }

    IEnumerator Start() 
    {
        while (spawnEnemy)
        {
            yield return waitUnitlNoEnemy; // 检测敌人数量=0，产生敌人

            // waveUI.SetActive(true);

            yield return waitTimeBetweenWaves; // 等待下一波

            // waveUI.SetActive(false);

            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
    }

    IEnumerator RandomlySpawnCoroutine()
    {        
        enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmount);

        for (int i = 0; i < enemyAmount; i++)
        {
            enemyList.Add(PoolManager.Release(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)]));
        }

        yield return waitTimeBetweenSpawns;

        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);


    [System.Obsolete]
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
            p.Size = 100;
            pool[i] = p;
        }

        PoolManager.Instance.Initialized(pool);

        enemyLoad.Invoke();
    }
}
