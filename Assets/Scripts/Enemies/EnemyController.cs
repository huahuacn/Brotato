using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("----- MOVE -----")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float moveRotationAngle = 25f;

    [Header("----- FIRE -----")]
    [SerializeField] GameObject[] projectiles;
    [SerializeField] Transform muzzle;
    [SerializeField] float minFireInterval;
    [SerializeField] float maxFireInterval;

    [SerializeField] AudioData projectileLaunchSFX;

    float paddingX;
    float paddingY;
    Vector3 targetPosition;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    protected virtual void Awake() 
    {
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }

    void OnEnable() 
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }

    void OnDisable() 
    {
        StopAllCoroutines();
    }

    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);

        targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf)
        {
            // if has not arrived targetPosition
            // keep moving to targetPosition
            // make enemy rotate with x axis while moving
            // else
            // set a new targetPosition

            if (Vector3.Distance(transform.position, targetPosition) >= moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle, Vector3.right);
            }
            else
            {
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }

            yield return waitForFixedUpdate;
        }
    }

    IEnumerator RandomlyFireCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            if (GameManager.GameOver()) yield break;

            foreach (var projectile in projectiles)
            {
                // PoolManager.Release(projectile, muzzle.position);
                AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            }
        }
    }
}
