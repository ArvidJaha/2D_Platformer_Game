using System;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public static event Action OnPlayerEnteredExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnteredExit?.Invoke();
        }
    }
}
