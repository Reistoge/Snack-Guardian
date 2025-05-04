
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class IdleState : PlayerBaseState
{
    public IdleState(Player  player) : base(player) {
         stateName= "Idle";
     }

    public override void Enter()
    {
        base.Enter();
        player.PlayerAnimation.playEnterIdle();
        
         
        // player.HorizontalVelocity = 0;
        // player.resetJumpValues();
    }
    public override void Exit(){
        base.Exit();

    }

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (Mathf.Abs(InputManager.movement.x) > player.Movement.moveStats.moveThreshold)
        {
            if (InputManager.runIsHeld)
                player.StateMachine.ChangeState(new RunState(player));
            else
                player.StateMachine.ChangeState(new WalkState(player));
            return;
        }

        if (InputManager.jumpWasPressed && (player.Movement.IsGrounded || player.Movement.CoyoteTimer > 0))
        {
            player.StateMachine.ChangeState(new JumpState(player));
            return;
        }

        if (!player.Movement.IsGrounded)
        {
            player.StateMachine.ChangeState(new FallState(player));
            return;
        }
    }
}