using System.Collections;
using UnityEngine;

public class WalkState : PlayerBaseState
{
    Coroutine coroutine;
    public WalkState(Player player) : base(player)
    {
        stateName = "Walk";
    }

    public override void Enter()
    {
        base.Enter();
        player.PlayerAnimation.playEnterWalk();

        player.playParticle(PlayerVisualEffectHandler.ParticleType.walk);
      
        
    }
    public override void Exit()
    {
        player.stopParticle(PlayerVisualEffectHandler.ParticleType.walk);
        base.Exit();
        // No specific cleanup needed
    }
 


    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (Mathf.Abs(InputManager.movement.x) < player.Movement.moveStats.moveThreshold)
        {
            player.StateMachine.ChangeState(new IdleState(player));
            return;
        }

        if (InputManager.runIsHeld)
        {
            player.StateMachine.ChangeState(new RunState(player));
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