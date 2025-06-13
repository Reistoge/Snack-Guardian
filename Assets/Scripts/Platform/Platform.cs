using System;
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Platform : MonoBehaviour
{
    [SerializeField] PlatformAnimHandler platformAnimHandler;
    [SerializeField] BoxCollider2D boxCollider;
    public event Action onPlatformLanded;
    [SerializeField] TrayId trayId;
    [SerializeField] SpriteRenderer idSpriteRenderer;
    [SerializeField] Light2D light2D;
    void OnEnable()
    {
        GameEvents.onGameOver += turnOffLight;
        GameEvents.turnOnLight += turnOnLight;
        GameEvents.turnOffLight += turnOffLight;
    }
    void OnDisable()
    {
        GameEvents.onGameOver -= turnOffLight;
        GameEvents.turnOnLight -= turnOnLight;
        GameEvents.turnOffLight -= turnOffLight;
    }

    // Start is called before the first frame update
    public void loadTrayIdSprite()
    {
        if (trayId != null)
        {
            idSpriteRenderer.sprite = trayId.idImage;
        }
    }
    public void setTrayIdSO(TrayId trayId)
    {
        this.trayId = trayId;
        //loadTrayIdSprite();
    }
    public PlatformAnimHandler getPlatformAnimHandler()
    {
        return platformAnimHandler;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        onPlatformLanded?.Invoke();
        platformAnimHandler.playOnLandedObjectAnimation();
    }
    public void turnOffLight()
    {
        if (light2D != null)
        {
            light2D.enabled = false;
        }
    }
    public void turnOnLight()
    {
        if (light2D != null)
        {
            light2D.enabled = true;
        }
    }
    public void destroyPlatform()
    {

        // Disable the collider
        boxCollider.enabled = false;
        platformAnimHandler.playBrokeAnimation();
        idSpriteRenderer.enabled = false; // Hide the tray ID sprite

        // Play the broke animation

    }
    public void repairPlatform()
    {
        // Enable the collider
        boxCollider.enabled = true;
        // Play the idle animation
        platformAnimHandler.playRepairAnimation();
        idSpriteRenderer.enabled = true; // Hide the tray ID sprite

    }
    // IEnumerator waitForAnimationToFinish(string animationName, Action postAnimationAction)
    // {
    //     // Wait for the animation to finish
    //     yield return new WaitForSeconds(platformAnimHandler.getClipDuration(animationName));
    //     postAnimationAction?.Invoke();
    // }
    // IEnumerator executeInAnimation(string animationName,float timingScale, Action postAnimationAction)
    // {
    //     timingScale = Mathf.Clamp(timingScale, 0.01f, 1f);
    //     // Wait for the animation to finish
    //     yield return new WaitForSeconds(platformAnimHandler.getClipDuration(animationName) / timingScale);
    //     postAnimationAction?.Invoke();
    // }




}
