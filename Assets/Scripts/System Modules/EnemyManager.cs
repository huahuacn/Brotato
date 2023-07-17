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
    public int EnemyCount => enemyList.Count;
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;
    [SerializeField] bool spawnEnemy = true; // 产生敌人开关
    [SerializeField] GameObject waveUI;
    [SerializeField] float timeBetweenSpawns = 1f; // 敌人生成时间间隔
    [SerializeField] float timeBetweenWaves = 10f; // 等待下一波初始时间
    [SerializeField] float timeBetweenBatches = 3f; // 等待下一批次时间
    [SerializeField] int minEnemyAmount = 4;
    [SerializeField] int maxEnemyAmount = 10;

    public event UnityAction enemyLoad = delegate{};

    int waveNumber = 1;
    int enemyAmount;
    bool waveStart = false;
    public float curTimeBetweenWaves = 10f; // 等待下一波时间

    List<GameObject> enemyList;
    private TransformAccessArray transformsAccessArray;
    private EnemyPositionUpdateJob enemyPositionUpdateJob;
    private JobHandle enemyPositionUpdateJobHandle;
    GameObject player;
    WaitForSeconds waitTimeBetweenSpawns; // 等待生成间隔时间
    WaitForSeconds waitTimeBetweenBatches; // 等待下一批次
    WaitUntil waitUnitlTimeZero; // 等待下一波
    WaitForSeconds waitTimeOnSeconds; // 倒计时
    WaitUntil waitUnitlNoEnemy;
    WaitUntil waitUnitlEnemyPrefabLoad;

    protected override void Awake()
    {
        base.Awake();

        enemyList = new List<GameObject>();
        

        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenBatches = new WaitForSeconds(timeBetweenBatches);
        waitTimeOnSeconds = new WaitForSeconds(1);
        waitUnitlNoEnemy = new WaitUntil(() => enemyList.Count == 0);
        waitUnitlEnemyPrefabLoad = new WaitUntil(() => PoolManager.Instance.EnemyLoaders.Loaded);
        waitUnitlTimeZero = new WaitUntil(() => curTimeBetweenWaves == 0);

        curTimeBetweenWaves = timeBetweenWaves;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    IEnumerator Start() 
    {
        while (spawnEnemy)
        {

            yield return waitUnitlEnemyPrefabLoad;

            // waveUI.SetActive(true);
            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));

            // waveUI.SetActive(false);
            // yield return waitTimeBetweenWaves; // 等待下一波
            yield return StartCoroutine(nameof(TimeCutdown)); // 等待倒计时结束进入下一波

            RemoveAll();

            yield return waitTimeBetweenSpawns;

            waveNumber++;
        }
    }

    IEnumerator RandomlySpawnCoroutine()
    { 
        waveStart = true;

        StopCoroutine(nameof(RandomEnemyBatches));

        StartCoroutine(nameof(RandomEnemyBatches));

        yield return waitTimeBetweenSpawns;


    }

    IEnumerator RandomEnemyBatches()
    {
        while (waveStart)
        {
            enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmount);
            for (int i = 0; i < enemyAmount; i++)
            {
                GameObject go = PoolManager.Release(PoolManager.Instance.EnemyLoaders.RandomPrefabs);
                go.transform.position = Viewport.Instance.RandomEnemyBronPosition(player.transform.position);
                go.transform.localScale = new Vector3(0.5f,0.5f,0.5f);

                enemyList.Add(go);
            }

            Transform[] transforms = new Transform[enemyList.Count];

            for (int i = 0; i < enemyList.Count; i++)
            {
                transforms[i] = enemyList[i].transform;
            }

            transformsAccessArray = new TransformAccessArray(transforms);

            yield return waitTimeBetweenBatches;
        }
    }

     IEnumerator TimeCutdown()
    {
        while (curTimeBetweenWaves > 0)
        {
            yield return waitTimeOnSeconds;

            curTimeBetweenWaves--;
        }
    }

    public void RemoveFromList(GameObject enemy) 
    {
        enemyList.Remove(enemy);
    }

    public void RemoveAll()
    {
        enemyList.Clear();

        CurTimeBetweenWaves();
    }

    public float CurTimeBetweenWaves()
    {
        waveStart = false;

        curTimeBetweenWaves = timeBetweenWaves + 10 * waveNumber;
        return curTimeBetweenWaves;
    }

    void Update() 
    {
        if (enemyList.Count == 0 || !waveStart) return;

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

    public GameObject GetNearestGameObject(){
        if(enemyList ==null || enemyList.Count == 0) return null;

        GameObject targetTemp = enemyList.Count>0? enemyList[0]:null; 
        float dis = Vector3.Distance(player.transform.position, enemyList[0].transform.position);
        float disTemp;

        for(int i=1;i<enemyList.Count;i++){
            disTemp = Vector3.Distance(player.transform.position,enemyList[i].transform.position);

            if(disTemp < dis){
                targetTemp = enemyList[i];
                dis = disTemp;
            }
        }

        return targetTemp;
    }
}
