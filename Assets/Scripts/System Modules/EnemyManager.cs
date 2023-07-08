using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject RandomEnemy => enemyList.Count == 0 ? null : enemyList[Random.Range(0, enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;
    [SerializeField] bool spawnEnemy = true; // 产生敌人开关
    [SerializeField] GameObject waveUI;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] float timeBetweenSpawns = 1f; // 敌人生成时间间隔
    [SerializeField] float timeBetweenWaves = 1f; // 等待下一波时间
    [SerializeField] int minEnemyAmount = 4;
    [SerializeField] int maxEnemyAmount = 10;

    int waveNumber = 1;
    int enemyAmount;

    List<GameObject> enemyList;

    WaitForSeconds waitTimeBetweenSpawns; // 等待生成间隔时间
    WaitForSeconds waitTimeBetweenWaves; // 等待下一波
    WaitUntil waitUnitlNoEnemy;

    protected override void Awake()
    {
        base.Awake();

        enemyList = new List<GameObject>();

        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
        waitUnitlNoEnemy = new WaitUntil(() => enemyList.Count == 0);
    }

    IEnumerator Start() 
    {
        while (spawnEnemy)
        {
            yield return waitUnitlNoEnemy; // 检测敌人数量=0，产生敌人

            waveUI.SetActive(true);

            yield return waitTimeBetweenWaves; // 等待下一波

            waveUI.SetActive(false);

            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
    }

    IEnumerator RandomlySpawnCoroutine()
    {
        enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmount);

        for (int i = 0; i < enemyAmount; i++)
        {
            // enemyList.Add(PoolManager.Release(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
        }

        yield return waitTimeBetweenSpawns;

        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);
}
