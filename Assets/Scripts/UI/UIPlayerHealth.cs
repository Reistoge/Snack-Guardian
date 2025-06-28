using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField] HealthBarUI healthBarUI;
    // use a healthBar component to encapsulate the event of the healthBar UI of the player.
    private void OnEnable()
    {
        GameEvents.onHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        GameEvents.onHealthChanged -= UpdateHealth;
    }
    private void UpdateHealth(float current, float max)
    {
        healthBarUI.updateHealth(current, max);
    }
  
}
