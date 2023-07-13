using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    TrailRenderer trail;
    protected virtual void Awake() 
    {
        trail = GetComponentInChildren<TrailRenderer>();

        if (moveDirection != Vector2.right) transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirection);
    }

    protected override void OnEnable()
    {
        SetTarget(EnemyManager.Instance.RandomEnemy);
        transform.rotation = Quaternion.identity;

        if (target == null) base.OnEnable();
        else StartCoroutine(HomingCoroutine(target));
    }

    void OnDisable() 
    {
        trail.Clear();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
    public IEnumerator  HomingCoroutine(GameObject target)
    {
        while(gameObject.activeSelf)
        {
            if (target.activeSelf) 
                moveDirection = (target.transform.position - transform.position).normalized;
            yield return null;
        }
    }
}

