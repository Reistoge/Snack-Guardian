using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverText : MonoBehaviour, GameOverUIAnimation
{
     
    [SerializeField] Animator anim;

    private const string ON_ENTER = "onEnter";
    private readonly int onEnterHash = Animator.StringToHash(ON_ENTER);
   [SerializeField] float onEnterAnimationDelay = 0.5f; // Adjust this value as needed
    public void playGameOverTextAnimation()
    {
        // Assuming you have an Animator component attached to this GameObject

        anim.Play(onEnterHash, 0);
    }
    public void playGameOverAnimation()
    {
        Invoke(nameof(playGameOverTextAnimation), onEnterAnimationDelay);
         
    }
}
