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

    public static event Action OnReset;




    private void Start()
    {
        PlayerHealth.OnPlayerDeath += ResetGame; // subscribe to player death event
    }


    void ResetGame()
    {        
        player.transform.position = spawnPoint; // reset player position
        OnReset.Invoke(); // trigger game reset event for other scripts to reset their states
    }

    private void LoadLevel(int level)
    {
        
    }


}
