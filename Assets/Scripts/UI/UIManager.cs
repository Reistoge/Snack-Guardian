using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HealthBarUI healthBar;
    
    private void OnEnable()
    {
        GameEvents.OnHealthChanged += updateHealthUI;
        GameEvents.onPlayerDeath += handlePlayerDeath;
    }
    
    private void OnDisable()
    {
        GameEvents.OnHealthChanged -= updateHealthUI;
        GameEvents.onPlayerDeath -= handlePlayerDeath;
    }
    
    private void updateHealthUI(float current, float max)
    {
        healthBar.updateHealth(current, max);
    }
    
    private void handlePlayerDeath()
    {
        // Show death screen, etc.
    }
}