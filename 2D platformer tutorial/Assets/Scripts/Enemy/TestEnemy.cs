using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class TestEnemy : MonoBehaviour

{

    private Rigidbody2D rb;

    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float distance;
    [SerializeField] private int damage;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;
    private PlayerHealth playerHealth;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 2f;
    [SerializeField] private float loseRange = 4f;
    [SerializeField] private LayerMask obstacleLayer;
    private float loseTargetTimer = 0f;
    [SerializeField] private float loseTargetDelay = 2f; // tweak in Inspector

    private float patrolDistance;
    [SerializeField] private float patrolSpeed = 2f;

    private Vector2 patrolPosition;
    private int patrolDirection = 1;
    private Vector2 lastPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 0.2f;

    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    private Transform player;
    private bool isTargeting;

    private SpriteRenderer spriteRenderer;



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        patrolPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolDistance = UnityEngine.Random.Range(6f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Start targeting
        if (!isTargeting && distanceToPlayer <= detectionRange && CanSeePlayer())
        {
            isTargeting = true;
            loseTargetTimer = 0f;
        }

        // Delayed stop targeting
        if (isTargeting && (distanceToPlayer > loseRange || !CanSeePlayer()))
        {
            loseTargetTimer += Time.deltaTime;
            if (loseTargetTimer >= loseTargetDelay)
            {
                isTargeting = false;
                patrolPosition = transform.position;
                loseTargetTimer = 0f;
            }
        }
        else if (isTargeting)
        {
            // Reset timer if player comes back into view
            loseTargetTimer = 0f;
        }




        if (isTargeting)
        {
            FollowPlayer();
            spriteRenderer.color = Color.red;
        }
        else
        {
            Patrol();
            spriteRenderer.color = Color.white;
        }

        // Attack logic (keep yours)
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown && PlayerIsInRange())
        {
            cooldownTimer = 0f;
            //Attack();
        }
    }

    private bool GroundAhead()
    {
        Vector2 checkPos = transform.position + new Vector3(patrolDirection * 0.5f, -0.5f, 0);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void Patrol()
    {
        float nextX = transform.position.x + patrolDirection * patrolSpeed * Time.deltaTime;

        if (!GroundAhead())
        {
            patrolDirection *= -1;
            Flip();
            return;
        }

        if (nextX > patrolPosition.x + patrolDistance)
        {
            patrolDirection = -1;
            nextX = patrolPosition.x + patrolDistance;
            Flip();
        }
        else if (nextX < patrolPosition.x - patrolDistance)
        {
            patrolDirection = 1;
            nextX = patrolPosition.x - patrolDistance;
            Flip();
        }

        // Stuck detection
        if (Vector2.Distance(transform.position, lastPosition) < 0.001f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                patrolDirection *= -1;
                Flip();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
        rb.linearVelocity = new Vector2(patrolDirection * patrolSpeed, rb.linearVelocity.y);

    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    private bool CanSeePlayer()
    {
        if (player == null) return false;

        // Only check horizontal distance for detection
        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);
        if (horizontalDistance > detectionRange) return false;

        // Optional: restrict vertical range so enemy ignores player far above/below
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);
        if (verticalDistance > 1.5f) return false; // tweak this value to taste

        // Still raycast to check for walls between enemy and player
        Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, horizontalDistance, obstacleLayer | playerLayer);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private bool PlayerIsInRange()
    {
        
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * distance,
            new Vector3 (boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0f, Vector2.left, 0f, playerLayer);
        if (hit.collider != null)
        {
            playerHealth = hit.collider.GetComponent<PlayerHealth>();
        }

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * distance,
            new Vector3 (boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z)
        );
    }

    private void Attack() 
    {
        if (PlayerIsInRange())
        {
            playerHealth.TakeDamage(damage);

        }
    }

    private void FollowPlayer()
    {
        if (!GroundAhead())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Smooth movement toward player
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        // Flip enemy to face player
        if ((player.position.x > transform.position.x && transform.localScale.x < 0) ||
            (player.position.x < transform.position.x && transform.localScale.x > 0))
        {
            Flip();
        }
    }

}
