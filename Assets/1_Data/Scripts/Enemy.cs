using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health;

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void EnemyTakeDamage(float _damage)
    {
        health -= _damage;
    }
}
