using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public bool isFacingRight = true;
    public ParticleSystem smokeFX;
    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;
    bool isGrounded; 

    [Header("spikeLayer")]
    public LayerMask spikeLayer;

    //[Header("wallCheck")]
    //public Transform wallCheckPos;
    //public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);
    //public LayerMask wallLayer;


    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    //[Header("wallMovement")]
    //public float wallSlideSpeed = 1f;
    //public bool isWallSliding;

    [Header("SlidingInAir")]
    public bool isAirSliding = false;

    //wall Jumping 
    bool iswallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.45f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    [Header("Slide")]
    public float slideSpeed = 10f;
    public float slideDuration = 0.5f;
    float slideTimer; 
    bool isSliding;

    [Header("Flop")]
    public float flopSpeed = 13f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    float coyoteTimeCounter;


    [Header("Acceleration")]
    public float groundAcceleration = 60f;
    public float groundDeceleration = 60f;

    public float iceAcceleration = 20f;
    public float iceDeceleration = 5f;

    bool isOnIce;
    public LayerMask iceLayer;


    bool wasGrounded;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        GroundCheck();
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }




        ProcessGravity();
        //ProcessWallSlide();
        //processWallJump();
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            float direction = transform.localScale.x;
            float speed = isGrounded ? slideSpeed : flopSpeed;
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            if (slideTimer <= 0)
    
                isSliding = false;
        }


            float targetSpeed = horizontalMovement * moveSpeed; // Calculate target speed based on input and move speed

            float accel = isOnIce ? iceAcceleration : groundAcceleration; // Use different acceleration values for ice and ground
            float decel = isOnIce ? iceDeceleration : groundDeceleration; // Use different deceleration values for ice and ground

            if (Mathf.Abs(horizontalMovement) > 0.01f) // If there is significant horizontal input, accelerate towards target speed
            {
                rb.linearVelocity = new Vector2(
                    Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.deltaTime),
                    rb.linearVelocity.y
                );
            }
            else // If there is no horizontal input, decelerate towards 0
            {
                rb.linearVelocity = new Vector2(
                    Mathf.MoveTowards(rb.linearVelocity.x, 0f, decel * Time.deltaTime),
                    rb.linearVelocity.y
                );
            }
            Flip();

        

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        //animator.SetBool("isWallSliding", isWallSliding);
        animator.SetBool("isSliding", isSliding);
    }

    //private bool WallJumpCheck()
    //{
    //    return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);

    //}



    private void ProcessGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Math.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    //private void ProcessWallSlide()
    //{
    //    if (!isGrounded && WallJumpCheck() && horizontalMovement != 0)
    //    {
    //        isWallSliding = true;
    //        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
    //    }
    //    else
    //    {
    //        isWallSliding = false;
    //    }
    //}

    //private void processWallJump()
    //{
    //    if (isWallSliding)
    //    {
    //        iswallJumping = false;
    //        wallJumpDirection = -transform.localScale.x;
    //        wallJumpTimer = wallJumpTime;

    //        CancelInvoke(nameof(CancelWallJump));
    //    }
    //    else if (wallJumpTimer > 0)
    //    {
    //        wallJumpTimer -= Time.deltaTime;
    //    }
    //}

    private void CancelWallJump()
    {
        iswallJumping = false;
    }

    public void Slide(InputAction.CallbackContext context)
    {
        if (context.performed && !isSliding)
        {
            StartSlide();
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        float direction = transform.localScale.x;
        rb.linearVelocity = new Vector2(direction * slideSpeed, 0f);


    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            if (coyoteTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining = maxJumps - 1;
                coyoteTimeCounter = 0;

                JumpFX();
            }
            // Air jump
            else if (jumpsRemaining > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                JumpFX();
            }
        } else if (context.canceled)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }


        //wall jump
        if (context.performed && wallJumpTimer > 0)
        {
            iswallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;
            JumpFX();

            //force flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime);

        }
    }

    private void JumpFX()
    {
        animator.SetTrigger("jump");
        smokeFX.Play();
    }


    private void GroundCheck()
    {
        wasGrounded = isGrounded;

        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheckPos.position,
            groundCheckSize,
            0f,
            Vector2.down,
            0.05f,
            groundLayer | spikeLayer
        );

        isGrounded = hit.collider != null && hit.normal.y > 0.5f;

        isOnIce = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, iceLayer);

        if (!wasGrounded && isGrounded)
            jumpsRemaining = maxJumps;
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;


            if (rb.linearVelocity.y == 0)
            {
                smokeFX.Play();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }



}