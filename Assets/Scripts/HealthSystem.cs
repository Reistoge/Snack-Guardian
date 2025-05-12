using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth;
    [SerializeField] float isTakingDamageTime = 1f;
    
    // [SerializeField] UIHealth healthBar;
    
    public Action onDamageTaken;

    bool isTakingDamage;
    bool isInvincible;



    void Start()
    {
        currentHealth = maxHealth;
        
        // healthBar.SetMaxHealth(maxHealth);
    }

    public void handleDamage(float damage)
    {
        currentHealth -= damage;
        startDamageTimer();
        // healthBar.SetHealth(currentHealth);
        onDamageTaken?.Invoke();

        if (currentHealth <= 0)
        {
            die();
        }
    }

    void die()
    {
        // Handle player death (e.g., respawn, game over, etc.)
        Debug.Log("Player has died.");

         
    }
    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public void handleHeal(float amount)
    {

    }

    public bool getIsTakingDamage()
    {
        return isTakingDamage;
    }
 
    public bool canTakeDamage()
    {
        return isTakingDamage == false && isInvincible == false;
    }
    public void startDamageTimer()
    {
        if (!isTakingDamage)
        {
            StartCoroutine(startDamageTimerCoroutine());
        }
    }
    IEnumerator startDamageTimerCoroutine()
    {
        isTakingDamage = true;
        yield return new WaitForSeconds(isTakingDamageTime);
        isTakingDamage = false;

     
    }
    public void startInvincibilityTimer()
    {
        if (!isInvincible)
        {
            StartCoroutine(startInvincibilityTimerCoroutine());
        }
    }
    IEnumerator startInvincibilityTimerCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(isTakingDamageTime);
        isInvincible = false;
    }


}
