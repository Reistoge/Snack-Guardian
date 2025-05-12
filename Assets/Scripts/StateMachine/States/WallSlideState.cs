public class WallSlideState : PlayerBaseState
{
    public WallSlideState(Player player) : base(player) {
        stateName = "WallSlide";
     }

    public override void Enter()
    {
        base.Enter();
        //player.PlayerAnimation.playEnterWallSlide();
        
        // player.IsWallSliding = true;
        // player.IsWallSlideFalling = false;
    }
    public override void Exit()
    {
        base.Exit();
        // player.IsWallSliding = false;
        // player.IsWallSlideFalling = false;
    }
    

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (InputManager.jumpWasPressed)
        {
            player.StateMachine.ChangeState(new WallJumpState(player));
            return;
        }

        if (!player.Movement.IsTouchingWall && !player.Movement.IsGrounded)
        {
            player.Movement.IsWallSlideFalling = true;
            player.StateMachine.ChangeState(new FallState(player));
            return;
        }

        if (player.Movement.IsGrounded)
        {
            player.StateMachine.ChangeState(new IdleState(player));
            return;
        }
        
    }
}