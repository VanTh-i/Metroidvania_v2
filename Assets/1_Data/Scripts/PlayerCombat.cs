using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Player
{
    [Header("Attack")]
    [SerializeField] private float damage;
    [SerializeField] private float timeBetweenAttack;
    private float timeSinceAttack;
    [SerializeField] private Transform sideAttackTransform, downAttackTransform, upAttackTransform;
    [SerializeField] private float sideAttackArea, downAttackArea, upAttackArea;
    [SerializeField] private LayerMask attackable;


    protected override void Update()
    {
        base.Update();
        Attack();
    }
    private void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attacking && timeSinceAttack > timeBetweenAttack)
        {
            timeSinceAttack = 0;
            Animation.AttackAnimation();

            if (yAxis == 0 || yAxis < 0 && Grounded)
            {
                Hit(sideAttackTransform, sideAttackArea);
            }
            else if (yAxis > 0)
            {
                Hit(upAttackTransform, upAttackArea);
            }
            else if (yAxis < 0 && !Grounded)
            {
                Hit(downAttackTransform, downAttackArea);

            }
        }
    }

    private void Hit(Transform _attackTransform, float _attackArea)
    {
        Collider2D[] objectToHit = Physics2D.OverlapCircleAll(_attackTransform.position, _attackArea, attackable);

        for (int i = 0; i < objectToHit.Length; i++)
        {
            if (objectToHit[i].GetComponent<Enemy>() != null)
            {
                Debug.Log("Hit " + objectToHit[i].GetComponent<Enemy>().name);
                objectToHit[i].GetComponent<Enemy>().EnemyTakeDamage(damage);
            }
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireSphere(downAttackTransform.position, downAttackArea);
        Gizmos.DrawWireSphere(upAttackTransform.position, upAttackArea);

    }
}
