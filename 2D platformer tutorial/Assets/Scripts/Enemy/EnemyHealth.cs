using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class EnemyHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int maxHealth = 2;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private bool dead = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame

    private void OnCollisionEnter2D(Collision2D collision) //detects collision with spikes and calls TakeDamage
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            TakeDamage(1);
        }
    }




    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //Flash Red
        StartCoroutine(FlashRed());


        if (currentHealth <= 0)
        {
            if (!dead)
            {
                dead = true;
                Destroy(gameObject); // destroy enemy object
            }

        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;    // turn red
        yield return new WaitForSeconds(0.2f); // wait 0.2 seconds
        spriteRenderer.color = Color.white;   // turn back
    }
}
