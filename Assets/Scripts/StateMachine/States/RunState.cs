using UnityEngine;

public class RunState : PlayerBaseState
{
    public RunState(Player  movement) : base(movement) {
         stateName= "Run";
     }

    public override void Enter()
    {
        base.Enter();
        player.PlayerAnimation.playEnterRun();
    }
    public override void Exit()
    {
        base.Exit();
    }

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (Mathf.Abs(InputManager.movement.x) < player.Movement.moveStats.moveThreshold)
        {
            player.StateMachine.ChangeState(new IdleState(player));
            return;
        }

        if (!InputManager.runIsHeld)
        {
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