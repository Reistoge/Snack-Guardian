
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public abstract class PlayerBaseState : IPlayerState
{
    protected Player player;
    protected string stateName;

    protected PlayerBaseState(Player player)
    {
        this.player = player;
    }

    public virtual void Enter()
    {
        Debug.Log("Enter " + stateName);
    }
    public virtual void Exit()
    {
        Debug.Log("Exit " + stateName);
    }

    public virtual void Update()
    {
        // Keep essential checks that ALL states need
        player.Movement.countTimers();
        player.Movement.jumpChecks();
        player.Movement.landCheck();
        player.Movement.wallJumpCheck();
        player.Movement.wallSlideCheck();
        player.Movement.dashCheck();
        CheckTransitions();
    }

    public virtual void FixedUpdate()
    {
        // Keep essential physics that ALL states need
        player.Movement.collisionChecks();
        player.Movement.handleMove();
        player.Movement.jump();
        player.Movement.fall();
        player.Movement.wallSlide();
        player.Movement.wallJump();
        player.Movement.dash();
        player.Movement.applyVelocity();
    }



    protected virtual void CheckTransitions()
    {

        if (InputManager.dashWasPressed==true){
            
            if (player.Movement.IsGrounded && player.Movement.IsDashing )
            {
                player.StateMachine.ChangeState(new DashState(player));
            }
            else if (!player.Movement.IsGrounded && player.Movement.IsAirDashing)
            {
                player.StateMachine.ChangeState(new DashState(player));

            }
        }
        if (player.isTakingDamage() && !(this is DamagedState))
        {
            player.StateMachine.ChangeState(new DamagedState(player));
            return;
        }
        
    }
}