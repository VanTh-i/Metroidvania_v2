using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerAnimation Animation;

    [SerializeField] private float speed;
    private float xAxis;
    [SerializeField] private float jumpForce;

    [Header("Raycast check ground")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY;
    [SerializeField] private LayerMask whatIsGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animation = GetComponentInChildren<PlayerAnimation>();
    }

    private void Update()
    {
        GetInput();
        Move();
        Jump();
    }


    private void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
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

    private void Jump()
    {
        //khi nha phim space player se roi xuong, tao cam giac jump tot hon
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        //jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        Animation.JumpAnimation(!IsGrounded() && rb.velocity.y > 0);
        Animation.FallAnimation(!IsGrounded() && rb.velocity.y < 0);
    }
}
