using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMainMenuButton : MonoBehaviour, GameOverUIAnimation
{
  
    [SerializeField] Animator anim;

    private const string ON_ENTER = "onEnter";
    private readonly int onEnterHash = Animator.StringToHash(ON_ENTER);
    [SerializeField] float onEnterAnimationDelay = 0.5f; // Adjust this value as needed
    public void playGameOverButtonAnimation()
    {
        // Assuming you have an Animator component attached to this GameObject

        anim.Play(onEnterHash, 0);
    }
    public void playGameOverAnimation()
    {
        Invoke(nameof(playGameOverButtonAnimation), onEnterAnimationDelay);

    }
    public void goToMainMenu()
    {
        // Assuming you have a method to load the main menu scene
        GameManager.Instance.loadScene("MainMenu");
    }
}
