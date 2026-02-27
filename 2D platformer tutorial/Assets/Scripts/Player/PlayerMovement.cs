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

    [Header("wallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask wallLayer;


    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("wallMovement")]
    public float wallSlideSpeed = 1f;
    public bool isWallSliding;

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

    [SerializeField] private float currentMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentMoveSpeed = Mathf.Abs(rb.linearVelocity.x);

        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        processWallJump();
        SlidingInAir();

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            float direction = transform.localScale.x;
            float speed = isGrounded ? slideSpeed : flopSpeed;
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            if (slideTimer <= 0)
                isSliding = false;

            return; // cancels normal movement while sliding
        }

        if (!iswallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();

        }

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
    }

    private bool WallJumpCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);

    }



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

    private void ProcessWallSlide()
    {
        if (!isGrounded && WallJumpCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void processWallJump()
    {
        if (isWallSliding)
        {
            iswallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

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
        if (jumpsRemaining > 0)
        {
            if (context.performed) //hold jump, max height
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                JumpFX();
            }
            else if (context.canceled) //light tap of jump, less height
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
                JumpFX();

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
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer | spikeLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
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
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }

    private void SlidingInAir(InputAction.CallbackContext context)
    {
        if (!isGrounded && context.performed)
        {
            isAirSliding = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 1.5f, rb.linearVelocity.y);
        }
    }
    private void SlideFX()
    {
        animator.SetTrigger("slide");
    }
}