using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInput playerInput;
    public static Vector2 movement;
    public static bool jumpWasPressed;
    public static bool jumpIsHeld;
    public static bool jumpWasReleased;
    public static bool dashWasPressed;

    public static bool runIsHeld;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private InputAction dashAction;

    private void Awake()
    {
        // we get the input actions from the asset.
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];
        dashAction = playerInput.actions["Dash"];
    }

    private void Update()
    {
        
        movement = moveAction.ReadValue<Vector2>();
        jumpWasPressed = jumpAction.WasPerformedThisFrame();
        jumpIsHeld = jumpAction.IsPressed();
        jumpWasReleased = jumpAction.WasReleasedThisFrame();
        runIsHeld = runAction.IsPressed(); 
        dashWasPressed = dashAction.WasPerformedThisFrame();
    }
}
