using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EasyTextEffects;
public class SnackStreakUI : MonoBehaviour
{
    [SerializeField] TextEffect textEffect;
    [SerializeField] TextMeshProUGUI snackStreakText;
    [SerializeField] private Animator anim;
    private static readonly string ON_ENTER_ANIMATION = "onEnter";
    private static readonly int ON_ENTER_ANIMATION_HASH = Animator.StringToHash(ON_ENTER_ANIMATION);
    public void playStreakAnimation(int streakCount)
    {
        textEffect.StartManualEffects();
        snackStreakText.SetText("x " + streakCount);
        anim.Play(ON_ENTER_ANIMATION_HASH);
 
    }
}
