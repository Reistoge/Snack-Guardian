
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class JumpState : PlayerBaseState
{
    public JumpState(Player player) : base(player) { 
        stateName= "Jump";
    }

    public override void Enter()
    {
        base.Enter();
        player.PlayerAnimation.playEnterJump();
        
        // player.initiateJump(1);
    }
    public override void Exit(){
        base.Exit();
        // player.IsJumping= false;
    }
    
    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (player.Movement.IsbumpedHead)
        {
            player.StateMachine.ChangeState(new FallState(player));
            return;
        }

        if (InputManager.jumpWasReleased && player.Movement.verticalVelocity > 0)
        {
            player.StateMachine.ChangeState(new FastFallState(player));
            return;
        }

        if (player.Movement.verticalVelocity <= 0)
        {
            player.StateMachine.ChangeState(new FallState(player));
            return;
        }
    }
}