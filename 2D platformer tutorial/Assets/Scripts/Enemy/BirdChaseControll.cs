using UnityEngine;

public class BirdChaseControll : MonoBehaviour
{
    public BirdEnemy bird;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            bird.PlayerDetected();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            bird.PlayerLost();
    }
}
