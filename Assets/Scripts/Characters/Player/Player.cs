using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [Header("==== INPUT ====")]
    [SerializeField] PlayerInput input;

    [Header("---- MOVE ----")]
    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] float decelerationTime = 3f;

    float paddingX = 0.2f;
    float paddingY = 0.2f;
    Vector2 previousVelocity; 
    Quaternion previousRotation;

    WaitForSeconds waitDecelerationTime;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate(); 
    Coroutine moveCoroutine;

    new Rigidbody2D rigidbody;
    new Collider2D collider;

    public void Awake() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        // var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        // paddingX = size.x / 2f;
        // paddingY = size.y / 2f;

        rigidbody.gravityScale = 0f;

        waitDecelerationTime = new WaitForSeconds(decelerationTime);
    }

    protected override void OnEnable() 
    {
        base.OnEnable();

        input.onMove += Move;
        input.onStopMove += StopMove;
    }

    void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
    }

    void Start()
    {
        input.EnableGameplayInput();

    }


    void Move(Vector2 moveInput)
    {
      
        // 限制player在grounds
        rigidbody.velocity = moveInput.normalized * moveSpeed;
        // Vector2 tmp = moveInput.normalized * moveSpeed;
        // transform.position = transform.position + new Vector3(tmp.x, tmp.y, 0)*Time.deltaTime;


        // rigidbody.velocity = Viewport.Instance.FollowPosition(transform.position, moveInput.normalized * moveSpeed, min, max, 0);

        // camera 跟随 player &&  限制camera在grounds
    }

    void StopMove()
    {
        rigidbody.velocity = Vector2.zero;
    }


    // 限制player在view point内
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

}
