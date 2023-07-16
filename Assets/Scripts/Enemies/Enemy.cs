using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] int scorePoint = 100;
    [SerializeField] int deathEnergyBonus = 3;

    void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(10);
            Die();
        }
    }

    private void OnDisable() {
        Die();
    }

    public override void Die()
    {
        EnemyManager.Instance.RemoveFromList(gameObject);
        base.Die();
    }
}
