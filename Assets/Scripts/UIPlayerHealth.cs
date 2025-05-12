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
        healthSystem.onDamageTaken += UpdateHealthText;
    }
    private void OnDisable()
    {
        healthSystem.onDamageTaken -= UpdateHealthText;
    }
    private void UpdateHealthText()
    {
        healthText.text = "Health: " + healthSystem.getCurrentHealth().ToString();
    }


}
