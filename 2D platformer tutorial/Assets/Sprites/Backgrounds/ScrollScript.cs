using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed = 0;
    void Start()
    {
        
    }

    void Update()
    {
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(speed * Time.time, 0f);
    }
}
