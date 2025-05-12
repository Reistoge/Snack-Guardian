using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] HealthSystem healthSystem;


    private void OnEnable()
    {
        GameManager.onPlayerLoaded += loadHealthSystem;
        
        

    }
    private void OnDisable()
    {
        GameManager.onPlayerLoaded -= loadHealthSystem;
        if (healthSystem != null)
        {
            healthSystem.onDamageTaken -= UpdateHealthText;
        }
    }
    private void UpdateHealthText()
    {
        if (healthSystem == null)
        {
            Debug.LogWarning("Health system is not loaded yet.");
            healthSystem = GameManager.Instance.getPlayerHealthSystem();
            return;
        }
        healthText.text = "Health: " + healthSystem.getCurrentHealth().ToString();
    }

    void loadHealthSystem()
    {
        // Get the player object from the GameManager
        healthSystem = GameManager.Instance.getPlayerHealthSystem();
        healthSystem.onDamageTaken += UpdateHealthText;
        loadHealthText();
        // Update the health text initially
    }

    private void loadHealthText()
    {
        if (healthSystem == null)
        {
            Debug.LogWarning("Health system is not loaded yet.");
            healthSystem = GameManager.Instance.getPlayerHealthSystem();
            return;
        }
        healthText.text = "Health: " + healthSystem.getMaxHealth().ToString();
    }
}
