using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GameSceneCamera : MonoBehaviour
{
    
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;



    void OnEnable()
    {
        GameEvents.onPlayerDamaged += shakeCameraOnDamage;
        GameEvents.onGameOver += shakeCameraGameOver;
        //GameEvents.onPlayerSpawned += lerpCameraWhenPlayerSpawned;
    }

    private void shakeCameraGameOver()
    {
       shakeCamera(1, shakeMagnitude);
    }

    void OnDisable()
    {
        GameEvents.onPlayerDamaged -= shakeCameraOnDamage;
        GameEvents.onGameOver -= shakeCameraGameOver;
       // GameEvents.onPlayerSpawned += lerpCameraWhenPlayerSpawned;

    }
    public void shakeCameraOnDamage()
    {

        shakeCamera(shakeDuration, shakeMagnitude);
    }

    public void shakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    
    public void lerpCameraWhenPlayerSpawned(GameObject player)
    {
        StartCoroutine(SmoothZoom(24, 48, 1f));
    }

    // Smoothly zooms the PixelPerfectCamera's assetsPPU value or camera's z position if not found
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
            float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f); // Smooth in/out

            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude * damper;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude * damper;

            Vector3 targetPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 0.5f); // Smooth transition

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition; // Reset to original position
    }
}
