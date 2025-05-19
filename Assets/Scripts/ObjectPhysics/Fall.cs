using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fall : MonoBehaviour
{
    [Header("References")]
    public FallStats fallStats;
    [SerializeField] private Collider2D feetColl;
    [SerializeField] private Collider2D bodyColl;

    private Rigidbody2D rb;

    //movement vars
    private float horizontalVelocity;
    private bool isFacingRight;

    //collision check vars
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;



    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isbumpedHead;



    //jump vars
    [SerializeField] public float verticalVelocity { get; set; }


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
    [SerializeField] UnityEvent onObjectLanded;
    private bool wantToFall = true;
    public UnityEvent getOnObjectLanded()
    {
        return onObjectLanded;
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        isFacingRight = true;

    }


    public void Update()
    {
        // Keep essential checks that ALL states need
        countTimers();
        landCheck();

 
    }
    public void setActiveFeet(bool active)
    {
        feetColl.enabled = active;
    }
    public void setActiveBody(bool active)
    {
        bodyColl.enabled = active;
    }
    


    public void FixedUpdate()
    {
        // Keep essential physics that ALL states need
        collisionChecks();
        jump();
        fall();
        applyVelocity();
    }



    #region jumps
    public void resetJumpValuesOnPastApexThreshold()
    {
        isPastApexThreshold = false;
        isFastFalling = true;
        fastFallTime = fallStats.timeForUpwardsCancel;
        verticalVelocity = 0f;
    }
    public void initiateJump(int quantityOfJumps)
    {

        if (!isJumping)
        {
            isJumping = true;

        }

        jumpBufferTimer = 0f;
        numberOfJumpsUsed += quantityOfJumps;
        verticalVelocity = fallStats.initialJumpVelocity;

    }
    public void desactivateColliders()
    {
        feetColl.enabled = false;
        bodyColl.enabled = false;
    }
    public void activateColliders()
    {
        feetColl.enabled = true;
        bodyColl.enabled = true;
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
                apexPoint = Mathf.InverseLerp(fallStats.initialJumpVelocity, 0, verticalVelocity);
                if (apexPoint > fallStats.apexThreshold)
                {

                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }
                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < fallStats.apexHangTime)
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
                    verticalVelocity += fallStats.gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }
            // gravity on descending     
            else if (!isFastFalling)
            {
                verticalVelocity += fallStats.gravity * fallStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
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
            if (fastFallTime >= fallStats.timeForUpwardsCancel)
            {
                verticalVelocity += fallStats.gravity * fallStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < fallStats.timeForUpwardsCancel)
            {
                verticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0, (fastFallTime / fallStats.timeForUpwardsCancel));
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
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            verticalVelocity += fallStats.gravity + Time.fixedDeltaTime;
        }
    }
    #endregion
    #region landCheck
    public void landCheck()
    {
        if ((isJumping || isFalling) && isGrounded && verticalVelocity <= 0f)
        {

            resetJumpValues();

            onObjectLanded?.Invoke();
            numberOfJumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;


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
        else { coyoteTimer = fallStats.jumpCoyoteTime; }

        //wall jump buffer timer



    }

    #endregion
    #region Movement

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


        verticalVelocity = Mathf.Clamp(verticalVelocity, -fallStats.maxFallSpeed, 50f);
        verticalVelocity = Mathf.Clamp(verticalVelocity, -50, 50f);
        if (wantToFall)
        {
            rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }


    }


    #endregion
    #region collisionChecks
    void checkIsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);  // point where the boxCast is draw.

        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x, fallStats.groundDetectionRayLength); // size of the boxCast 
        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, fallStats.groundDetectionRayLength, fallStats.groundLayer);
        if (groundHit.collider != null)
        {
            // print(groundHit.collider.gameObject);
            isGrounded = true;

        }
        else { isGrounded = false; }

        #region Debug Visualization
        if (fallStats.debugShowIsGroundedBox)
        {
            Color rayColor;
            if (isGrounded)
            {
                rayColor = Color.green;

            }
            else { rayColor = Color.red; }
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * fallStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * fallStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - fallStats.groundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);

        }

        #endregion

    }
    public bool checkLandOnLayer(LayerMask layerMask)
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);  // point where the boxCast is draw.

        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x, fallStats.groundDetectionRayLength); // size of the boxCast 
        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, fallStats.groundDetectionRayLength, layerMask);
        if (groundHit.collider != null)
        {
            return true;

        }
        else { return false; }
    }
    public bool checkLandOnObjectTag(string tag)
    {
        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);  // point where the boxCast is draw.

        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x, fallStats.groundDetectionRayLength); // size of the boxCast 
        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, fallStats.groundDetectionRayLength, fallStats.groundLayer);
        if (groundHit && groundHit.collider && groundHit.collider.gameObject && groundHit.collider.gameObject.CompareTag(tag))
        {

            return true;

        }
        else { return false; }

    }

    private void bumpedHead()
    {

        Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetColl.bounds.size.x * fallStats.headWidth, fallStats.headDetectionRayLength);
        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, fallStats.headDetectionRayLength, fallStats.groundLayer);
        if (headHit.collider != null)
        {
            isbumpedHead = true;
        }
        else { isbumpedHead = false; }
        #region Debug Visualization
        if (fallStats.debugShowIsGroundedBox)
        {
            float headWidth = fallStats.headWidth;
            Color rayColor;
            if (isbumpedHead)
            {
                rayColor = Color.green;

            }
            else { rayColor = Color.red; }
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * fallStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * fallStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + fallStats.headDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);

        }


    }


    #endregion
    public void collisionChecks()
    {
        checkIsGrounded();
        bumpedHead();

    }

    public void stopFall()
    {
        // check if the variable setup is right
        wantToFall = false;
        isFalling = false;
        verticalVelocity = 0f;
        if(rb) rb.velocity = new Vector2(rb.velocity.x, 0f);
        
        isJumping = false;
        isFastFalling = false;
        fastFallTime = 0f;
        isPastApexThreshold = false;
        coyoteTimer = fallStats.jumpCoyoteTime;


    }
    public void startFall()
    {
        // check if the variable setup is right
        wantToFall = true;
        isFalling = true;
        verticalVelocity = 0f;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        isJumping = false;
        isFastFalling = false;
        fastFallTime = 0f;
        isPastApexThreshold = false;
        coyoteTimer = fallStats.jumpCoyoteTime;


    }

    internal void setFallStats(FallStats fallStats)
    {
        this.fallStats = fallStats;
    }



    #endregion




    #region getters and setters
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsbumpedHead { get => isbumpedHead; set => isbumpedHead = value; }

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
    public float HorizontalVelocity { get => horizontalVelocity; set => horizontalVelocity = value; }
    public bool IsFacingRight { get => isFacingRight; set => isFacingRight = value; }
    public RaycastHit2D GroundHit { get => groundHit; set => groundHit = value; }
    public RaycastHit2D HeadHit { get => headHit; set => headHit = value; }

    public int NumberOfJumpsUsed { get => numberOfJumpsUsed; set => numberOfJumpsUsed = value; }
    #endregion


}
