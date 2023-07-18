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
        GameObject enemy = EnemyManager.Instance.GetNearestGameObject();
        if (enemy == null)
        {
            gameObject.SetActive(false);
            return;
        }

        SetTarget(enemy);

        StartCoroutine(nameof(MoveDirectionCoroutine));
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
    public IEnumerator MoveDirectionCoroutine()
    {
        yield return null;

        if (target.activeSelf) 
        {
            moveDirection = (target.transform.position - transform.position).normalized;
            moveDirection.z = 0;

            transform.rotation = Quaternion.FromToRotation(Vector3.right, moveDirection);

        } 
   
    }
}

