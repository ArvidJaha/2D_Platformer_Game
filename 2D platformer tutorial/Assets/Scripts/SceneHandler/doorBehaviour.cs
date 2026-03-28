using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class doorBehaviour : MonoBehaviour
{
    public Animator animator;
    private bool isOpen = false;
    public GameObject player;
    private Collider2D door;

    public GameControllerWithoutPCG gc;

    public int score = 0;

    public int scoreThreshold = 5;


    void Start()
    {
        door = GetComponent<Collider2D>();
    }

    
    void Update()
    {
        score = gc.score;
        if (isOpen)
        {
            System.Threading.Thread.Sleep(1500);
            SceneManager.LoadScene("StartScene");
        }

        if(isInRangeOfDoor() && scoreCheck())
        {
            animator.SetBool("Open", true);
        }
        else
        {
            animator.SetBool("Open", false);
        }


    }

    public bool scoreCheck()
    {
        return gc.score >= scoreThreshold;
    }

    public void OpenDoor(InputAction.CallbackContext context)
    {
        if (!isOpen && context.performed && IsCollidingWithPlayer() && isInRangeOfDoor() && scoreCheck())
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
