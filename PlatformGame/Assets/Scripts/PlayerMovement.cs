using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;


    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    float moveInput = 0f;




    [Header("Jump Settings")]
    public int maxJumps = 2;
    private int jumpsLeft;  




    [Header("Ground Check Settings")]
    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 1)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsLeft--;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded)
        {
            jumpsLeft = maxJumps;
        }
    }
}

