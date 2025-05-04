
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FallState : PlayerBaseState
{
    public FallState(Player player) : base(player) {
         stateName = "Fall";
     }


    public override void Enter()
    {
        base.Enter();
        player.PlayerAnimation.playEnterFall();
        
        // player.IsFalling = true;
    }
    public override void Exit()
    {
        base.Exit();
        // player.IsFalling = false;
    }


    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (player.Movement.IsGrounded && player.Movement.verticalVelocity <= 0)
        {
            player.StateMachine.ChangeState(new IdleState(player));
            return;
        }

        if (player.Movement.IsTouchingWall && !player.Movement.IsGrounded)
        {
            player.StateMachine.ChangeState(new WallSlideState(player));
            return;
        }

        if (InputManager.jumpWasPressed &&
            player.Movement.NumberOfJumpsUsed < player.Movement.moveStats.numberOfJumpsAllowed)
        {
            player.StateMachine.ChangeState(new JumpState(player));
            return;
        }
    }
}