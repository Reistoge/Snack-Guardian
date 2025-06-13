using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    void OnEnable()
    {
        GameEvents.onGameOver += playGameOverUIAnimation;
    }

    private void playGameOverUIAnimation()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            child.TryGetComponent<GameOverUIAnimation>(out var animationComponent);
            if (animationComponent != null)
            {
                animationComponent.playGameOverAnimation();
            }
            else
            {
                Debug.LogWarning($"No GameOverUIAnimation component found on {child.name}");
            }
        }
    }
    

    void OnDisable()
    {
        GameEvents.onGameOver -= playGameOverUIAnimation;
    }
}
interface GameOverUIAnimation
{
    void playGameOverAnimation();
}

