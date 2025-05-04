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

public interface IPlayerState
{
    void Enter();
    void Exit();
    void Update();
    void FixedUpdate();
    
}

