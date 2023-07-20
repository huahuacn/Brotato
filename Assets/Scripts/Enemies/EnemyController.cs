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
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float pubchingMoveSpeed = 5f;
    private Vector3 moveDirection;
    public float nextMove;
    private Vector3 startPubchingPosition;
    private Vector3 pubchingPosition;
    private FSMSystem fms;

    // [Header("----- FIRE -----")]
    // [SerializeField] GameObject[] projectiles;
    // [SerializeField] Transform muzzle;
    // [SerializeField] float minFireInterval;
    // [SerializeField] float maxFireInterval;

    // [SerializeField] AudioData projectileLaunchSFX;

    static GameObject player;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    protected virtual void Awake() 
    {
        // var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        // paddingX = size.x / 2f;
        // paddingY = size.y / 2f;

        player ??= GameObject.FindGameObjectWithTag("Player");
        fms = new FSMSystem();
        fms.SetHolder(this);
        fms.AddState(new MovingToPlayerState());
        fms.AddState(new PubchingToPlayerState());

        // EnemyManager.Instance.enemyLoad += EnemyLoad;
    }

    void OnEnable() 
    {
        // StartCoroutine(nameof(RandomlyFireCoroutine));
        // StartCoroutine(nameof(MovingToPlayerCoroutine));
        StartCoroutine(nameof(FMSCoroutine));
    }

    void OnDisable() 
    {
        StopAllCoroutines();
    }

    IEnumerator FMSCoroutine()
    {
        fms.SwitchState<MovingToPlayerState>();
        while (gameObject.activeSelf)
        {
            fms.CurState.OnStay(fms);
            yield return waitForFixedUpdate;
        }
    }

    IEnumerator MovingToPlayerCoroutine()
    {
        while (gameObject.activeSelf)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance >= moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.fixedDeltaTime);
            }

            yield return waitForFixedUpdate;
        }
    }

    public float CheckPlayerDistance()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    public float NextMove()
    {
        nextMove = moveSpeed * Time.fixedDeltaTime;
        return nextMove;
    }

    public void MovingToPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.fixedDeltaTime);
    }

    public void CheckMoveDirection()
    {
        moveDirection = (player.transform.position - transform.position).normalized;

        float distance =  CheckPlayerDistance() + 2;

        startPubchingPosition = transform.position;
        pubchingPosition = transform.position + moveDirection * distance;
    }

    public bool CheckPubchingPosition()
    {
        return transform.position == pubchingPosition;
    }

    public void PubchingToPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, pubchingPosition, pubchingMoveSpeed* Time.fixedDeltaTime);

    }

}
