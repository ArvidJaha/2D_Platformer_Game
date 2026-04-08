using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public Vector3 spawnPoint;
    public LevelGenerator levelGenerator;
    public GameObject exitTriggerPrefab;
    GameObject currentExit;
    public int score = 0;
    public int deathCount = 0;
    public TMP_Text scoreText;
    public TMP_Text deathText;

    public static event Action OnReset;
    private int numFishes;
    [SerializeField] private GameObject winScreen;



    private void Start()
    {
        numFishes = 3 + (int)(5 * PlayerPrefs.GetFloat("difficultyIntensity"));
        PlayerHealth.OnPlayerDeath += ResetGame; // subscribe to player death event
        ExitTrigger.OnPlayerEnteredExit += LoadLevel;
        player.transform.position = levelGenerator.spawnPos;
        Fish.OnFishCollected += IncreaseScore; // increment score when a fish is collected
        score = 0;
        scoreText.text = "Fish: 0 / " + numFishes; // or "Score: 0" for the tutorial one
        //SpawnExitTrigger();
    }

    void IncreaseScore(int value)
    {
        score += value;
        scoreText.text = "Fish: " + score + " / " + numFishes;

        if (score >= numFishes)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("YOU WIN");

        // stop player
        player.GetComponent<PlayerMovement>().enabled = false;

        winScreen.SetActive(true);
        System.Threading.Thread.Sleep(1500);
        SceneManager.LoadScene("EndScene");
    }

    //void SpawnExitTrigger()
    //{
    //    if (currentExit != null)
    //        Destroy(currentExit);

    //    currentExit = Instantiate(
    //        exitTriggerPrefab,
    //        levelGenerator.exitPos,
    //        Quaternion.identity
    //    );
    //}

    void LoadLevel()
    {
        levelGenerator.GenerateLevel();
        player.transform.position = levelGenerator.spawnPos;
        //SpawnExitTrigger();
    }
    void ResetGame()
    {        
        player.transform.position = levelGenerator.spawnPos; // reset player position
        OnReset.Invoke(); // trigger game reset event for other scripts to reset their states
        deathCount++;
        deathText.text = "Deaths: " + deathCount;
    }

    private void OnDestroy()
    {
        PlayerHealth.OnPlayerDeath -= ResetGame;
        ExitTrigger.OnPlayerEnteredExit -= LoadLevel;
        Fish.OnFishCollected -= IncreaseScore;
    }

}
