using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using Vector2 = UnityEngine.Vector2;

public class PlayerStateMachine
{
    private IPlayerState currentState;

    public void Initialize(IPlayerState startingState)
    {
        ChangeState(startingState);
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Update() => currentState?.Update();
    public void FixedUpdate() => currentState?.FixedUpdate();
    public IPlayerState CurrentState { get => currentState; }
}
