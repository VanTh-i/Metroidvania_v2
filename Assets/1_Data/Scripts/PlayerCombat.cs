using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Player
{
    [Header("Attack")]
    [SerializeField] private float damage;
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private float hitForce;
    [SerializeField] private GameObject swordSlashPrefab;
    private float timeSinceAttack;
    [SerializeField] private Transform sideAttackTransform, downAttackTransform, upAttackTransform;
    [SerializeField] private float sideAttackArea, downAttackArea, upAttackArea;
    [SerializeField] private LayerMask attackable;

    protected override void Start()
    {
        base.Start();
    }

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

            if (yAxis == 0 || yAxis < 0 && playerState.IsInGround)
            {
                Hit(sideAttackTransform, sideAttackArea);
                Instantiate(swordSlashPrefab, sideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(upAttackTransform, upAttackArea);
                SlashEffect(swordSlashPrefab, 90, upAttackTransform);
            }
            else if (yAxis < 0 && !playerState.IsInGround)
            {
                Hit(downAttackTransform, downAttackArea);
                SlashEffect(swordSlashPrefab, -90, downAttackTransform);

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
                objectToHit[i].GetComponent<Enemy>().EnemyTakeDamage(damage, hitForce, new Vector2(transform.position.x > objectToHit[i].transform.position.x ? 1 : -1 ,
                                                                                                   transform.position.y > objectToHit[i].transform.position.y ? 1 : -1));
            }
        }


    }

    private void SlashEffect(GameObject _slashEffect, int _effectAngle, Transform _acttackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _acttackTransform);
        if (playerState.IsLookingRight)
        {
            _slashEffect.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, _effectAngle);
            
        }
        else
        {
            _slashEffect.transform.eulerAngles = new Vector3(0, -transform.eulerAngles.y, _effectAngle);

        }

        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireSphere(downAttackTransform.position, downAttackArea);
        Gizmos.DrawWireSphere(upAttackTransform.position, upAttackArea);

    }
}
