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




    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown && PlayerIsInRange())
        {
            cooldownTimer = 0f;
            Attack();
        }
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


}
