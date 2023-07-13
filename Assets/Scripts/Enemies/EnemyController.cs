using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

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

    GameObject player;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    protected virtual void Awake() 
    {
        // var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        // paddingX = size.x / 2f;
        // paddingY = size.y / 2f;

        player = GameObject.FindGameObjectWithTag("Player");

        // EnemyManager.Instance.enemyLoad += EnemyLoad;
    }

    void OnEnable() 
    {
        // StartCoroutine(nameof(RandomlyFireCoroutine));
        // StartCoroutine(nameof(RandomlyMovingCoroutine));

    }

    void OnDisable() 
    {
        StopAllCoroutines();
    }

    // IEnumerator RandomlyMovingCoroutine()
    // {
    //     var targetPosition = player.transform.position;
    //     transform.position = Viewport.Instance.RandomEnemyBronPosition(targetPosition);

    //     while (gameObject.activeSelf)
    //     {
    //         targetPosition = player.transform.position;


    //         if (Vector3.Distance(transform.position, targetPosition) >= moveSpeed * Time.fixedDeltaTime)
    //         {
    //             transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
    //         }
    //         yield return waitForFixedUpdate;
    //     }
    // }

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
