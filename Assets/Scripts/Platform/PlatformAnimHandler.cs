using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAnimHandler : MonoBehaviour
{
    static readonly string BROKE_ANIMATION = "Broke";
    static readonly string LANDED_ANIMATION = "Landed";
    static readonly string IDLE_ANIMATION = "Idle";
    static readonly string REPAIR_ANIMATION = "Repair";

    private readonly int BROKE_ANIMATION_HASH = Animator.StringToHash(BROKE_ANIMATION);
    private readonly int LANDED_ANIMATION_HASH = Animator.StringToHash(LANDED_ANIMATION);
    private readonly int IDLE_ANIMATION_HASH = Animator.StringToHash(IDLE_ANIMATION);
    private readonly int REPAIR_ANIMATION_HASH = Animator.StringToHash(REPAIR_ANIMATION);

    [SerializeField] Animator anim;

    public void playOnLandedObjectAnimation()
    {
        // Play the animation for the object that landed on the platform
        anim.Play(LANDED_ANIMATION_HASH);
    }
    public void playBrokeAnimation()
    {
        anim.Play(BROKE_ANIMATION_HASH);

    }
    public void playIdleAnimation()
    {
        anim.Play(IDLE_ANIMATION_HASH);
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

    public void playRepairAnimation()
    {
        anim.Play(REPAIR_ANIMATION_HASH);
    }
}
