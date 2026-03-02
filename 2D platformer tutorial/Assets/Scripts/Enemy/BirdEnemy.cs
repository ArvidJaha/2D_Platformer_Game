using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public float normalSpeed;
    public float chaseSpeed;
    private Transform player;

    private bool isChasing = false;
    private bool isReturning = false;
    private bool isPatroling = true;

    private Vector2 startPoint;
    public float patrolDistance;
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

        // Clamp within patrol bounds
        if (nextX > startPoint.x + patrolDistance)
        {
            nextX = startPoint.x + patrolDistance;
            patrolDirection *= -1;
            Flip();
        }
        else if (nextX < startPoint.x - patrolDistance)
        {
            nextX = startPoint.x - patrolDistance;
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
}
