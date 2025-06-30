using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GameSceneCamera : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    
    [Header("Player Follow Settings")]
    [SerializeField] private int normalPPU = 24;
    [SerializeField] private int zoomedPPU = 48;
    [SerializeField] private float zoomDuration = 1f;
    [SerializeField] private Vector3 playerOffset = new Vector3(0, 2, 0);
    [SerializeField] private float followSpeed = 5f; // Speed for following player
    
    private bool isZoomedOnPlayer = false;
    private bool isFollowingPlayer = false; // Add this flag
    private Vector3 originalCameraPosition;
    private int originalPPU;
    private Transform playerTransform;
    private Coroutine currentZoomCoroutine;

    void Start()
    {
        // Store original camera settings
        originalCameraPosition = transform.position;
        
        if (TryGetComponent<PixelPerfectCamera>(out var cam))
        {
            originalPPU = cam.assetsPPU;
        }
        else
        {
            originalPPU = normalPPU;
        }
        
    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TogglePlayerZoom();
        }

        // Follow player if in follow mode
        if (isFollowingPlayer && playerTransform != null)
        {
            // FollowPlayer();
        }
    }

    void OnEnable()
    {
        GameEvents.onPlayerDamaged += shakeCameraOnDamage;
        GameEvents.onGameOver += shakeCameraGameOver;
        GameEvents.onPlayerSpawned += getPlayerTransform;
    }

    void OnDisable()
    {
        GameEvents.onPlayerDamaged -= shakeCameraOnDamage;
        GameEvents.onGameOver -= shakeCameraGameOver;
        GameEvents.onPlayerSpawned -= getPlayerTransform;
    }

    public void getPlayerTransform(GameObject player)
    {
        playerTransform = player.transform;
    }

    private void FindPlayerTransform()
    {
        // Try to find player by tag first
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            return;
        }

        // Fallback: find by component
        Player playerComponent = FindObjectOfType<Player>();
        if (playerComponent != null)
        {
            playerTransform = playerComponent.transform;
        }
    }

    private void FollowPlayer()
    {
        if (playerTransform == null) return;

        Vector3 targetPosition = playerTransform.position + playerOffset;
        targetPosition.z = originalCameraPosition.z; // Keep original Z

        // Smoothly move camera to follow player
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    public void TogglePlayerZoom()
    {
        if (playerTransform == null)
        {
            FindPlayerTransform();
            if (playerTransform == null)
            {
                Debug.LogWarning("Player not found! Cannot zoom to player.");
                return;
            }
        }

        // Stop any current zoom coroutine
        if (currentZoomCoroutine != null)
        {
            StopCoroutine(currentZoomCoroutine);
        }

        if (isZoomedOnPlayer)
        {
            // Stop following and zoom back to original position
            isFollowingPlayer = false;
            currentZoomCoroutine = StartCoroutine(ZoomToOriginal());
        }
        else
        {
            // Zoom to player and start following
            currentZoomCoroutine = StartCoroutine(ZoomToPlayer());
        }

        isZoomedOnPlayer = !isZoomedOnPlayer;
    }

    private IEnumerator ZoomToPlayer()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = playerTransform.position + playerOffset;
        targetPosition.z = originalCameraPosition.z; // Keep original Z
        
        int startPPU = originalPPU;
        int targetPPU = zoomedPPU;

        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            float t = elapsed / zoomDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Update target position during zoom (in case player moves)
            if (playerTransform != null)
            {
                targetPosition = playerTransform.position + playerOffset;
                targetPosition.z = originalCameraPosition.z;
            }

            // Lerp position
            transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            // Lerp zoom
            if (TryGetComponent<PixelPerfectCamera>(out var cam))
            {
                float currentPPU = Mathf.Lerp(startPPU, targetPPU, smoothT);
                cam.assetsPPU = Mathf.RoundToInt(currentPPU);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values
        if (playerTransform != null)
        {
            targetPosition = playerTransform.position + playerOffset;
            targetPosition.z = originalCameraPosition.z;
            transform.position = targetPosition;
        }
        
        if (TryGetComponent<PixelPerfectCamera>(out var finalCam))
        {
            finalCam.assetsPPU = targetPPU;
        }

        // Start following the player after zoom is complete
        isFollowingPlayer = true;
        currentZoomCoroutine = null;
    }

    private IEnumerator ZoomToOriginal()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = originalCameraPosition;
        
        int startPPU = zoomedPPU;
        int targetPPU = originalPPU;

        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            float t = elapsed / zoomDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Lerp position
            transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            // Lerp zoom
            if (TryGetComponent<PixelPerfectCamera>(out var cam))
            {
                float currentPPU = Mathf.Lerp(startPPU, targetPPU, smoothT);
                cam.assetsPPU = Mathf.RoundToInt(currentPPU);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values
        transform.position = targetPosition;
        if (TryGetComponent<PixelPerfectCamera>(out var finalCam))
        {
            finalCam.assetsPPU = targetPPU;
        }

        currentZoomCoroutine = null;
    }

    public void shakeCameraOnDamage()
    {
        shakeCamera(shakeDuration, shakeMagnitude);
    }

    private void shakeCameraGameOver()
    {
        shakeCamera(1, shakeMagnitude);
    }

    public void shakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    public void lerpCameraWhenPlayerSpawned(GameObject player)
    {
        StartCoroutine(SmoothZoom(24, 48, 1f));
    }

    public IEnumerator SmoothZoom(int from, int to, float duration)
    {
        float elapsed = 0f;
        float start = from;
        float end = to;

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, to);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float current = Mathf.Lerp(start, end, smoothT);

            if (TryGetComponent<PixelPerfectCamera>(out var cam))
            {
                cam.assetsPPU = Mathf.RoundToInt(current);
            }
            else
            {
                transform.position = Vector3.Lerp(originalPosition, targetPosition, smoothT);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (TryGetComponent<PixelPerfectCamera>(out var finalCam))
        {
            finalCam.assetsPPU = to;
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);

            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude * damper;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude * damper;

            Vector3 targetPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 0.5f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
