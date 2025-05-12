using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnackAnimHandler : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Snack snack;

    [SerializeField] float finalScale = 1f; // final object scale when leaving the spiral tray

    [SerializeField] float leaveDuration = 1f; // duration of the scale change when leaving the spiral tray
    private readonly int destroyHash = Animator.StringToHash("Destroy");
    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int fallHash = Animator.StringToHash("Fall");
    private readonly int catchHash = Animator.StringToHash("Catch");
    private readonly int leavingSpiralTrayHash = Animator.StringToHash("LeavingSpiralTray");

    Coroutine scaleRoutine;
    bool readyToFall; // bool to check if the object is ready to fall
    public bool readyToDestroy; // bool to check if the object is ready to be destroyed

    // items in idle has a relucent flash anim.



    public void playDestroyAnimation()
    {

        anim.Play(destroyHash);

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
        readyToFall = true;
    }






}
