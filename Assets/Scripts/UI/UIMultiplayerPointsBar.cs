using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMultiplayerPointsBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    public Image FillImage { get => fillImage; set => fillImage = value; }

    public void updateMultiplayerBarPoints(float current, float max)
    {
        if (FillImage != null)
        {
            // fillImage.fillAmount = current / max;
            lerpFill(current / max, 0.3f);

        }
        if (healthText != null)
        {
            healthText.text = "Points:" + $"{Mathf.Round(current)}/{max}";
        }
    }

    public void lerpFill(float targetFill, float duration)
    {
        if (FillImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(lerpFillCoroutine(targetFill, duration));
        }
    }
    IEnumerator lerpFillCoroutine(float targetFill, float duration)
    {
        float startFill = FillImage.fillAmount;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            FillImage.fillAmount = Mathf.Lerp(startFill, targetFill, time / duration);
            yield return null;
        }
        FillImage.fillAmount = targetFill;
    }

}
