using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject gameOverScreen;
    public GameObject player;

    public static event Action OnReset;




    private void Start()
    {
        PlayerHealth.OnPlayerDeath += ResetGame; // subscribe to player death event
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }

    void ResetGame()
    {
        gameOverScreen.SetActive(false);
        player.transform.position = Vector3.zero; // reset player position
        OnReset.Invoke(); // trigger game reset event for other scripts to reset their states
    }

    private void LoadLevel(int level)
    {
        
    }


}
