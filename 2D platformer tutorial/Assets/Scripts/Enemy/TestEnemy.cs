using Unity.VisualScripting;
using UnityEngine;

public class TestEnemy : MonoBehaviour

{

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

    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float patrolSpeed = 2f;

    private Vector2 patrolPosition;
    private int patrolDirection = 1;

    private Transform player;
    private bool isTargeting;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        patrolPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Start targeting
        if (!isTargeting && distanceToPlayer <= detectionRange && CanSeePlayer())
            isTargeting = true;

        // Stop targeting
        if (isTargeting && (distanceToPlayer > loseRange || !CanSeePlayer()))
        {
            isTargeting = false;
            patrolPosition = transform.position;
        }
            



        if (isTargeting)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }

        // Attack logic (keep yours)
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown && PlayerIsInRange())
        {
            cooldownTimer = 0f;
            Attack();
        }
    }

    private void Patrol()
    {
        float nextX = transform.position.x + patrolDirection * patrolSpeed * Time.deltaTime;
         
        // Clamp within patrol bounds
        if (nextX > patrolPosition.x + patrolDistance)
        {
            nextX = patrolPosition.x + patrolDistance;
            patrolDirection *= -1;
            Flip();
        }
        else if (nextX < patrolPosition.x - patrolDistance)
        {
            nextX = patrolPosition.x - patrolDistance;
            patrolDirection *= -1;
            Flip();
        }

        transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
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

        Vector2 direction = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceToPlayer, obstacleLayer | playerLayer);

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
