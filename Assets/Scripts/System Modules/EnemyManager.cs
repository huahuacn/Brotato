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
    public AddressableLoader addressableLoader;
    public GameObject RandomEnemy => enemyList.Count == 0 ? null : enemyList[UnityEngine.Random.Range(0, enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;
    [SerializeField] bool spawnEnemy = true; // 产生敌人开关
    [SerializeField] GameObject waveUI;
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
        addressableLoader = new AddressableLoader();

        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
        waitUnitlNoEnemy = new WaitUntil(() => enemyList.Count == 0);
        waitUnitlEnemyPrefabLoad = new WaitUntil(() => addressableLoader.Loaded);

        player = GameObject.FindGameObjectWithTag("Player");

        addressableLoader.OnResLoadAsset("Enemy","Enemy");
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

    IEnumerator RandomlySpawnCoroutine()
    {        
        enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmount);

        Transform[] transforms = new Transform[enemyAmount];

        for (int i = 0; i < enemyAmount; i++)
        {
            GameObject go = PoolManager.Release(addressableLoader.RandomPrefabs);
            go.transform.position = Viewport.Instance.RandomEnemyBronPosition(player.transform.position);

            enemyList.Add(go);
            transforms[i] = go.transform;
        }

        transformsAccessArray = new TransformAccessArray(transforms);

        loading = true;
        yield return waitTimeBetweenSpawns;

        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) 
    {
        enemyList.Remove(enemy);
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

    GameObject GetNearestGameObject(){
        if(enemyList !=null && enemyList.Count > 0)
        {
            GameObject targetTemp = enemyList.Count>0? enemyList[0]:null; 
            float dis = Vector3.Distance(player.transform.position,enemyList[0].transform.position);
            float disTemp;

            for(int i=1;i<enemyList.Count;i++){
                disTemp = Vector3.Distance(player.transform.position,enemyList[i].transform.position);

                if(disTemp < dis){
                    targetTemp = enemyList[i];
                    dis = disTemp;
                }
            }

            return targetTemp;
        }else{

            return null;
        }
    }
}
