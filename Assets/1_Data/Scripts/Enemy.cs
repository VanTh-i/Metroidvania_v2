using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float health;
    [SerializeField] private float recoilLength;
    //[SerializeField] private float recoilFactor;
    float recoilTimer;
    private bool isRecoiling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = true;
                recoilTimer = 0;
            }
        }
    }

    public void EnemyTakeDamage(float _damage, float _hitForce, Vector2 _hitDirection)
    {
        health -= _damage;
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * _hitDirection);
        }
    }
}
