using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HealthBarUI healthBar;
    
    private void OnEnable()
    {
        GameEvents.onHealthChanged += updateHealthUI;
        GameEvents.onGameOver += handlePlayerDeath;
    }
    
    private void OnDisable()
    {
        GameEvents.onHealthChanged -= updateHealthUI;
        GameEvents.onGameOver -= handlePlayerDeath;
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