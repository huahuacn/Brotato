using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("----- MOVE -----")]
    [SerializeField] float moveSpeed = 15f;

    // [Header("----- FIRE -----")]
    // [SerializeField] GameObject[] projectiles;
    // [SerializeField] Transform muzzle;
    // [SerializeField] float minFireInterval;
    // [SerializeField] float maxFireInterval;

    // [SerializeField] AudioData projectileLaunchSFX;

    float paddingX = 0.1f;
    float paddingY = 0.1f;
    Vector3 targetPosition;
    GameObject player;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    protected virtual void Awake() 
    {
        // var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        // paddingX = size.x / 2f;
        // paddingY = size.y / 2f;

        player = GameObject.FindGameObjectWithTag("Player");
        targetPosition = player.transform.position;
        
        // EnemyManager.Instance.enemyLoad += EnemyLoad;
    }

    void OnEnable() 
    {
        // StartCoroutine(nameof(RandomlyFireCoroutine));
        EnemyLoad();
    }

    void EnemyLoad()
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
    }

    void OnDisable() 
    {
        EnemyManager.Instance.enemyLoad -= EnemyLoad;
        StopAllCoroutines();
    }

    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);
        targetPosition = player.transform.position;

        // targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            targetPosition = player.transform.position;
            if (Vector3.Distance(transform.position, targetPosition) >= moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Player");
                targetPosition = player.transform.position;
            }

            yield return waitForFixedUpdate;
        }
    }

    // IEnumerator RandomlyFireCoroutine()
    // {
    //     while (gameObject.activeSelf)
    //     {
    //         yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

    //         if (GameManager.GameOver()) yield break;

    //         foreach (var projectile in projectiles)
    //         {
    //             // PoolManager.Release(projectile, muzzle.position);
    //             AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
    //         }
    //     }
    // }
}
