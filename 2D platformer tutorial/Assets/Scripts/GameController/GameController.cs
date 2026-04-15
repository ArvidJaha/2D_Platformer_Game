using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ResetGame();
        }
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

        // Stop all movement
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Reset player movement state
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.isSliding = false;
            pm.enabled = false;

        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("yVelocity", 0f);
            anim.SetFloat("magnitude", 0f);
            anim.SetBool("isSliding", false);
        }

        OnReset.Invoke(); // trigger game reset event for other scripts to reset their states
        deathCount++;
        deathText.text = "Deaths: " + deathCount;
        StartCoroutine(ReenableMovement(pm));

    }

    private IEnumerator ReenableMovement(PlayerMovement pm)
    {
        yield return new WaitForSeconds(0.35f); // half second freeze after respawn
        if (pm != null) pm.enabled = true;
    }

    private void OnDestroy()
    {
        PlayerHealth.OnPlayerDeath -= ResetGame;
        ExitTrigger.OnPlayerEnteredExit -= LoadLevel;
        Fish.OnFishCollected -= IncreaseScore;
    }

}
