using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsToAddTextUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] Animator anim;
    private static readonly string ON_ENTER_ANIMATION = "onEnter";
    public static readonly string IDLE_ANIMATION_STATE = "idle";
    private static readonly int ON_ENTER_ANIMATION_HASH = Animator.StringToHash(ON_ENTER_ANIMATION);
    private static readonly int IDLE_ANIMATION_HASH = Animator.StringToHash(IDLE_ANIMATION_STATE);

    public static float ON_ENTER_ANIMATION_DURATION;  // Duration of the animation in seconds
    void Start()
    {
        ON_ENTER_ANIMATION_DURATION = getClipDuration(ON_ENTER_ANIMATION);
    }

    public void addPoints(string pointsToAdd)
    {
        pointsText.enabled = true;
        anim.Play(IDLE_ANIMATION_HASH);
        pointsText.text = pointsToAdd;
        anim.Play(ON_ENTER_ANIMATION_HASH);


    }
    public void hidePointsText()
    {
        pointsText.text = "";
        pointsText.enabled = false;
    }
    public float getClipDuration(string clipName)
    {
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) // Replace with your damaged animation's name
                return clip.length;
        }
        return 0f; // Or throw an exception if not found
    }


}
