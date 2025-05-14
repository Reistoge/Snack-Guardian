using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedState : PlayerBaseState
{
    
     

    public DamagedState(Player player) : base(player)
    {
        stateName = "Damaged";
    }

    public override void Enter()
    {
        base.Enter();
    
        player.stopMovement();    
        player.activateDamagedMovement();
        player.playDamagedAnimation();
   
    }

    public override void Exit()
    {
        base.Exit();
            
        player.resetMovementStats();
        // player.Movement.setIsDamaged(false);
    }

    protected override void CheckTransitions()
    {
        
        if (player.isTakingDamage() == false)
        {
        
            player.StateMachine.ChangeState(new IdleState(player));
        }
        

    }
}







