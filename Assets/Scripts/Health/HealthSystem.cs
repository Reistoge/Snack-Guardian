using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth;
    [SerializeField] float isTakingDamageTime = 0.5f;
    [SerializeField] float invincibilityTime = 2f;

    // [SerializeField] UIHealth healthBar;
    
    public Action onDamageTaken;

    public Action onDie;


    bool isTakingDamage;
    bool isInvincible;

    void Start()
    {
        currentHealth = maxHealth;
        GameEvents.triggerHealthChanged(currentHealth, maxHealth);

        // healthBar.SetMaxHealth(maxHealth);
    }

    public void handleDamage(float damage)
    {
        currentHealth -= damage;
        GameEvents.triggerHealthChanged(currentHealth, maxHealth);
        startDamageTimer();
        // healthBar.SetHealth(currentHealth);
        // onDamageTaken?.Invoke();
        if (currentHealth <= 0)
        {
            handleDie();
        }
    }
    void handleDie()
    {
        // Handle player death (e.g., respawn, game over, etc.)
        Debug.Log("object has die.");
        //GameEvents.triggerPlayerDeath();
        onDie?.Invoke();
    }
    public float getCurrentHealth()
    {
        return currentHealth;
    }
    public float getMaxHealth()
    {
        return maxHealth;
    }

    public void handleHeal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        GameEvents.triggerHealthChanged(currentHealth, maxHealth);
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
        startInvincibilityTimer();


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
        GameEvents.triggerInvincibilityStart();
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
        GameEvents.triggerInvincibilityEnd();
    }


}
