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





    private void Start()
    {
        PlayerHealth.OnPlayerDeath += ResetGame; // subscribe to player death event
        player.transform.position = spawnPoint;
        Fish.OnFishCollected += IncreaseScore; // increment score when a fish is collected
     
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
    }



}
