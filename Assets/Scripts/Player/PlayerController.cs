using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -1;
    private float knockbackStartTime;
    private float beginUpSpeedTime;
    private float knockbackDuration = 0.2f;
    private float groundCheckRadius = 0.3f;
    private float wallCheckDistance = 0.65f;
    private float wallSlideSpeed = 1;
    //private float movementForceInAir = 50;
    private float variableJumpHightMultiplier = 0.5f;
    private float airDragMultiplier = 0.95f;
    private float wallHopForce = 10f;
    private float wallJumpForce = 20f;
    private float turnTimerSet = 0.1f;
    private float jumpTimerSet = 0.15f;
    private float movementSpeed = 10f;
    private float jumpForce = 20.0f;
    private float wallJumpTimerSet = 0.5f;
    private float dashTime = 0.1f;
    private float dashSpeed = 50f;
    private float distanceBetweenImages = 0.1f;
    private float dashCoolDown = 2.5f;
    private float upSpeedDuration = 3;
    private float ledgeClimbXOffset1 = 0.3f;
    private float ledgeClimbYOffset1 = 0f;
    private float ledgeClimbXOffset2 = 0.5f;
    private float ledgeClimbYOffset2 = 2f;

    private int amountOfJumpsLeft;
    private int facingDirection = 1; // right
    private int lastWallJumpDirection;

    private bool isFacingRight = true;
    private bool timeIsUp = true;
    private bool isWalking;
    private bool isDashing;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingLedge;
    private bool isWallSliding;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool canClimbLedge;
    private bool hasWallJumped;
    private bool ledgeDetected;
    private bool knockback;
    
    [SerializeField] private Vector2 knockbackSpeed;
    private Animator anim;
    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    private Rigidbody2D rb;

    private int amountOfJumps = 1;

    public bool upSpeed;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public LayerMask whatIsMask;

    [SerializeField] private AudioSource jumpSoundEffect;

    private void Start()
    {
        movementSpeed = 10f;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }


    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
        UpdateAnimations();
        CheckDash();
        CheckLedgeClimb();
        CheckKnockback();
        UpSpeed();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }
    public void UpSpeed()
    {
        if (upSpeed)
        {
            upSpeed = false;
            timeIsUp = false;

            movementSpeed *= 1.5f;
            dashSpeed *= 2;
            wallSlideSpeed *= 2;
            
            beginUpSpeedTime = Time.time;
        }
        if ((Time.time >= beginUpSpeedTime + upSpeedDuration) && !timeIsUp && !upSpeed)
        {
            timeIsUp = true;
            movementSpeed /= 2;
            dashSpeed /= 2;
            wallSlideSpeed /= 2;  
        }
    }
    private void CheckKnockback()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }
    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }
    private void ApplyMovement()
    {
        if (!isGrounded && !isWallSliding && movementInputDirection == 0 && !knockback)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove && !knockback)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }


        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
        //if (isGrounded)
        //{
        //    rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        //}

        //else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        //{
        //    Vector2 addForce = new Vector2(movementForceInAir * movementInputDirection, 0);
        //    rb.AddForce(addForce);

        //    if (Mathf.Abs(rb.velocity.x) > movementSpeed)
        //        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        //}

    }
    public bool GetDashStation()
    {
        return isDashing;
    }
    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);
    }
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
            amountOfJumpsLeft = amountOfJumps;

        if (isTouchingWall)
            canWallJump = true;

        if (amountOfJumpsLeft <= 0)
            canNormalJump = false;

        else
            canNormalJump = true;
    }
    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            //WallJump
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }
    private void WallJump()
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;

        }
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            jumpSoundEffect.Play();
        }
    }
    private void WallHop()
    {
        if (isWallSliding && movementInputDirection == 0 && canNormalJump)
        {
            isWallSliding = false;
            amountOfJumpsLeft--;

            Vector2 addForce = new Vector2(-facingDirection * wallHopForce * wallHopDirection.x, wallHopForce * wallHopDirection.y);
            rb.AddForce(addForce, ForceMode2D.Impulse);
        }
    }
    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            canMove = false;
            canFlip = false;

            anim.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }
        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHightMultiplier);
        }
        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }
        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }
        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time >= (lastDash + dashCoolDown))
                AttemptToDash();
        }
    }
    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0.0f);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }

            if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
            if (Input.GetButtonDown("Dash"))
            {
                if (Time.time >= (lastDash + dashCoolDown))
                    AttemptToDash();
            }

        }
    }
    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsMask);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsMask);

        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsMask);

        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }
    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }
    public int GetFacingDirection()
    {
        return facingDirection;
    }
    private void Flip()
    {
        if (!isWallSliding && canFlip && !knockback)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }

    }
    public void EnableFlip()
    {
        canFlip = true;
    }
    public void DisableFlip()
    {
        canFlip = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
