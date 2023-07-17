using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [Header("==== INPUT ====")]
    [SerializeField] PlayerInput input;

    [Header("---- MOVE ----")]
    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] float decelerationTime = 3f;
    [Header("---- FIRE ----")]
    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleRight;
    [SerializeField] Transform muzzleBottom;
    [SerializeField] Transform muzzleLeft;
    [SerializeField] float fireInterval = 0.12f;

    float paddingX = 0.2f;
    float paddingY = 0.2f;
    Vector2 previousVelocity; 
    Quaternion previousRotation;

    WaitForSeconds waitDecelerationTime;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate(); 
    WaitForSeconds waitForFireInterval;
   WaitUntil waitUntilEnemyComplete;

    Coroutine moveCoroutine;
    JobHandle moveJobHandle;

    new Rigidbody2D rigidbody;
    new Collider2D collider;

    public void Awake() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        rigidbody.gravityScale = 0f;

        waitDecelerationTime = new WaitForSeconds(decelerationTime);
        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitUntilEnemyComplete = new WaitUntil(() => PoolManager.Instance.ProjectileLoaders.Loaded &&
        EnemyManager.Instance.EnemyCount > 0 );
    }

    protected override void OnEnable() 
    {
        base.OnEnable();

        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire+= FireStart;
    }

    void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= FireStart;
    }

    void Start()
    {
        input.EnableGameplayInput();

        StartCoroutine(nameof(FireCoroutine));
    }

    void Move(Vector2 moveInput)
    {
        // 限制player在grounds
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveCoroutine(moveInput.normalized * moveSpeed));

        // StopCoroutine(nameof(DecelerationCoroutine));
        // StartCoroutine(nameof(MoveRangeLimitationCoroutine));
    }

    void StopMove()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveCoroutine(Vector2.zero));

        // StartCoroutine(nameof(DecelerationCoroutine));
    }

    IEnumerator MoveCoroutine(Vector2 moveVelocity)
    {
        float t = 0f;
        Vector3 velocity = Vector3.zero;
        while (t < 1f)
        {
            t += Time.fixedDeltaTime;
            rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, moveVelocity, t);

            yield return waitForFixedUpdate;

        }

    }

    // 限制player在Grounds内
    IEnumerator MoveRangeLimitationCoroutine()
    {
        while(true)
        {
            this.transform.position = Viewport.Instance.PlayerMoveablePosition(this.transform.position, paddingX, paddingY);

            yield return null;
        }
    }

    IEnumerator DecelerationCoroutine()
    {
        yield return waitDecelerationTime;

        StopCoroutine(nameof(MoveRangeLimitationCoroutine));
    }

    IEnumerator FireCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return waitUntilEnemyComplete;

            // Instantiate(projectile, muzzle.position, Quaternion.identity);
            PoolManager.Release(PoolManager.Instance.ProjectileLoaders.RandomPrefabs, muzzleTop.position);

            yield return waitForFireInterval;

        }
    }

    void FireStart()
    {
        PoolManager.Release(PoolManager.Instance.ProjectileLoaders.RandomPrefabs, muzzleTop.position);
    }

}
