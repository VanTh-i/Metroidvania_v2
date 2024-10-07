using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected PlayerAnimation Animation;
    protected PlayerState playerState;

    protected bool attacking;
    protected float xAxis, yAxis;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animation = GetComponentInChildren<PlayerAnimation>();
        playerState = GetComponent<PlayerState>();
    }
    protected virtual void Update()
    {
        GetInput();
    }

    protected virtual void GetInput()
    {
        attacking = Input.GetKeyDown(KeyCode.Mouse0);
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
    }
}
