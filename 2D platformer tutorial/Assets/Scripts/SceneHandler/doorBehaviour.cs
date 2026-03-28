using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class doorBehaviour : MonoBehaviour
{
    public Animator animator;
    private bool isOpen = false;
    public GameObject player;
    private Collider2D door;

    public GameController gameController;


    void Start()
    {
        door = GetComponent<Collider2D>();
    }

    
    void Update()
    {
        if(isOpen)
        {
            System.Threading.Thread.Sleep(1500);
            SceneManager.LoadScene("StartScene");
        }

        if(isInRangeOfDoor())
        {
            animator.SetBool("Open", true);
        }
        else
        {
            animator.SetBool("Open", false);
        }


    }

    public void OpenDoor(InputAction.CallbackContext context)
    {
        if (!isOpen && context.performed && IsCollidingWithPlayer() && isInRangeOfDoor() && gameController.score == 5)
        {
            isOpen = true;
        }
    }

    private bool isInRangeOfDoor()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance < 5f; 
    }


    private bool IsCollidingWithPlayer()
    {
        return door.bounds.Contains(player.transform.position);
    }
}
