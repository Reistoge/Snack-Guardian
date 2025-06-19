using System;
using UnityEngine;
using UnityEngine.Video;

public class PlayerAnimHandler : MonoBehaviour
{

    public static readonly string IDLE_ANIMATION_STATE = "Idle";
    public static readonly string RUN_ANIMATION_STATE = "Run";
    public static readonly string JUMP_ANIMATION_STATE = "Jump";
    public static readonly string WALL_JUMP_ANIMATION_STATE = "WallJump";
    public static readonly string DASH_ANIMATION_STATE = "Dash";
    public static readonly string FALL_ANIMATION_STATE = "Fall";
    public static readonly string WALK_ANIMATION_STATE = "Walk";
    public static readonly string FAST_FALL_ANIMATION_STATE = "FastFall";
    public static readonly string DAMAGED_ANIMATION_STATE = "Damaged";
    public static readonly string BUMP_ANIMATION_STATE = "Bump";
    public static readonly string GAMEOVER_ANIMATION_STATE = "GameOver";

    public static readonly string WALL_SLIDE_ANIMATION_STATE = "WallSlide";

    private readonly int idleAnimationHash = Animator.StringToHash(IDLE_ANIMATION_STATE);
    private readonly int runAnimationHash = Animator.StringToHash(RUN_ANIMATION_STATE);
    private readonly int jumpAnimationHash = Animator.StringToHash(JUMP_ANIMATION_STATE);
    private readonly int wallJumpAnimationHash = Animator.StringToHash(WALL_JUMP_ANIMATION_STATE);
    private readonly int dashAnimationHash = Animator.StringToHash(DASH_ANIMATION_STATE);
    private readonly int fallAnimationHash = Animator.StringToHash(FALL_ANIMATION_STATE);
    private readonly int walkAnimationHash = Animator.StringToHash(WALK_ANIMATION_STATE);
    private readonly int fastFallSpeedAnimationHash = Animator.StringToHash(FAST_FALL_ANIMATION_STATE);
    private readonly int damagedAnimationHash = Animator.StringToHash(DAMAGED_ANIMATION_STATE);
    private readonly int bumpAnimationHash = Animator.StringToHash(BUMP_ANIMATION_STATE);
    private readonly int gameOverAnimationHash = Animator.StringToHash(GAMEOVER_ANIMATION_STATE);
    private readonly int wallSlideAnimationHash = Animator.StringToHash(WALL_SLIDE_ANIMATION_STATE);
    void OnEnable()
    {
        GameEvents.onInvincibilityStart += setInvincibleMaterial;
        GameEvents.onInvincibilityEnd += setDefaultMaterial;
    }
    void OnDisable()
    {
        GameEvents.onInvincibilityStart -= setInvincibleMaterial;
        GameEvents.onInvincibilityEnd -= setDefaultMaterial;
    }
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material invincibleMaterial;

    public void playEnterDash()
    {
        anim.Play(dashAnimationHash);
    }
    public void playEnterJump()
    {
        anim.Play(jumpAnimationHash);
    }
    public void playEnterWallJump()
    {
        anim.Play(wallJumpAnimationHash);
    }
    public void playEnterFall()
    {
        anim.Play(fallAnimationHash);
    }
    public void playEnterRun()
    {
        anim.Play(runAnimationHash);
    }
    public void playEnterIdle()
    {
        anim.Play(idleAnimationHash);
    }
    public void playEnterWalk()
    {
        anim.Play(walkAnimationHash);
    }
    public void playEnterFastFall()
    {
        anim.Play(fastFallSpeedAnimationHash);
    }

    public void playDamagedAnimation()
    {
        anim.Play(damagedAnimationHash);
    }

    public void setInvincibleMaterial()
    {
        sprite.material = invincibleMaterial;


    }
    public void setDefaultMaterial()
    {
        sprite.material = defaultMaterial;
    }

    // Example: Get the length of the "Damaged" animation clip
    public float getDamagedAnimationDuration()
    {
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == DAMAGED_ANIMATION_STATE) // Replace with your damaged animation's name
                return clip.length;
        }
        return 0f; // Or throw an exception if not found
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

    public void playGameOverAnimation()
    {
       anim.Play(gameOverAnimationHash);
    }

    public void playEnterWallSlide()
    {
         anim.Play(wallSlideAnimationHash);
    }
}