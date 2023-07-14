using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    TrailRenderer trail;
    protected virtual void Awake() 
    {
        // trail = GetComponentInChildren<TrailRenderer>();

        // if (moveDirection != Vector2.right) transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirection);
    }

    protected override void OnEnable()
    {
        SetTarget(EnemyManager.Instance.GetNearestGameObject());

        StartCoroutine(MoveDirectionCoroutine(target));
        base.OnEnable();
    }

    void OnDisable() 
    {
        // trail.Clear();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
    public IEnumerator  MoveDirectionCoroutine(GameObject target)
    {
        yield return null;

        if (target.activeSelf) moveDirection = (target.transform.position - transform.position).normalized;
   
    }
}

