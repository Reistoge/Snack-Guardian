using System;
using UnityEngine;
using UnityEngine.Video;

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
    
    public void setInvincibleMaterial( )
    {
        sprite.material = invincibleMaterial;
         
    
    }
    public void setDefaultMaterial()
    {
        sprite.material = defaultMaterial;
    }
}