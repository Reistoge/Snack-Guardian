using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsTextUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] Animator anim;
    private static readonly string ON_ENTER_ANIMATION = "onEnter";
    private static readonly int ON_ENTER_ANIMATION_HASH = Animator.StringToHash(ON_ENTER_ANIMATION);
    [SerializeField] float waitTime = 0.01f;
    [SerializeField] float pointsToAddPerSecond = 1f; // How many points to add per second during the animation
    Coroutine updateCoinText;
 

    public void addPoints(float pointsToAdd)
    {
        anim.Play(ON_ENTER_ANIMATION_HASH);
        if (updateCoinText != null)
        {
            StopCoroutine(updateCoinText);
        }
        updateCoinText = StartCoroutine(updateCoinTextCoroutine(int.Parse(pointsText.text), int.Parse(pointsText.text) + Mathf.RoundToInt(pointsToAdd)));
    }

    IEnumerator updateCoinTextCoroutine(int oldValue, int newValue)
    {
        // Duration of the animation in seconds

        if (Mathf.Abs(newValue - oldValue) > 5)
        {
            float temp = oldValue;

            while (temp != newValue)
            {
                if (temp > newValue)
                {
                    // ensure it gets the exact value at the end
                    if (temp - pointsToAddPerSecond < newValue)
                    {
                        temp = newValue; // ensure it gets the exact value at the end
                        yield break;
                    }
                    temp -= pointsToAddPerSecond;
                }
                else if (temp < newValue)
                {
                    if (temp + pointsToAddPerSecond > newValue)
                    {
                        temp = newValue; // ensure it gets the exact value at the end
                        yield break;
                    }
                    else
                    {
                        temp += pointsToAddPerSecond;

                    }
                }

                yield return new WaitForSeconds(waitTime);
                pointsText.text = temp.ToString();

                //yield return null; // Wait for the next frame
            }
        }
        pointsText.text = newValue.ToString();
        pointsText.ForceMeshUpdate();
        updateCoinText = null;

    }
}
