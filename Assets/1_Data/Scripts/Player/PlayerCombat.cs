using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Player
{
    [Header("Attack")]
    [SerializeField] private float damage;
    [SerializeField] private float timeBetweenAttack;
    private float timeSinceAttack;

    [SerializeField] private GameObject swordSlashPrefab;
    [SerializeField] private Transform sideAttackTransform, downAttackTransform, upAttackTransform;
    [SerializeField] private float sideAttackArea, downAttackArea, upAttackArea;
    [SerializeField] private LayerMask attackable;

    [Header("Recoil")]
    [SerializeField] private float hitForce; //to enemy

    [SerializeField] private int recoilXSteps;
    [SerializeField] private int recoilYSteps;
    [SerializeField] private float recoilXSpeed;
    [SerializeField] private float recoilYSpeed;
    private int stepXRecoiled, stepYRecoiled;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        Recoil();
        if (playerState.IsWallSliding) return;
        Attack();
    }
    private void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attacking && timeSinceAttack > timeBetweenAttack)
        {
            timeSinceAttack = 0;
            //Animation.AttackAnimation();

            if (yAxis == 0 || yAxis < 0 && playerState.IsInGround)
            {
                Hit(sideAttackTransform, sideAttackArea, ref playerState.recoilingX);
                Instantiate(swordSlashPrefab, sideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(upAttackTransform, upAttackArea, ref playerState.recoilingY);
                SlashEffect(swordSlashPrefab, 90, upAttackTransform);
            }
            else if (yAxis < 0 && !playerState.IsInGround)
            {
                Hit(downAttackTransform, downAttackArea, ref playerState.recoilingY);
                SlashEffect(swordSlashPrefab, -90, downAttackTransform);

            }
        }
    }

    private void Hit(Transform _attackTransform, float _attackArea, ref bool _recoilDir)
    {
        Collider2D[] objectToHit = Physics2D.OverlapCircleAll(_attackTransform.position, _attackArea, attackable);

        if (objectToHit.Length > 0)
        {
            _recoilDir = true;
        }

        for (int i = 0; i < objectToHit.Length; i++)
        {
            if (objectToHit[i].GetComponent<Enemy>() != null)
            {
                Debug.Log("Hit " + objectToHit[i].GetComponent<Enemy>().name);
                objectToHit[i].GetComponent<Enemy>().EnemyTakeDamage(damage, hitForce, new Vector2(transform.position.x > objectToHit[i].transform.position.x ? 1 : -1,
                                                                                                   transform.position.y > objectToHit[i].transform.position.y ? 1 : -1));
            }
        }


    }

    private void SlashEffect(GameObject _slashEffect, int _effectAngle, Transform _acttackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _acttackTransform);
        if (playerState.IsFacingRight)
        {
            _slashEffect.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, _effectAngle);

        }
        else
        {
            _slashEffect.transform.eulerAngles = new Vector3(0, -transform.eulerAngles.y, _effectAngle);

        }

        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);


    }

    private void Recoil()
    {
        if (playerState.recoilingX)
        {
            if (playerState.IsFacingRight)
            {
                //rb.velocity = new Vector2(-recoilXSpeed, 0);
                //rb.AddForce(-recoilXSpeed * Vector2.right, ForceMode2D.Impulse);
                transform.Translate(-recoilXSpeed * Vector2.right * Time.deltaTime);
            }
            else
            {
                //rb.velocity = new Vector2(recoilXSpeed, 0);
                //rb.AddForce(recoilXSpeed * Vector2.right, ForceMode2D.Impulse);
                transform.Translate(recoilXSpeed * Vector2.right * Time.deltaTime);

            }
        }

        if (playerState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                //rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
                transform.Translate(recoilYSpeed * Vector2.up * Time.deltaTime);

            }
            else
            {
                //rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
                transform.Translate(-recoilYSpeed * Vector2.up * Time.deltaTime);

            }

            playerState.AirJumpCounter = 0; // reset double jump when Recoil in air
        }
        else
        {
            rb.gravityScale = playerState.GravityScale;
            //rb.gravityScale = 1;
        }

        //stop recoil
        if (playerState.recoilingX && stepXRecoiled < recoilXSteps)
        {
            stepXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (playerState.recoilingY && stepYRecoiled < recoilYSteps)
        {
            stepYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (playerState.IsInGround)
        {
            StopRecoilY();
        }
    }

    private void StopRecoilX()
    {
        stepXRecoiled = 0;
        playerState.recoilingX = false;
    }

    private void StopRecoilY()
    {
        stepYRecoiled = 0;
        playerState.recoilingY = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireSphere(downAttackTransform.position, downAttackArea);
        Gizmos.DrawWireSphere(upAttackTransform.position, upAttackArea);

    }
}
