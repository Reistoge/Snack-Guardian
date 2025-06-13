using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameOverLight : MonoBehaviour
{
    void OnEnable()
    {
        GameEvents.onGameOver += playGameOverLightAnimationDelay;
    }
    void OnDisable()
    {
        GameEvents.onGameOver -= playGameOverLightAnimationDelay;
    }
    [SerializeField] Animator anim;

    private const string ON_ENTER = "onEnter";
    private readonly int onEnterHash = Animator.StringToHash(ON_ENTER);
    [SerializeField] float onEnterAnimationDelay = 0.5f; // Adjust this value as needed
    public void playGameOverLightAnimation()
    {
        // Assuming you have an Animator component attached to this GameObject
        Animator globalLightAnimator;
        GameObject.Find("GlobalLight").TryGetComponent<Animator>(out globalLightAnimator);
        if (globalLightAnimator == null)
        {
            Debug.LogError("GlobalLight Animator not found!");
            return;
        }
        else
        {
            globalLightAnimator.runtimeAnimatorController = anim.runtimeAnimatorController;
            globalLightAnimator.Play(onEnterHash, 0);
        }
        // anim.Play(onEnterHash, 0);
    }
    public void playGameOverLightAnimationDelay()
    {
        Invoke(nameof(playGameOverLightAnimation), onEnterAnimationDelay);
         
    }
}
