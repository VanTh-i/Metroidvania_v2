using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Player player;

    [Header("Attack")]
    [SerializeField] private float timeBetweenAttack;
    private float timeSinceAttack;
    private bool attacking;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        GetInput();
        Attack();
    }

    private void GetInput()
    {
        attacking = Input.GetKeyDown(KeyCode.Mouse0);
    }

    private void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attacking && timeSinceAttack > timeBetweenAttack)
        {
            timeSinceAttack = 0;
            player.Animation.AttackAnimation();
        }
    }
}
