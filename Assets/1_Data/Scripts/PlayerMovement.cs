using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Player
{
    private Rigidbody2D rb;

    [SerializeField] private float speed;

    [Header("Jump Mechanic")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferTime;
    private float jumpBufferCounter;
    //private bool doubleJump;
    private int airJumpCounter = 0;
    private int maxAirJump = 1;


    [Header("Player Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;
    private bool dashed;
    private bool isDashing;


    [Header("Raycast check ground")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY;
    [SerializeField] private LayerMask whatIsGround;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        base.Update();
        CheckPlayerIsInGround();

        if (isDashing) return;
        Move();
        Jump();
        StartDash();
    }

    private void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }

    private void Move()
    {
        Flip();

        rb.velocity = new Vector2(speed * xAxis, rb.velocity.y);
        //chay animation run khi o tren mat dat va player di chuyen
        Animation.RunAnimation(IsGrounded() && rb.velocity.x != 0);
    }

    public bool IsGrounded()
    {
        //kiem tra xem player co dang dung tren mat dat hay la khong
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else return false;
    }

    public void CheckPlayerIsInGround()
    {
        if (IsGrounded())
        {
            Grounded = true;
        }
        else Grounded = false;
    }

    private void Jump()
    {
        /*CoyoteTimeCounter se bi tru lien tuc khi khong o tren mat dat, khi roi khoi mat
        dat 0.2s, player van co the nhay. Tim hieu them ve coyoteTime de biet chi tiet.*/
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        //
        //jump
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteTimeCounter = 0f;
        }
        //double jump
        else if (!IsGrounded() && airJumpCounter < maxAirJump && Input.GetKeyDown(KeyCode.Space))
        {
            //doubleJump = true;
            airJumpCounter++;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }

        //khi nha phim space player se roi xuong, tao cam giac jump tot hon
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.1f);
            coyoteTimeCounter = 0f;
        }

        Animation.JumpAnimation(!IsGrounded() && rb.velocity.y > 0);
        Animation.FallAnimation(!IsGrounded() && rb.velocity.y < 0);
    }

    private void StartDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (IsGrounded())
        {
            dashed = false;
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        Animation.DashAnimation();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);

        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = 1;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
