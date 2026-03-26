using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public float normalSpeed;
    public float chaseSpeed;
    private Transform player;
    public Animator animator;

    private bool isChasing = false;
    private bool isReturning = false;
    private bool isPatroling = true;

    private Vector2 startPoint;
    public float patrolDistance;
    private int yDirection = 1; 
    private int patrolDirection = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (isChasing)
        {
            ChasePlayer();
        }
        else if (isReturning)
        {
            ReturnToPatrol();
        }
        else
        {
            Patrol();
        }

    }

    public void PlayerDetected()
    {
        if (!isChasing)
        {
            isChasing = true;
            isReturning = false;
            isPatroling = false;

        }
            
    }

    public void PlayerLost()
    {
        if (isChasing)
        {
            isChasing = false;
            isReturning = true;
            isPatroling = false;
        }
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position, 
            player.position, 
            chaseSpeed * Time.deltaTime
        );

        // Flip enemy to face player
        if ((player.position.x > transform.position.x && transform.localScale.x < 0) ||
            (player.position.x < transform.position.x && transform.localScale.x > 0))
        {
            Flip();
        }
    }

    private void ReturnToPatrol()
    {
        if (transform.position.Equals(startPoint))
        {
            isChasing = false;
            isReturning = false;
            isPatroling = true;

            yDirection = 1;
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                startPoint,
                normalSpeed * Time.deltaTime
            );

            if ((startPoint.x > transform.position.x && transform.localScale.x < 0) ||
            (startPoint.x < transform.position.x && transform.localScale.x > 0))
            {
                Flip();
            }

        }
        
    }

    private void Patrol()
    {
        float nextX = transform.position.x + patrolDirection * normalSpeed * Time.deltaTime;
        float nextY = transform.position.y + yDirection * 0.5f * Time.deltaTime;

        Vector3 scale = transform.localScale;
        if (scale.x != patrolDirection)
            Flip();

        // Clamp within patrol bounds
        if (nextX > startPoint.x + patrolDistance)
        {
            nextX = startPoint.x + patrolDistance;
            patrolDirection *= -1;
            Flip();
        }
        else if (nextX < startPoint.x)
        {
            nextX = startPoint.x;
            patrolDirection *= -1;
            Flip();
        }

        if (nextY > startPoint.y + 1)
        {
            nextY = startPoint.y + 1;
            yDirection *= -1;
        } else if (nextY < startPoint.y - 1)
        {
            nextY = startPoint.y - 1;
            yDirection *= -1;
        }

        transform.position = new Vector3(nextX, nextY, transform.position.z);
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
