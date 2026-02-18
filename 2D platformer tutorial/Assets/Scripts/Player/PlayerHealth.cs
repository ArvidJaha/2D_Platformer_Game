using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int maxHealth = 3;
    private int currentHealth;
    

    public HealthUI HealthUI;

    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayerDeath; // event for player death
    void Start()
    {
        ResetHealth(); // initialize health

        spriteRenderer = GetComponent<SpriteRenderer>();
        GameController.OnReset += ResetHealth; // subscribe to game reset event
    }

    // Update is called once per frame

    private void OnCollisionEnter2D(Collision2D collision) //detects collision with spikes and calls TakeDamage
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            TakeDamage(1);
        }
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        HealthUI.SetMaxHearts(maxHealth);
    }



    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthUI.UpdateHearts(currentHealth);

        //Flash Red
        StartCoroutine(FlashRed());


        if (currentHealth <= 0)
        {
            // implement death logic
            OnPlayerDeath.Invoke(); // trigger player death event
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;    // turn red
        yield return new WaitForSeconds(0.2f); // wait 0.2 seconds
        spriteRenderer.color = Color.white;   // turn back
    }
}
