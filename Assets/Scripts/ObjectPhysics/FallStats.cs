using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "FallStats", menuName = "Snack Guardian/FallStats", order = 1)]
public class FallStats : ScriptableObject
{
    [Header("Grounded/Collision Checks")]
    public LayerMask groundLayer;
    public float groundDetectionRayLength = 0.02f;
    public float headDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float headWidth = 0.75f;
    public float wallDetectionRayLength = 0.125f;
    [Range(0f, 1f)] public float wallDetectionRayHeightMultiplier = 0.9f;

    [Header("Jump")]
    public float jumpHeight = 6.5f;
    [Range(0, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = 0.35f;
    [Range(0f, 5f)] public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;
    [Range(0, 5)] public int numberOfJumpsAllowed = 2;

    [Header("Reset jump option")]
    public bool resetJumpOnWallSlide = true;

    [Header("Jump Cut")]
    [Range(0f, 0.3f)] public float timeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0f, 1f)] public float apexThreshold = 0.97f;
    [Range(0f, 1f)] public float apexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;
    //jump
    public float gravity { get; private set; }
    public float initialJumpVelocity { get; private set; }
    public float adjustedJumpHeight { get; private set; }
    public float initialWallJumpVelocity { get; private set; }
    public bool debugShowIsGroundedBox;
    public bool debugShowHeadBumpBox;
    
   

    private void OnValidate()
    {
        calculateValues();
    }

    private void OnEnable()
    {
        calculateValues();
    }
    private void calculateValues()
    {
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        gravity = -(2 * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initialJumpVelocity = Mathf.Abs(gravity) * timeTillJumpApex;

    }
}
