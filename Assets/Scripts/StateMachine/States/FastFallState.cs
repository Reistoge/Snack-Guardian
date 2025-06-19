

using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class FastFallState : PlayerBaseState
{
    public FastFallState(Player player) : base(player) { 
         stateName= "FastFall";
    }


    public override void Enter()
    {
        base.Enter();
        player.PlayerAnimation.playEnterFastFall();
         
        // player.IsFastFalling = true;
        // player.FastFallReleaseSpeed = player.verticalVelocity;
    }
    public override void Exit()
    {
        base.Exit();
        // player.IsFastFalling = false;
    }


    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (player.Movement.IsGrounded)
        {
            player.Movement.resetJumpValues();
            player.StateMachine.ChangeState(new IdleState(player));
            return;
        }
        // (player.Movement.IsWallSliding || player.Movement.IsWallSlideFalling)&&
        if (  player.Movement.IsTouchingWall && !player.Movement.IsGrounded)
        {
            player.StateMachine.ChangeState(new WallSlideState(player));
            return;
        }

        if (InputManager.jumpWasPressed &&
            player.Movement.NumberOfJumpsUsed < player.Movement.moveStats.numberOfJumpsAllowed)
        {
            player.Movement.IsFastFalling = false;
            player.StateMachine.ChangeState(new JumpState(player));
            return;
        }
    }
}