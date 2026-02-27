using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public Vector3 spawnPoint;
    public LevelGenerator levelGenerator;
    public GameObject exitTriggerPrefab;
    GameObject currentExit;
    int score = 0;
    public TMP_Text scoreText;

    public static event Action OnReset;




    private void Start()
    {
        PlayerHealth.OnPlayerDeath += ResetGame; // subscribe to player death event
        ExitTrigger.OnPlayerEnteredExit += LoadLevel;
        player.transform.position = levelGenerator.spawnPos;
        Fish.OnFishCollected += IncreaseScore; // increment score when a fish is collected
        SpawnExitTrigger();
    }

    void IncreaseScore(int value)
    {
        score += value;
        scoreText.text = "Score: " + score;
    }

    void SpawnExitTrigger()
    {
        if (currentExit != null)
            Destroy(currentExit);

        currentExit = Instantiate(
            exitTriggerPrefab,
            levelGenerator.exitPos,
            Quaternion.identity
        );
    }

    void LoadLevel()
    {
        levelGenerator.GenerateLevel();
        player.transform.position = levelGenerator.spawnPos;
        SpawnExitTrigger();
    }
    void ResetGame()
    {        
        player.transform.position = levelGenerator.spawnPos; // reset player position
        OnReset.Invoke(); // trigger game reset event for other scripts to reset their states
    }



}
