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

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats moveStats;
    [SerializeField] private Collider2D feetColl;
    [SerializeField] private Collider2D bodyColl;

    private Rigidbody2D rb;

    //movement vars
    private float horizontalVelocity;
    private bool isFacingRight;

    //collision check vars
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private RaycastHit2D wallHit;
    private RaycastHit2D lastWallHit;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isbumpedHead;
    [SerializeField] private bool isTouchingWall;


    //jump vars
    [SerializeField] public float verticalVelocity { get;  set; }


    [SerializeField] private bool isJumping;
    [SerializeField] private bool isFastFalling;
    [SerializeField] private bool isFalling;
    [SerializeField] private float fastFallTime;
    [SerializeField] private float fastFallReleaseSpeed;
    [SerializeField] private int numberOfJumpsUsed;

    //apex vars
    [SerializeField] private float apexPoint;
    [SerializeField] private float timePastApexThreshold;
    [SerializeField] private bool isPastApexThreshold;

    //jump buffer vars
    [SerializeField] private float jumpBufferTimer;
    [SerializeField] private bool jumpReleasedDuringBuffer;

    //coyote time vars
    [SerializeField] private float coyoteTimer;

    // wall slide
    [SerializeField] private bool isWallSliding;
    [SerializeField] private bool isWallSlideFalling;
    [SerializeField] private bool isWallFalling;

    //wall jump
    [SerializeField] private bool useWallJumpMoveStats;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private bool isWallJumpFastFalling;
    [SerializeField] private bool isWallJumpFalling;
    [SerializeField] private float wallJumpFastFallTime;
    [SerializeField] private float wallJumpFastFallReleaseSpeed;
    [SerializeField] private float wallJumpPostBufferTime;
    [SerializeField] private float wallJumpApexPoint;
    [SerializeField] private float timePastWallJumpApexThreshold;
    [SerializeField] private bool isPastWallJumpApexThreshold;


    // dash vars
    [SerializeField] private bool isDashing;
    [SerializeField] private bool isAirDashing;
    [SerializeField] private float dashTimer;
    [SerializeField] private float dashOnGroundTime;
    [SerializeField] private int numberOfDashesUsed;
    [SerializeField] private Vector2 dashDirection;
    [SerializeField] private bool isDashFastFalling;
    [SerializeField] private float dashFastFallTime;
    [SerializeField] private float dashFastFallReleaseSpeed;
 
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
   
        isFacingRight = true;
 
    }
     
    public void handleMove()
    {
        if (isGrounded)
        {
            move(moveStats.groundAcceleration, moveStats.groundDecceleration, InputManager.movement);
        }
        else
        {
            if (useWallJumpMoveStats)
            {
                move(moveStats.wallJumpMoveAcceleration, moveStats.wallJumpMoveDecceleration, InputManager.movement);
            }
            else
            {
                move(moveStats.airAcceleration, moveStats.airDecceleration, InputManager.movement);
            }
        }
    }
    #region jumps

    public void jumpChecks()
    {
        // WHEN WE PRESS THE JUMP BUTTON 
        if (InputManager.jumpWasPressed)
        {
            if (isWallSlideFalling && wallJumpPostBufferTime >= 0)
            {
                return;
            }
            else if (isWallSliding || (isTouchingWall && !isGrounded))
            {
                //just a wall jump
                return;
            }
            jumpBufferTimer = moveStats.jumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }
        // WHEN WE RELEASE THE BUTTON
        if (InputManager.jumpWasReleased)
        {

            if (jumpBufferTimer > 0)
            {
                jumpReleasedDuringBuffer = true;
            }
            if (isJumping && verticalVelocity > 0)
            {
                if (isPastApexThreshold)
                {
                    resetJumpValuesOnPastApexThreshold();
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = verticalVelocity;
                }
            }
        }
        // INITIATE JUMP WITH JUMP BUFFER TIME
        if (jumpBufferTimer > 0 && !isJumping && (isGrounded || coyoteTimer > 0f))
        {

            initiateJump(1);
            print("JUMP");
            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = verticalVelocity;
            }

        }
        // DOUBLE JUMP
        else if (jumpBufferTimer > 0 && (isJumping || isWallJumping || isWallSlideFalling || isAirDashing || isDashFastFalling) && !isTouchingWall && (numberOfJumpsUsed < moveStats.numberOfJumpsAllowed))
        {
            isFastFalling = false;
            initiateJump(1);
            print("DOUBLE JUMP");
            if (isDashFastFalling)
            {
                isDashFastFalling = false;
            }

        }
        // AIR JUMP AFTER COYOTE TIME ELAPSED
        else if (jumpBufferTimer > 0 && isFalling && !isWallSlideFalling && numberOfJumpsUsed < moveStats.numberOfJumpsAllowed - 1)
        {
            initiateJump(2);
            isFastFalling = false;
        }
        // LANDED 


    }
    public void resetJumpValuesOnPastApexThreshold()
    {
        isPastApexThreshold = false;
        isFastFalling = true;
        fastFallTime = moveStats.timeForUpwardsCancel;
        verticalVelocity = 0f;
    }
    public void initiateJump(int quantityOfJumps)
    {

        if (!isJumping)
        {
            isJumping = true;

        }
        resetWallJumpValues();
        jumpBufferTimer = 0f;
        numberOfJumpsUsed += quantityOfJumps;
        verticalVelocity = moveStats.initialJumpVelocity;

    }
    public void jump()
    {
        // apply gravity while jumping 
        if (isJumping)
        {
            // check for headBump

            if (isbumpedHead)
            {
                isFastFalling = true;

            }
            // gravity on ascending
            if (verticalVelocity >= 0f)
            {
                apexPoint = Mathf.InverseLerp(moveStats.initialJumpVelocity, 0, verticalVelocity);
                if (apexPoint > moveStats.apexThreshold)
                {

                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }
                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < moveStats.apexHangTime)
                        {
                            verticalVelocity = 0f;

                        }
                        else
                        {
                            verticalVelocity = -0.01f;
                        }
                    }
                }
                //gravity on ascending but not past apex threshold
                else if (!isFastFalling)
                {
                    verticalVelocity += moveStats.gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }
            // gravity on descending     
            else if (!isFastFalling)
            {
                verticalVelocity += moveStats.gravity * moveStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (verticalVelocity < 0f)
            {
                if (!isFalling)
                {
                    isFalling = true;
                }
            }



            // jump cut 
        }
        if (isFastFalling)
        {
            if (fastFallTime >= moveStats.timeForUpwardsCancel)
            {
                verticalVelocity += moveStats.gravity * moveStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < moveStats.timeForUpwardsCancel)
            {
                verticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0, (fastFallTime / moveStats.timeForUpwardsCancel));
            }
            fastFallTime += Time.fixedDeltaTime;
        }





    }
    public void resetJumpValues()
    {
        isJumping = false;
        isFalling = false;
        isFastFalling = false;
        fastFallTime = 0f;
        isPastApexThreshold = false;

    }
    #endregion
    #region fall
    public void fall()
    {
        // normal gravity while falling 
        if (!isGrounded && !isJumping && !isWallSliding && !isWallJumping && !isDashing && !isDashFastFalling)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            verticalVelocity += moveStats.gravity + Time.fixedDeltaTime;
        }
    }
    #endregion
    #region landCheck
    public void landCheck()
    {
        if ((isJumping || isFalling || isWallJumpFalling || isWallJumping || isWallSlideFalling || isWallSliding || isDashFastFalling) && isGrounded && verticalVelocity <= 0f)
        {

            resetJumpValues();
            stopWallSlide();
            resetWallJumpValues();
            resetDashes();

            numberOfJumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;

            if (isDashFastFalling && isGrounded)
            {
                resetDashValues();
                return;
            }
            resetDashValues();

        }
    }
    #endregion
    #region timers
    public void countTimers()
    {
        //jump buffer timer 
        jumpBufferTimer -= Time.deltaTime;

        //jump coyote timer
        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else { coyoteTimer = moveStats.jumpCoyoteTime; }

        //wall jump buffer timer
        if (!shouldApplyPostWallJumpBuffer())
        {
            wallJumpPostBufferTime -= Time.deltaTime;
        }
        if (isGrounded)
        {
            dashOnGroundTime -= Time.deltaTime;
        }


    }

    #endregion
    #region Movement
    public void move(float acceleration, float decceleration, Vector2 moveInput)
    {
        if (!isDashing)
        {
            if (Mathf.Abs(moveInput.x) >= moveStats.moveThreshold)
            {
                // check if he needs to turn 
                turnCheck(moveInput);
                float targetVelocity = 0;
                if (InputManager.runIsHeld)
                {
                    targetVelocity = moveInput.x * moveStats.maxRunSpeed;


                    // if(stateMachine.CurrentState != runState) // CRUCIAL TO ENTER THE STATE
                    // {
                    //     // first time entering run state 
                    //     stateMachine.ChangeState(runState);
                    // }
                }
                else
                {

                    targetVelocity = moveInput.x * moveStats.maxWalkSpeed;
                    // if(stateMachine.CurrentState != moveState) // CRUCIAL TO ENTER THE STATE
                    // {
                    //     // first time entering run state 
                    //     stateMachine.ChangeState(moveState);
                    // }


                }
                horizontalVelocity = Mathf.Lerp(horizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);


            }

            else if (Mathf.Abs(moveInput.x) < moveStats.moveThreshold)
            {
                horizontalVelocity = Mathf.Lerp(horizontalVelocity, 0, decceleration * Time.fixedDeltaTime);
                // if (stateMachine.CurrentState != idleState) // CRUCIAL TO ENTER THE STATE
                // {
                //     // first time entering idle state 
                //     stateMachine.ChangeState(idleState);
                // }



            }

        }
    }
    public void turnCheck(Vector2 moveInput)
    {
        // animacion.turn
        if (isFacingRight && moveInput.x < 0)
        {
            turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            turn(true);
        }
    }
    public void turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0, -180, 0f);
        }
    }
    public void applyVelocity()
    {
        if (!isDashing)
        {
            verticalVelocity = Mathf.Clamp(verticalVelocity, -moveStats.maxFallSpeed, 50f);

        }
        else
        {
            verticalVelocity = Mathf.Clamp(verticalVelocity, -50, 50f);
        }
        rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);

    }

    #endregion
    #region collisionChecks
    void checkIsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);  // point where the boxCast is draw.

        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x, moveStats.groundDetectionRayLength); // size of the boxCast 
        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, moveStats.groundDetectionRayLength, moveStats.groundLayer);
        if (groundHit.collider != null)
        {
            isGrounded = true;
        }
        else { isGrounded = false; }

        #region Debug Visualization
        if (moveStats.debugShowIsGroundedBox)
        {
            Color rayColor;
            if (isGrounded)
            {
                rayColor = Color.green;

            }
            else { rayColor = Color.red; }
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - moveStats.groundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);

        }

        #endregion

    }
    private void bumpedHead()
    {

        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x * moveStats.headWidth, moveStats.headDetectionRayLength);
        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, moveStats.headDetectionRayLength, moveStats.groundLayer);
        if (headHit.collider != null)
        {
            isbumpedHead = true;
        }
        else { isbumpedHead = false; }
        #region Debug Visualization
        if (moveStats.debugShowIsGroundedBox)
        {
            float headWidth = moveStats.headWidth;
            Color rayColor;
            if (isbumpedHead)
            {
                rayColor = Color.green;

            }
            else { rayColor = Color.red; }
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * moveStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * moveStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + moveStats.headDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);

        }


    }

    private void drawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPos = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);
        Vector2 previousPosition = startPos;
        float speed = 0;
        if (moveStats.drawRight)
        {
            speed = moveSpeed;
        }
        else
        {
            speed = -moveSpeed;

        }
        Vector2 velocity = new Vector2(speed, moveStats.initialJumpVelocity);
        Gizmos.color = gizmoColor;
        float timeStep = 2 * moveStats.timeTillJumpApex / moveStats.arcResolution;
        for (int i = 0; i < moveStats.visualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;
            if (simulationTime < moveStats.timeTillJumpApex)
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, moveStats.gravity) * simulationTime * simulationTime;

            }
            else if (simulationTime < moveStats.timeTillJumpApex + moveStats.apexHangTime)
            {
                float apexTime = simulationTime - moveStats.timeTillJumpApex;
                displacement = velocity * moveStats.timeTillJumpApex + 0.5f * new Vector2(0, moveStats.gravity) * moveStats.timeTillJumpApex * moveStats.timeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime; // No vertical movement during hang time

            } // Apex hang time


            else
            {
                float descendTime = simulationTime - (moveStats.timeTillJumpApex + moveStats.apexHangTime);
                displacement = velocity * moveStats.timeTillJumpApex + 0.5f * new Vector2(0, moveStats.gravity) * moveStats.timeTillJumpApex * moveStats.timeTillJumpApex;
                displacement += new Vector2(speed, 0) * moveStats.apexHangTime; // Horizontal movement during hang time
                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, moveStats.gravity) * descendTime * descendTime;
            }// Descending



            drawPoint = startPos + displacement;

            if (moveStats.stopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), moveStats.groundLayer);
                if (hit.collider != null)

                    // If a hit is detected, stop drawing the arc at the hit point
                    Gizmos.DrawLine(previousPosition, hit.point);
                break;

            }


            previousPosition = drawPoint;
            Gizmos.DrawLine(previousPosition, drawPoint);

        }

    }
    #endregion
    public void collisionChecks()
    {
        checkIsGrounded();
        bumpedHead();
        checkIsTouchingWall();
    }

    #endregion
    #region wallSlide and wallJump
    public void wallSlideCheck()
    {
        if (isTouchingWall && !isGrounded && !isDashing)
        {


            if (verticalVelocity < 0f && !isWallSliding)
            {

                resetJumpValues();
                resetWallJumpValues();
                resetDashValues();

                if (moveStats.resetDashOnWallSlide)
                {
                    resetDashes();

                }

                isWallSlideFalling = false;
                isWallSliding = true;

                if (moveStats.resetJumpOnWallSlide)
                {
                    numberOfJumpsUsed = 0;
                }
            }

        }
        else if (isWallSliding && !isTouchingWall && !isGrounded && !isWallSlideFalling)
        {
            isWallSlideFalling = true;
            stopWallSlide();
        }
        else
        {
            stopWallSlide();
        }

    }
    private void stopWallSlide()
    {
        if (isWallSliding)
        {
            numberOfJumpsUsed++;
            isWallSliding = false;

        }
    }
    public void wallSlide()
    {
        if (isWallSliding)
        {
            verticalVelocity = Mathf.Lerp(verticalVelocity, -moveStats.wallSlideSpeed, moveStats.wallSlideDeccelerationSpeed * Time.fixedDeltaTime);
        }

    }
    private void checkIsTouchingWall()
    {
        float originEndPoint = 0f;
        if (isFacingRight)
        {
            originEndPoint = bodyColl.bounds.max.x;

        }
        else { originEndPoint = bodyColl.bounds.min.x; }
        float adjustedHeight = bodyColl.bounds.size.y * moveStats.wallDetectionRayHeightMultiplier;
        Vector2 boxCastOrigin = new Vector2(originEndPoint, bodyColl.bounds.center.y);
        Vector2 boxCastSize = new Vector2(moveStats.wallDetectionRayLength, adjustedHeight);
        wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, moveStats.wallDetectionRayLength, moveStats.groundLayer);
        if (wallHit.collider != null)
        {
            lastWallHit = wallHit;
            isTouchingWall = true;
        }
        else { isTouchingWall = false; }
        #region Debug Visualization
        if (moveStats.debugShowWallDetectionBox)
        {
            Color rayColor;
            if (isTouchingWall)
            {
                rayColor = Color.green;

            }
            else { rayColor = Color.red; }
            Vector2 boxBottomLeft = new Vector2((boxCastOrigin.x - boxCastSize.x / 2), boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxBottomRight = new Vector2((boxCastOrigin.x + boxCastSize.x / 2), boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxTopLeft = new Vector2((boxCastOrigin.x - boxCastSize.x / 2), boxCastOrigin.y + boxCastSize.y / 2);
            Vector2 boxTopRight = new Vector2((boxCastOrigin.x + boxCastSize.x / 2), boxCastOrigin.y + boxCastSize.y / 2);
            Debug.DrawLine(boxBottomLeft, boxBottomRight, rayColor);
            Debug.DrawLine(boxBottomLeft, boxTopLeft, rayColor);
            Debug.DrawLine(boxTopLeft, boxTopRight, rayColor);
            Debug.DrawLine(boxTopRight, boxBottomRight, rayColor);
        }
        #endregion
    }

    public void wallJumpCheck()
    {
        if (shouldApplyPostWallJumpBuffer())
        {

            wallJumpPostBufferTime = moveStats.wallJumpPostBufferTime;

        }
        if (InputManager.jumpWasReleased && !isWallSliding && !isTouchingWall && isWallJumping)
        {
            if (verticalVelocity > 0f)
            {
                if (isPastWallJumpApexThreshold)
                {
                    isPastWallJumpApexThreshold = false;
                    isWallJumpFastFalling = true;
                    wallJumpFastFallTime = moveStats.timeForUpwardsCancel;
                    verticalVelocity = 0f;
                }
                else
                {
                    isWallJumpFastFalling = true;
                    wallJumpFastFallReleaseSpeed = verticalVelocity;

                }
            }
        }
        if (InputManager.jumpWasPressed && wallJumpPostBufferTime > 0f)
        {
            initiateWallJump();
        }


    }
    public void initiateWallJump()
    {
        if (!isWallJumping)
        {
            isWallJumping = true;
            useWallJumpMoveStats = true;
        }
        stopWallSlide();
        resetJumpValues();
        wallJumpTime = 0f;
        verticalVelocity = moveStats.initialWallJumpVelocity;
        int dirMultiplier = 0;
        Vector2 hitPoint = lastWallHit.collider.ClosestPoint(bodyColl.bounds.center);
        if (hitPoint.x > transform.position.x)
        {
            dirMultiplier = -1;
        }
        else { dirMultiplier = 1; }
        horizontalVelocity = MathF.Abs(moveStats.wallJumpDirection.x) * dirMultiplier;
    }
    public void wallJump()
    {
        if (isWallJumping)
        {
            wallJumpTime += Time.fixedDeltaTime;
            if (wallJumpTime >= moveStats.timeTillJumpApex)
            {
                useWallJumpMoveStats = false;
            }
            if (isbumpedHead)
            {
                isWallJumpFastFalling = true;
                useWallJumpMoveStats = false;
            }
            if (verticalVelocity >= 0f)
            {
                wallJumpApexPoint = Mathf.InverseLerp(moveStats.wallJumpDirection.y, 0, verticalVelocity);
                if (wallJumpApexPoint > moveStats.apexThreshold)
                {
                    if (!isPastWallJumpApexThreshold)
                    {
                        isPastWallJumpApexThreshold = true;
                        timePastWallJumpApexThreshold = 0f;
                    }
                    if (isPastWallJumpApexThreshold)
                    {
                        timePastWallJumpApexThreshold += Time.fixedDeltaTime;
                        if (timePastWallJumpApexThreshold < moveStats.apexHangTime)
                        {
                            verticalVelocity = 0f;
                        }
                        else
                        {
                            verticalVelocity = -0.01f;
                        }
                    }
                }
                // gravity in ascending but not past apex threshold
                else if (!isWallJumpFastFalling)
                {
                    verticalVelocity += moveStats.wallJumpGravity * Time.fixedDeltaTime;
                    if (isPastWallJumpApexThreshold)
                    {
                        isPastWallJumpApexThreshold = false;
                    }
                }
            }
            else if (!isWallJumpFastFalling)
            {
                verticalVelocity += moveStats.wallJumpGravity * Time.fixedDeltaTime;

            }
            else if (verticalVelocity < 0f)
            {
                if (!isWallJumpFalling)
                {
                    isWallJumpFalling = true;
                }
            }


        }
        //habndle wall jump cut time
        if (isWallJumpFastFalling)
        {
            if (wallJumpFastFallTime >= moveStats.timeForUpwardsCancel)
            {
                verticalVelocity += moveStats.wallJumpGravity * moveStats.wallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (wallJumpFastFallTime < moveStats.timeForUpwardsCancel)
            {
                verticalVelocity = Mathf.Lerp(wallJumpFastFallReleaseSpeed, 0, (wallJumpFastFallTime / moveStats.timeForUpwardsCancel));
            }
            wallJumpFastFallTime += Time.fixedDeltaTime;
        }
    }
    private bool shouldApplyPostWallJumpBuffer()
    {
        if (!isGrounded && (isTouchingWall || isWallSliding))
        {
            return true;

        }
        else { return false; }
    }
    public void resetWallJumpValues()
    {
        isWallSlideFalling = false;
        useWallJumpMoveStats = false;
        isWallJumping = false;
        isWallJumpFastFalling = false;
        isWallJumpFalling = false;
        isPastWallJumpApexThreshold = false;
        wallJumpFastFallTime = 0;
        wallJumpTime = 0;
    }

    #endregion

    #region dashes
    public void dashCheck()
    {
        if (InputManager.dashWasPressed)
        {
            if (isGrounded && dashOnGroundTime < 0 && !isDashing)
            {
                initiateDash();
            }
            // air dash
            else if (!isGrounded && !isDashing && numberOfDashesUsed < moveStats.numberOfDashes)
            {
                isAirDashing = true;
                initiateDash();
                //you left wall slide but within the wall jump post buffer
                if (wallJumpPostBufferTime > 0f)
                {
                    numberOfJumpsUsed--;
                    if (numberOfJumpsUsed < 0)
                    {
                        numberOfJumpsUsed = 0;
                    }
                }
            }
        }
    }
    public void initiateDash()
    {
        dashDirection = InputManager.movement;
        Vector2 closestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(dashDirection, moveStats.dashDirections[0]);
        for (int i = 0; i < moveStats.dashDirections.Length; i++)
        {
            if (dashDirection == moveStats.dashDirections[i])
            {
                closestDirection = dashDirection;
                break;
            }
            float distance = Vector2.Distance(dashDirection, moveStats.dashDirections[i]);
            bool isDiagonal = Mathf.Abs(moveStats.dashDirections[i].x) == 1 && Mathf.Abs(moveStats.dashDirections[i].y) == 1;
            if (isDiagonal)
            {
                distance -= moveStats.dashDiagnallyBias;
            }
            else if (distance < minDistance)
            {
                minDistance = distance;
                closestDirection = moveStats.dashDirections[i];
            }
        }
        //handle direction with no input
        if (closestDirection == Vector2.zero)
        {
            if (isFacingRight)
            {
                closestDirection = Vector2.right;
            }
            else { closestDirection = Vector2.left; }
        }

        dashDirection = closestDirection;
        numberOfDashesUsed++;
        isDashing = true;
        dashTimer = 0f;
        dashOnGroundTime = moveStats.timeBtwnDashesOnGround;
        resetJumpValues();
        resetWallJumpValues();
        stopWallSlide();
    }
    public void dash()
    {
        if (isDashing)
        {
            dashTimer += Time.fixedDeltaTime;
            if (dashTimer >= moveStats.dashTime)
            {
                if (isGrounded)
                {
                    resetDashes();
                }
                isAirDashing = false;
                isDashing = false;
                if (!isJumping && !isWallJumping)
                {
                    dashFastFallTime = 0f;
                    dashFastFallReleaseSpeed = verticalVelocity;
                    if (!isGrounded)
                    {
                        isDashFastFalling = true;
                    }
                }
                return;

            }
            horizontalVelocity = moveStats.dashSpeed * dashDirection.x;
            if (dashDirection.y != 0f || isAirDashing)
            {
                verticalVelocity = moveStats.dashSpeed * dashDirection.y;
            }


        }
        else if (isDashFastFalling)
        {
            if (verticalVelocity > 0)
            {
                if (dashFastFallTime < moveStats.dashTimeUpwardsCancel)
                {
                    verticalVelocity = Mathf.Lerp(dashFastFallReleaseSpeed, 0, (dashFastFallTime / moveStats.dashTimeUpwardsCancel));
                }
                else if (dashFastFallTime >= moveStats.dashTimeUpwardsCancel)
                {
                    verticalVelocity += moveStats.gravity * moveStats.dashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
                dashFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                verticalVelocity += moveStats.gravity * moveStats.dashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
        }
    }
    public void resetDashValues()
    {
        isDashFastFalling = false;
        dashOnGroundTime = -0.01f;

    }
    public void resetDashes()
    {
        numberOfDashesUsed = 0;

    }
    #endregion

    #region getters and setters
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsbumpedHead { get => isbumpedHead; set => isbumpedHead = value; }
    public bool IsTouchingWall { get => isTouchingWall; set => isTouchingWall = value; }
    public bool IsJumping { get => isJumping; set => isJumping = value; }
    public bool IsFastFalling { get => isFastFalling; set => isFastFalling = value; }
    public bool IsFalling { get => isFalling; set => isFalling = value; }
    public float FastFallTime { get => fastFallTime; set => fastFallTime = value; }
    public float FastFallReleaseSpeed { get => fastFallReleaseSpeed; set => fastFallReleaseSpeed = value; }
    public float ApexPoint { get => apexPoint; set => apexPoint = value; }
    public float TimePastApexThreshold { get => timePastApexThreshold; set => timePastApexThreshold = value; }
    public bool IsPastApexThreshold { get => isPastApexThreshold; set => isPastApexThreshold = value; }
    public float JumpBufferTimer { get => jumpBufferTimer; set => jumpBufferTimer = value; }
    public bool JumpReleasedDuringBuffer { get => jumpReleasedDuringBuffer; set => jumpReleasedDuringBuffer = value; }
    public float CoyoteTimer { get => coyoteTimer; set => coyoteTimer = value; }
    public bool IsWallSliding { get => isWallSliding; set => isWallSliding = value; }
    public bool IsWallSlideFalling { get => isWallSlideFalling; set => isWallSlideFalling = value; }
    public bool IsWallFalling { get => isWallFalling; set => isWallFalling = value; }
    public bool UseWallJumpMoveStats { get => useWallJumpMoveStats; set => useWallJumpMoveStats = value; }
    public bool IsWallJumping { get => isWallJumping; set => isWallJumping = value; }
    public float WallJumpTime { get => wallJumpTime; set => wallJumpTime = value; }
    public bool IsWallJumpFastFalling { get => isWallJumpFastFalling; set => isWallJumpFastFalling = value; }
    public bool IsWallJumpFalling { get => isWallJumpFalling; set => isWallJumpFalling = value; }
    public float WallJumpFastFallTime { get => wallJumpFastFallTime; set => wallJumpFastFallTime = value; }
    public float WallJumpFastFallReleaseSpeed { get => wallJumpFastFallReleaseSpeed; set => wallJumpFastFallReleaseSpeed = value; }
    public float WallJumpPostBufferTime { get => wallJumpPostBufferTime; set => wallJumpPostBufferTime = value; }
    public float WallJumpApexPoint { get => wallJumpApexPoint; set => wallJumpApexPoint = value; }
    public float TimePastWallJumpApexThreshold { get => timePastWallJumpApexThreshold; set => timePastWallJumpApexThreshold = value; }
    public bool IsPastWallJumpApexThreshold { get => isPastWallJumpApexThreshold; set => isPastWallJumpApexThreshold = value; }
    public bool IsDashing { get => isDashing; set => isDashing = value; }
    public bool IsAirDashing { get => isAirDashing; set => isAirDashing = value; }
    public float DashTimer { get => dashTimer; set => dashTimer = value; }
    public float DashOnGroundTime { get => dashOnGroundTime; set => dashOnGroundTime = value; }
    public int NumberOfDashesUsed { get => numberOfDashesUsed; set => numberOfDashesUsed = value; }
    public Vector2 DashDirection { get => dashDirection; set => dashDirection = value; }
    public bool IsDashFastFalling { get => isDashFastFalling; set => isDashFastFalling = value; }
    public float DashFastFallTime { get => dashFastFallTime; set => dashFastFallTime = value; }
    public float DashFastFallReleaseSpeed { get => dashFastFallReleaseSpeed; set => dashFastFallReleaseSpeed = value; }
    public float HorizontalVelocity { get => horizontalVelocity; set => horizontalVelocity = value; }
    public bool IsFacingRight { get => isFacingRight; set => isFacingRight = value; }
    public RaycastHit2D GroundHit { get => groundHit; set => groundHit = value; }
    public RaycastHit2D HeadHit { get => headHit; set => headHit = value; }
    public RaycastHit2D WallHit { get => wallHit; set => wallHit = value; }
    public RaycastHit2D LastWallHit { get => lastWallHit; set => lastWallHit = value; }
 
    public int NumberOfJumpsUsed { get => numberOfJumpsUsed; set => numberOfJumpsUsed = value; }
    #endregion


}


