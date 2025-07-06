
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DashState : PlayerBaseState
{
    public DashState(Player  player) : base(player) { 
        stateName= "Dash";
    }

    public override void Enter()
    {
        base.Enter();

        player.PlayerAnimation.playEnterDash();
        player.setIsInvincible(true); // check if works with a spike.
        player.playParticle(PlayerVisualEffectHandler.ParticleType.dash);
     
        
        // player.initiateDash();
    }
    // DashState
    public override void Exit()
    {
        base.Exit();
        player.setIsInvincible(false);
        // player.IsDashing = false;
        // player.IsAirDashing = false;
    }
    

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (player.Movement.DashTimer >= player.Movement.moveStats.dashTime)
        {
            if (player.Movement.IsGrounded)
            {
                player.Movement.resetDashes();
                player.StateMachine.ChangeState(new IdleState(player));
            }
            else
            {
                // Remove duplicate flag management, handled in Exit()
                player.StateMachine.ChangeState(new FallState(player));
            }
            return;
        }
    }
}