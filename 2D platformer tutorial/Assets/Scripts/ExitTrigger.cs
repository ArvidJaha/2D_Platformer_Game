using System;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public static event Action OnPlayerEnteredExit;
    bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (other.CompareTag("Player"))
        {
            used = true;
            OnPlayerEnteredExit?.Invoke();
        }
    }
}
