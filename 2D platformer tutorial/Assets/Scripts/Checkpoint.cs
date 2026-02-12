using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;
    public GameController gameController;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !activated)
        {
            activated = true;
            gameController.spawnPoint = transform.position;
        }
    }


}
