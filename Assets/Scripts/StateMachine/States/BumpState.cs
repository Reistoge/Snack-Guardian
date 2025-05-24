using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpState : PlayerBaseState
{



    public BumpState(Player player) : base(player)
    {
        stateName = "Bump";
    }

    public override void Enter()
    {
        base.Enter();
        player.stopMovement();
        player.Movement.applyImpact(isDamaged: false);
        player.initiateBumpTimer();
    }

    public override void FixedUpdate()
    {
        player.Movement.collisionChecks();
        player.Movement.applyBumpedVelocity(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.Movement.stopImpact();
        player.resetMovementStats();
    }




    protected override void CheckTransitions()
    {

        if (player.getIsBumping() == false)
        {

            player.StateMachine.ChangeState(new IdleState(player));
        }



    }
}
