using System.Collections;
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
            // fillImage.fillAmount = current / max;
            lerpFill(current / max, 0.3f);

        }
        if (healthText != null)
        {
            healthText.text = "Health:" + $"{Mathf.Round(current)}/{max}";
        }
    }

    public void lerpFill(float targetFill, float duration)
    {
        if (fillImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(lerpFillCoroutine(targetFill, duration));
        }
    }
    IEnumerator lerpFillCoroutine(float targetFill, float duration)
    {
        float startFill = fillImage.fillAmount;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(startFill, targetFill, time / duration);
            yield return null;
        }
        fillImage.fillAmount = targetFill;
    }
    
}