using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Player Movement Stats", menuName = "Snack Guardian/PlayerMovementStats", order = 1)]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(0, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(0, 50f)] public float groundAcceleration = 5f;
    [Range(0, 50f)] public float groundDecceleration = 20f;
    [Range(0, 1)] public float moveThreshold = 0.25f;
    [Range(0, 50f)] public float airAcceleration = 5f;
    [Range(0, 50f)] public float airDecceleration = 5f;
    [Range(0, 50f)] public float wallJumpMoveAcceleration = 5f;
    [Range(0, 50f)] public float wallJumpMoveDecceleration = 5f;



    [Header("Run")]
    [Range(0, 100f)] public float maxRunSpeed = 20f;

    [Header("Grounded/Collision Checks")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask bumpedHeadLayer;
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

    [Header("Wall slide")]
    [Min(0f)] public float wallSlideSpeed = 5f;
    [Range(0f, 50f)] public float wallSlideDeccelerationSpeed = 50f;

    [Header("Wall jump")]
    public Vector2 wallJumpDirection = new Vector2(-20f, 6.5f);
    [Range(0f, 50f)] public float wallJumpPostBufferTime = 0.125f;
    [Range(0f, 50f)] public float wallJumpGravityOnReleaseMultiplier = 1f;

    [Header("Dash stats")]
    [Range(0f, 1f)] public float dashTime = .11f;
    [Range(0f, 200f)] public float dashSpeed = 40f;
    [Range(0, 1)] public float timeBtwnDashesOnGround = 0.225f;
    public bool resetDashOnWallSlide = true;
    [Range(0, 5)] public int numberOfDashes = 2;
    [Range(0, 0.5f)] public float dashDiagnallyBias = 0.4f;

    [Header("Dash cancel time")]
    [Range(0f, 5f)] public float dashGravityOnReleaseMultiplier = 1f;
    [Range(0f, 0.3f)] public float dashTimeUpwardsCancel = 0.027f;

 
    [Header("Debug")]
    public bool debugShowIsGroundedBox;
    public bool debugShowHeadBumpBox;
    public bool debugShowWallDetectionBox;

    [Header("JumpVisualization Tool")]
    public bool showWalkJumpArc = false;
    public bool showRunJumpArc = false;
    public bool stopOnCollision = true;
    public bool drawRight = true;
    [Range(5, 100)] public int arcResolution = 20;
    [Range(0, 500)] public int visualizationSteps = 90;

    [Header("Impact Settings")]
    public ImpactEffect bumpEffect;
    public ImpactEffect damageEffect;


    public readonly Vector2[] dashDirections = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1).normalized,
        new Vector2(0, 1),
        new Vector2(-1, 1).normalized,
        new Vector2(-1, 0),
        new Vector2(-1, -1).normalized,
        new Vector2(0, -1),
        new Vector2(1, -1).normalized,

    };



    //jump
    public float gravity { get; private set; }
    public float initialJumpVelocity { get; private set; }
    public float adjustedJumpHeight { get; private set; }

    // wall jump
    public float wallJumpGravity { get; private set; }
    public float initialWallJumpVelocity { get; private set; }
    public float adjustedWallJumpHeight { get; private set; }
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

        adjustedWallJumpHeight = wallJumpDirection.y * jumpHeightCompensationFactor;
        wallJumpGravity = -(2 * adjustedWallJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initialWallJumpVelocity = Mathf.Abs(wallJumpGravity) * timeTillJumpApex;
    }






}
