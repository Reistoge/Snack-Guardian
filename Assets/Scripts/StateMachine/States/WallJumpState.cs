public class WallJumpState : PlayerBaseState
{
    public WallJumpState(Player player) : base(player) {
         stateName= "WallJump";
     }

    public override void Enter()
    {
        base.Enter();
        // player.initiateWallJump();
        player.PlayerAnimation.playEnterWallJump();
    }
    public override void Exit()
    {
        base.Exit();
        // player.IsWallJumping = false;
        // player.UseWallJumpMoveStats = false;
    }

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (player.Movement.IsbumpedHead)
        {
            player.Movement.IsWallJumpFastFalling = true;
            player.StateMachine.ChangeState(new FallState(player));
            return;
        }

        if (!player.Movement.IsTouchingWall && !player.Movement.IsGrounded)
        {
            player.Movement.IsWallJumpFalling = true;
            player.StateMachine.ChangeState(new FallState(player));
            return;
        }

        if (player.Movement.IsGrounded)
        {
            player.Movement.resetWallJumpValues();
            player.StateMachine.ChangeState(new IdleState(player));
            return;
        }
    }
}