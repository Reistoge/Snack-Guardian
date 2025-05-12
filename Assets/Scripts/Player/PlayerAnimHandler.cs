using System;
using UnityEngine;

public class PlayerAnimHandler : MonoBehaviour{
    
    private readonly int idleAnimationState = Animator.StringToHash("Idle");
    private readonly int runAnimationState = Animator.StringToHash("Run");
    private readonly int jumpAnimationState = Animator.StringToHash("Jump");
    private readonly int wallJumpAnimationState = Animator.StringToHash("WallJump");
    private readonly int dashAnimationState = Animator.StringToHash("Dash");
    private readonly int fallAnimationState = Animator.StringToHash("Fall");
    private readonly int walkAnimationState = Animator.StringToHash("Walk");
    private readonly int fastFallSpeedAnimationState = Animator.StringToHash("FastFall");
    private readonly int damagedAnimationState = Animator.StringToHash("Damaged");

    
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer sprite;
    
    public void playEnterDash(){
        anim.Play(dashAnimationState);
    }
    public void playEnterJump(){
        anim.Play(jumpAnimationState);
    }
    public void playEnterWallJump(){
        anim.Play(wallJumpAnimationState);
    }
    public void playEnterFall(){
        anim.Play(fallAnimationState);
    }
    public void playEnterRun(){
        anim.Play(runAnimationState);
    }
    public void playEnterIdle(){
        anim.Play(idleAnimationState);
    }
    public void playEnterWalk(){
        anim.Play(walkAnimationState);
    }
    public void playEnterFastFall(){
        anim.Play(fastFallSpeedAnimationState);
    }

    public void playDamagedAnimation()
    {
        anim.Play(damagedAnimationState);
    }
}