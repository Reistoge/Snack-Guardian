using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ImpactEffect", menuName = "Snack Guardian/Impact Effect")]
public class ImpactEffect : ScriptableObject
{
    [Header("Impact Phase Settings")]
    public float horizontalForce = 8f;
    public float verticalForce = 5f;
    public float impactDuration = 0.3f;
    public bool applyGravity = true;  // Added this

    [Header("Fall Phase Settings")]
    public bool hasFallPhase = false;
    public float fallGravity = 20f;
    public float maxFallSpeed = 15f;
    public LayerMask groundLayerCheck;

    [Header("Layer Settings")]
    public LayerMask includeLayers;
    public LayerMask excludeLayers;

    [Header("Debug Settings")]
    [SerializeField] private bool showDebugGizmos = false;
    public Color debugColor = Color.red;

    public Vector2 calculateImpactVelocity(Vector2 currentVelocity, bool isFacingRight)
    {
        float directionX = isFacingRight ? -1f : 1f;
        Vector2 baseVelocity = new Vector2(
            directionX * horizontalForce,
            verticalForce
        );

        // Clamp the velocities to prevent extreme values
        return new Vector2(
            clampVelocity(baseVelocity.x),
            clampVelocity(baseVelocity.y)
        );
    }

    private float clampVelocity(float velocity)
    {
        const float minSpeed = 3f;
        const float maxSpeed = 50f;
        
        if (velocity == 0) return 0;
        return velocity > 0 
            ? Mathf.Clamp(velocity, minSpeed, maxSpeed)
            : -Mathf.Clamp(-velocity, minSpeed, maxSpeed);
    }

    // Optional: Debug visualization
    public void DrawDebugGizmos(Vector3 position, Vector2 velocity)
    {
        if (!showDebugGizmos) return;
        
        Gizmos.color = debugColor;
        Gizmos.DrawRay(position, velocity);
        Gizmos.DrawWireSphere(position + (Vector3)velocity, 0.2f);
    }
}