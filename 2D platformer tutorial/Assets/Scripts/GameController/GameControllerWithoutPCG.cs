using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameControllerWithoutPCG : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public Vector3 spawnPoint;
    public int score = 0;
    public TMP_Text scoreText;


    public static event Action OnReset;



    private void Start()
    {
        PlayerHealth.OnPlayerDeath += ResetGame; // subscribe to player death event
        player.transform.position = spawnPoint;
        Fish.OnFishCollected += IncreaseScore; // increment score when a fish is collected
        score = 0;
        scoreText.text = "Fish: 0 / 5"; // or "Score: 0" for the tutorial one

    }

    void IncreaseScore(int value)
    {
        score += value;
        Debug.Log("Score increased by " + value);
        Debug.Log("Current score: " + score);
        scoreText.text = "Score: " + score;
    }

    
    void ResetGame()
    {
        player.transform.position = spawnPoint; // reset player position
        OnReset.Invoke(); // trigger game reset event for other scripts to reset their states
    }

    private void OnDestroy()
    {
        PlayerHealth.OnPlayerDeath -= ResetGame;
        Fish.OnFishCollected -= IncreaseScore;
    }


}
