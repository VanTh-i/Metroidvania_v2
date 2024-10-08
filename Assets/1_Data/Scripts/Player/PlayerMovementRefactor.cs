using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRefactor : Player
{
    public PlayerData Data;

    #region STATE VARIABLES
    //Variables control the various actions the player can perform

    //public bool IsFacingRight //call in player state
    private bool isJumping;
    private bool isWallJumping;
    private bool isDashing;
    private bool isSliding;

    //Timers
    private float lastOnGroundTime;
    private float lastOnWallTime;
    private float lastOnWallRightTime;
    private float lastOnWallLeftTime;

    //Jump
    private bool isJumpCut;
    private bool isJumpFalling;

    //Wall Jump
    private float wallJumpStartTime;
    private int lastWallJumpDir;

    //Dash
    private int dashesLeft;
    private bool dashRefilling;
    private Vector2 lastDashDir;
    private bool isDashAttacking;
    #endregion

    #region INPUT PARAMETERS
    private float lastPressedJumpTime;
    private float lastPressedDashTime;
    #endregion

    #region LAYER CHECK PARAMETERS
    [Header("Layer Checks Point")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);

    [Space(5)]
    [SerializeField] private Transform frontWallCheckPoint;
    [SerializeField] private Transform backWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask groundLayer;
    #endregion

    protected override void Start()
    {
        base.Start();
        SetGravityScale(Data.gravityScale);
        playerState.IsFacingRight = true;
    }

    protected override void Update()
    {
        base.Update();

        #region TIMERS
        lastOnGroundTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastOnWallRightTime -= Time.deltaTime;
        lastOnWallLeftTime -= Time.deltaTime;

        lastPressedJumpTime -= Time.deltaTime;
        lastPressedDashTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        if (xAxis != 0)
            CheckDirectionToFace(xAxis > 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUpInput();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            OnDashInput();
        }
        #endregion

        #region COLLISION CHECKS
        if (!isDashing && !isJumping)
        {
            //Ground Check
            if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0 , groundLayer) && !isJumping)//checks if set box overlaps with ground
            {
                lastOnGroundTime = Data.coyoteTime;
                playerState.IsInGround = true;
            }

            //Right Wall Check
            if ((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && playerState.IsFacingRight)
                || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !playerState.IsFacingRight) && !isWallJumping)
            {
                lastOnWallRightTime = Data.coyoteTime;
            }

            //Left Wall Check
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !playerState.IsFacingRight)
                || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) && playerState.IsFacingRight)) && !isWallJumping)
            {
                lastOnWallLeftTime = Data.coyoteTime;
            }

            lastOnWallTime = MathF.Max(lastOnWallLeftTime, lastOnWallRightTime);
        }
        #endregion

        #region JUMP CHECKS
        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;

            if (!isWallJumping)
                isJumpFalling = true;
        }

        if (isWallJumping && Time.time - wallJumpStartTime > Data.wallJumpTime)
        {
            isWallJumping = false;
        }

        if (lastOnGroundTime > 0 && !isJumping && !isWallJumping)
        {
            isJumpCut = false;

            if (!isJumping)
                isJumpFalling = false;
        }

        if (!isDashing)
        {
            //Jump
            if (CanJump() && lastPressedJumpTime > 0)
            {
                isJumping = true;
                isWallJumping = false;
                isJumpCut = false;
                isJumpFalling = false;
                Jump();
            }
            //WALL JUMP
            else if (CanWallJump() && lastPressedJumpTime > 0)
            {
                isWallJumping = true;
                isJumping = false;
                isJumpCut = false;
                isJumpFalling = false;

                wallJumpStartTime = Time.time;
                lastWallJumpDir = (lastOnWallRightTime > 0) ? -1 : 1;

                WallJump(lastWallJumpDir);
            }
        }
        #endregion

        #region DASH CHECKS
        if (CanDash() && lastPressedDashTime > 0)
        {
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            GameSleep(Data.dashSleepTime);

            //If not direction pressed, dash forward
            if (new Vector2(xAxis, yAxis) != Vector2.zero)
                lastDashDir = new Vector2(xAxis, yAxis);
            else
                lastDashDir = playerState.IsFacingRight ? Vector2.right : Vector2.left;



            isDashing = true;
            isJumping = false;
            isWallJumping = false;
            isJumpCut = false;

            StartCoroutine(nameof(StartDash), lastDashDir);
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((lastOnWallLeftTime > 0 && xAxis < 0) || (lastOnWallRightTime > 0 && xAxis > 0)))
            isSliding = true;
        else
            isSliding = false;
        #endregion

        #region GRAVITY
        if (!isDashAttacking) //dasing = failed
        {
            if (isSliding)
            {
                SetGravityScale(0);
            }
            else if (rb.velocity.y < 0 && yAxis < 0)
            {
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                rb.velocity = new Vector2(rb.velocity.x, MathF.Max(rb.velocity.y, -Data.maxFastFallSpeed));
            }
            else if (isJumpCut)
            {
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                rb.velocity = new Vector2(rb.velocity.x, MathF.Max(rb.velocity.y, -Data.maxFallSpeed));
            }
            else if ((isJumping || isWallJumping || isJumpFalling) && MathF.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold )
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (rb.velocity.y < 0)
            {
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));

            }
            else
            {
				SetGravityScale(Data.gravityScale); //Default gravity if standing on a platform
            }
        }
        else
        {
			SetGravityScale(0); //No gravity when dashing
        }
        #endregion
    }

    private void FixedUpdate()
    {
        //Handle Run
        if (!isDashing)
        {
            if (isWallJumping)
                Run(Data.wallJumpRunLerp);
            else
                Run(1);
        }
        else if (isDashAttacking)
        {
            Run(Data.dashEndRunLerp);
        }

        //Handle Slide
        if (isSliding)
            Slide();
    }

    #region INPUT CALLBACKS
    //Methods which whandle input detected in Update()
    public void OnJumpInput()
    {
        lastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            isJumpCut = true;
    }

    public void OnDashInput()
    {
        lastPressedDashTime = Data.dashInputBufferTime;
    }
    #endregion

    #region GENERAL METHODS
    private void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    private void GameSleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
    #endregion

    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        float targetSpeed = xAxis * Data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

        float accelRate;
        if (lastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        }

        if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        if (Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        float speedDif = targetSpeed -rb.velocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        playerState.IsFacingRight = !playerState.IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        float force = Data.jumpForce;
        if (rb.velocity.y < 0)
        {
            force -= rb.velocity.y;
        }

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void WallJump(int dir)
    {
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;
        lastOnWallRightTime = 0;
        lastOnWallLeftTime = 0;

        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(rb.velocity.x) != MathF.Sign(force.x))
        {
            force.x -= rb.velocity.x;
        }
        if (rb.velocity.y < 0)
        {
            force.y -= rb.velocity.y;
        }

        rb.AddForce(force, ForceMode2D.Impulse);
    }
    #endregion

    #region DASH METHODS
    private IEnumerator StartDash(Vector2 dir)
    {
        lastOnGroundTime = 0;
        lastPressedDashTime = 0;
        float startTime = Time.time;
        dashesLeft--;
        isDashAttacking = true;

        SetGravityScale(0);

        while (Time.time - startTime <= Data.dashAttackTime)
        {
            rb.velocity = dir.normalized * Data.dashSpeed;
            yield return null;
        }

        startTime = Time.time;

        isDashAttacking = false;

        SetGravityScale(Data.gravityScale);
        rb.velocity = Data.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
        {
			yield return null;
        }

        isDashing = false;

    }

    private IEnumerator RefillDash(int amount)
    {
        dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        dashRefilling = false;
        dashesLeft = Mathf.Min(Data.dashAmount, dashesLeft + 1);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        float speedDif = Data.slideSpeed - rb.velocity.y;
        float movement = speedDif * Data.slideAccel;
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }
    #endregion

    #region CHECK METHODS
    private void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != playerState.IsFacingRight)
        {
            Turn();
        }
    }

    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;

    }

    private bool CanWallJump()
    {
        return lastPressedJumpTime > 0 && lastOnWallTime > 0 && lastOnGroundTime <= 0 && (!isWallJumping ||
             (lastOnWallRightTime > 0 && lastWallJumpDir == 1) || (lastOnWallLeftTime > 0 && lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return isJumping && rb.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return isWallJumping && rb.velocity.y > 0;
    }

    private bool CanDash()
    {
        if (!isDashing && dashesLeft < Data.dashAmount && lastOnGroundTime > 0 && !dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return dashesLeft > 0;
    }

    public bool CanSlide()
    {
        if (lastOnWallTime > 0 && !isJumping && !isWallJumping && !isDashing && lastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(frontWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(backWallCheckPoint.position, wallCheckSize);
    }
    #endregion
}
