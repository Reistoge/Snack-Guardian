using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DamagedState : PlayerBaseState
{
    private float impactTimer;

    public DamagedState(Player player) : base(player)
    {
        stateName = "Damaged";
    }

    public override void Enter()
    {
        base.Enter();
        player.stopMovement();
        player.Movement.applyImpact(isDamaged: true);
        player.playDamagedAnimation();
        
        // Set timer like BumpState does through initiateBumpTimer
        impactTimer = player.Movement.moveStats.damageEffect.impactDuration;
    }

    public override void FixedUpdate()
    {
        player.Movement.collisionChecks();
        player.Movement.applyBumpedVelocity(true); // Apply gravity like BumpState
    }

    protected override void CheckTransitions()
    {
        // Similar to BumpState's transition check
        if (!player.isTakingDamage()  )
        {
            player.StateMachine.ChangeState(new IdleState(player));
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Movement.stopImpact();
        player.resetMovementStats();
    }
}
// public class DamagedState : PlayerBaseState
// {
//     private enum DamagePhase { Impact, Falling }
//     private DamagePhase currentPhase;
//     private float impactTimer;

//     public DamagedState(Player player) : base(player)
//     {
//         stateName = "Damaged";
//     }

//     public override void Enter()
//     {
//         base.Enter();
//         currentPhase = DamagePhase.Impact;
//         player.stopMovement();
//         player.Movement.applyImpact(isDamaged: true);
//         player.playDamagedAnimation();

//         // Get impact duration from effect
//         var damageEffect = player.Movement.moveStats.damageEffect;
//         impactTimer = damageEffect.impactDuration;
//     }

//     public override void FixedUpdate()
//     {
//         player.Movement.collisionChecks();

//         switch (currentPhase)
//         {
//             case DamagePhase.Impact:
//                 HandleImpactPhase();
//                 break;

//             case DamagePhase.Falling:
//                 HandleFallingPhase();
//                 break;
//         }
//     }

//     private void HandleImpactPhase()
//     {
//         player.Movement.applyBumpedVelocity(applyGravity: false);

//         impactTimer -= Time.fixedDeltaTime;
//         if (impactTimer <= 0)
//         {
//             currentPhase = DamagePhase.Falling;
//             player.Movement.startFallPhase();
//         }
//     }

//     private void HandleFallingPhase()
//     {
//         player.Movement.applyBumpedVelocity(applyGravity: true);
//     }

//     protected override void CheckTransitions()
//     {
//         // Only check for ground collision in falling phase
//         if (currentPhase == DamagePhase.Falling)
//         {
//             var damageEffect = player.Movement.moveStats.damageEffect;
//             bool hitGround = player.Movement.isGroundedOnLayer(damageEffect.groundLayerCheck);

//             if (hitGround)
//             {
//                 player.StateMachine.ChangeState(new IdleState(player));
//             }
//         }
//     }

//     public override void Exit()
//     {
//         base.Exit();
//         player.Movement.stopImpact();
//         player.resetMovementStats();
//     }
// }
// public class DamagedState : PlayerBaseState
// {



//     public DamagedState(Player player) : base(player)
//     {
//         stateName = "Damaged";
//     }

//     public override void Enter()
//     {
//         base.Enter();

//         player.stopMovement();
//         player.activateDamagedMovement();
//         player.playDamagedAnimation();



//     }
//     public override void FixedUpdate()
//     {
//         player.Movement.collisionChecks();
//         player.Movement.handleMove();
//         //player.Movement.jump();
//         // player.Movement.fall();
//         //player.Movement.wallSlide();
//         //player.Movement.wallJump();
//         // player.Movement.dash();
//         // Apply bump velocity
//         if (player.isTakingDamage())
//         {
//             player.Movement.applyBumpedVelocity(false);
//         }
//         else
//         {
//             player.Movement.fall();
//             player.Movement.applyVelocity();
//         }
//     }

//     public override void Exit()
//     {
//         base.Exit();

//         player.resetMovementStats();
//         player.Movement.resetLayer();

//         // player.Movement.setIsDamaged(false);
//     }

//     protected override void CheckTransitions()
//     {

//         // if (player.isTakingDamage() == false)
//         // {

//         //     player.StateMachine.ChangeState(new IdleState(player));
//         // }
//         if (player.Movement.IsGrounded)
//         {
//             player.StateMachine.ChangeState(new IdleState(player));
//         }




//     }
// }







