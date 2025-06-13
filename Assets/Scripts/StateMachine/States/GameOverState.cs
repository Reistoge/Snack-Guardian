using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverState : PlayerBaseState
{
    public GameOverState(Player player) : base(player) {
         stateName= "GameOverState";
     }

    public override void Enter()
    {

        base.Enter();
         GameEvents.triggerGameOver();
        player.PlayerAnimation.playGameOverAnimation();
        player.Movement.stopMovement(); // Stop player movement
        player.Movement.enabled = false; // Reset player movement state
    }
    public override void Exit()
    {
        base.Exit();
        // No specific cleanup needed
    }
    public override void Update()
    {
        // base.Update();
        // No specific update logic needed for game over state
    }
    public override void FixedUpdate()
    {
        // base.FixedUpdate();
        // No specific physics logic needed for game over state
        player.Movement.fall();
        player.Movement.applyVelocity();
    }

    protected override void CheckTransitions()
    {



    }
}
