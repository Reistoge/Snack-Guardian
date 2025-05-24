using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using Random = UnityEngine.Random;

public class SnackAnimHandler : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Snack snack;
    [SerializeField] SpriteRenderer spriteRenderer;


    float finalScale = 1f; // final object scale when leaving the spiral tray


    [SerializeField] float leaveDuration = 1f; // duration of the scale change when leaving the spiral tray

    public static readonly string IDLE_ANIMATION_STATE = "Idle";
    public static readonly string FALL_ANIMATION_STATE = "Fall";
    public static readonly string CATCH_ANIMATION_STATE = "Catch";
    public static readonly string DESTROY_ANIMATION_STATE = "Destroy";
    public static readonly string LEAVING_SPIRAL_TRAY_ANIMATION_STATE = "LeavingSpiralTray";
    public static readonly string FLIP_ANIMATION_STATE = "Flip";

    private readonly int destroyHash = Animator.StringToHash(DESTROY_ANIMATION_STATE);
    private readonly int idleHash = Animator.StringToHash(IDLE_ANIMATION_STATE);
    private readonly int fallHash = Animator.StringToHash(FALL_ANIMATION_STATE);
    private readonly int catchHash = Animator.StringToHash(CATCH_ANIMATION_STATE);
    private readonly int leavingSpiralTrayHash = Animator.StringToHash(LEAVING_SPIRAL_TRAY_ANIMATION_STATE);
    private readonly int flipHash = Animator.StringToHash(FLIP_ANIMATION_STATE);
    // private readonly int touchGroundRightHash = Animator.StringToHash("TouchGroundRight");
    // private readonly int touchGroundLeftHash = Animator.StringToHash("TouchGroundLeft");
    Coroutine scaleRoutine;
    bool readyToFall; // bool to check if the object is ready to fall
    public bool readyToDestroy; // bool to check if the object is ready to be destroyed

    // items in idle has a relucent flash anim.



    public void setLeaveDuration(float duration)
    {
        leaveDuration = duration;
    }
    public void setScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
    public void playDestroyAnimation()
    {

        anim.Play(destroyHash);

    }
    public void playFlipAnimation()
    {
        anim.Play(flipHash);
    }
    public void isReadyToDestroy()
    {
        readyToDestroy = true;
    }
    public bool getReadyToDestroy()
    {
        return readyToDestroy;
    }
    public void playIdleAnimation()
    {

        anim.Play(idleHash);
    }
    public void playFallAnimation()
    {

        anim.Play(fallHash);
    }
    public void playCatchAnimation()
    {

        anim.Play(catchHash);
    }
    public void playCatchAnimation(float delay)
    {

        anim.Play(catchHash, 0, delay);
    }

    public void playLeavingSpiralTrayAnimation()
    {
        if (scaleRoutine == null)
        {
            scaleRoutine = StartCoroutine(changeScale(finalScale, leaveDuration));
            anim.Play(leavingSpiralTrayHash);
        }
    }

 
    // public void playTouchGroundAnimation()
    // {
    //     int randomSign = Random.value < 0.5f ? 1 : -1;
    //     if(randomSign == 1)
    //     {
    //         anim.Play(touchGroundRightHash);
    //     }
    //     else
    //     {
    //         anim.Play(touchGroundLeftHash);
    //     }




    // }
    // public float getTouchGroundAnimationDuration()
    // {
    //     foreach (var clip in anim.runtimeAnimatorController.animationClips)
    //     {
    //         if (clip.name =="TouchGround") // Replace with your damaged animation's name
    //             return clip.length;
    //     }
    //     return 0f; // Or throw an exception if not found
    // }
    public void stopLeavingSpiralTrayAnimation()
    {

        // to cancel the changeScale()
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
            scaleRoutine = null;
        }
        readyToFall = false;
    }
    public bool isReadyToFall()
    {
        return readyToFall;
    }

    IEnumerator changeScale(float targetScale, float duration)
    {

        // change the scale when the object is leaving the spiral tray.
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(targetScale, targetScale, targetScale);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Smooth out the interpolation
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }


        transform.localScale = endScale;
        playFlipAnimation();
        snack.canBeCatched(true);
        yield return new WaitForSeconds(getClipDuration(FLIP_ANIMATION_STATE));
        readyToFall = true;
    }

    internal void setAnimator(RuntimeAnimatorController animController)
    {
        anim.runtimeAnimatorController = animController;

    }

    internal void setOrderInLayer(int count)
    {
        spriteRenderer.sortingOrder = count;
    }
    public float getClipDuration(string clipName)
    {
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) // Replace with your damaged animation's name
                return clip.length; 
        }
        return 0f; // Or throw an exception if not found
    }
}
