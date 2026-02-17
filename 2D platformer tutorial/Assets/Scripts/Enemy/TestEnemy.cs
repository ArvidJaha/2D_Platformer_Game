using UnityEngine;

public class TestEnemy : MonoBehaviour

{

    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private BoxCollider2D Collider;
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;


    

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown && PlayerIsInRange())
        {
            Attack();
        }
    }

    private void Attack()
    {
        cooldownTimer = 0f;
        // Implement attack logic, e.g., reduce player health
        Debug.Log("Enemy attacks for " + damage + " damage!");
    }

    private bool PlayerIsInRange()
    {
        RaycastHit2D hit = Physics2D.BoxCast(Collider.bounds.center, Collider.bounds.size, 0f, Vector2.left, 0f, playerLayer);
        return hit.collider != null; // Placeholder, replace with actual range check
    }

    
}
