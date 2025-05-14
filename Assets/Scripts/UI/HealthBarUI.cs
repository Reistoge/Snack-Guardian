using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    
    public void updateHealth(float current, float max)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = current / max;

        }
        if (healthText != null)
        {
            healthText.text = "Health:" + $"{Mathf.Round(current)}/{max}";
        }
    }
}