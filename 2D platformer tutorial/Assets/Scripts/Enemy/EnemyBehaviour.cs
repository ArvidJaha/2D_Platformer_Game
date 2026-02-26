using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
   public GameObject player;
   public float speed;
   public Animator animator;

   public bool isFacingRight = true;

   float horizontalMovement;

   public float distance;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        if (distance < 10)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            horizontalMovement = transform.position.x - player.transform.position.x;
        }
        
        Flip();
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }
}
