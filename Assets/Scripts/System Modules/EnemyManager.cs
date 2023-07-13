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
using UnityEngine.Jobs;

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
    private TransformAccessArray transformsAccessArray;
    private EnemyPositionUpdateJob enemyPositionUpdateJob;
    private JobHandle enemyPositionUpdateJobHandle;
    GameObject player;

    bool loading;


    WaitForSeconds waitTimeBetweenSpawns; // 等待生成间隔时间
    WaitForSeconds waitTimeBetweenWaves; // 等待下一波
    WaitUntil waitUnitlNoEnemy;
    WaitUntil waitUnitlEnemyPrefabLoad;

    protected override void Awake()
    {
        base.Awake();

        enemyList = new List<GameObject>();

        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
        waitUnitlNoEnemy = new WaitUntil(() => enemyList.Count == 0);
        waitUnitlEnemyPrefabLoad = new WaitUntil(() => enemyPrefabs.Length > 0);

        player = GameObject.FindGameObjectWithTag("Player");
        
        OnResLoadAsset("Enemy", "Enemy");
    }

    IEnumerator Start() 
    {

        while (spawnEnemy)
        {
            yield return waitUnitlEnemyPrefabLoad;

            yield return waitUnitlNoEnemy; // 检测敌人数量=0，产生敌人

            // waveUI.SetActive(true);

            yield return waitTimeBetweenWaves; // 等待下一波

            // waveUI.SetActive(false);

            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
    }

    // void RandomlySpawnJob()
    // {
    //     enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmount);

    //     for (int i = 0; i < TargetPositions.Length; i++)
    //     {
    //         TargetPositions[i] = player.transform.position;
    //     }

    //     for (int i = 0; i < enemyAmount; i++)
    //     {
    //         GameObject go = PoolManager.Release(enemyPrefabs[UnityEngine.Random.Range(-1, enemyPrefabs.Length)]);
    //         SeekerPositions[i] = go.transform.position;
            
    //         enemyList.Add(go);
    //     }

    //     EnemyManagerJob findJob = new EnemyManagerJob
    //     {
    //             TargetPositions = TargetPositions,
    //             SeekerPositions = SeekerPositions,
    //             NearestTargetPositions = NearestTargetPositions,
    //     };

    //     JobHandle findHandle = findJob.Schedule(SeekerPositions.Length, 100);

    //     findHandle.Complete();

    //     // yield return waitTimeBetweenSpawns;

    //     waveNumber++;
    // }

    // IEnumerator RandomlySpawnCoroutine()
    // {        
    //     enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmount);

    //     for (int i = 0; i < enemyAmount; i++)
    //     {
    //         enemyList.Add(PoolManager.Release(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)]));
    //     }

    //     yield return waitTimeBetweenSpawns;

    //     waveNumber++;
    // }

    IEnumerator RandomlySpawnCoroutine()
    {        
        enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmount);

        Transform[] transforms = new Transform[enemyAmount];

        for (int i = 0; i < enemyAmount; i++)
        {
            GameObject go = PoolManager.Release(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)]);
            go.transform.position = Viewport.Instance.RandomEnemyBronPosition(player.transform.position);

            enemyList.Add(go);
            transforms[i] = go.transform;
        }

        transformsAccessArray = new TransformAccessArray(transforms);
        Debug.Log("transforms.len: " + transforms.Length);

        loading = true;
        yield return waitTimeBetweenSpawns;

        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) 
    {
        enemyList.Remove(enemy);
    }

    

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


    }

    void Update() 
    {
        if (!loading) return;

        enemyPositionUpdateJob = new EnemyPositionUpdateJob
        {
            velocity = 1f,
            fixDeltaTime = Time.fixedDeltaTime,
            playerPosition = player.transform.position,
        };

        enemyPositionUpdateJobHandle = enemyPositionUpdateJob.Schedule(transformsAccessArray);
    }

    // 保证当前帧内Job执行完毕
    private void LateUpdate()
    {
        enemyPositionUpdateJobHandle.Complete();
    }

    // OnDestroy中释放NativeArray的内存
    private void OnDestroy()
    {
        // transformsAccessArray.Dispose();
    }

}
