using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snack : MonoBehaviour, IInteractable,IInteractor
{
    [SerializeField] SnackConfig snackConfig;
    [SerializeField] Fall fall;
    [SerializeField] SnackAnimHandler animatorHandler;
    [SerializeField] SnackInteract catchDetector;
    ObjectEffect snackEffect;
    Coroutine leavingSpiralTrayRoutine;
    [SerializeField] float initScale = 0.5f;
    [SerializeField] float leaveDuration = 1f;
    

    
    public float getInitScale()
    {
        return initScale;
    }
    public void setInitScale(float scale)
    {
        initScale = scale;
    }
    public float getLeaveDuration()
    {
        return leaveDuration;
    }
    public void setLeaveDuration(float duration)
    {
        leaveDuration = duration;
    }
    void OnEnable()
    {
        
        loadConfig();
        fall.getOnObjectLanded().AddListener(() => animatorHandler.playIdleAnimation());
         
    }

   
 
    void Start()
    {
          
        fall.stopFall();
        animatorHandler.playIdleAnimation();
        //catchDetector.desactivateCollider();
        //  fall.startFall();   
    }
    public void startLeavingSpiralTray()
    {
        if (leavingSpiralTrayRoutine == null)
        {
            leavingSpiralTrayRoutine = StartCoroutine(startLeavingSpiralTrayCorutine());
        }
    }
    public void stopLeavingSpiralTrayCorutine()
    {

        if (leavingSpiralTrayRoutine != null)
        {
            StopCoroutine(leavingSpiralTrayRoutine);
            animatorHandler.stopLeavingSpiralTrayAnimation();
            leavingSpiralTrayRoutine = null;
        }

    }
    public void startFalling()
    {
        fall.startFall();
        animatorHandler.playFallAnimation();
        catchDetector.activateCollider();
    }
 
    public void destroySnack()
    {
        stopLeavingSpiralTrayCorutine();
        StartCoroutine(destroyObjectCorutine());
    }


    IEnumerator startLeavingSpiralTrayCorutine()
    {
        animatorHandler.playLeavingSpiralTrayAnimation();
        yield return new WaitUntil(() => animatorHandler.isReadyToFall());
        startFalling();

    }
    IEnumerator destroyObjectCorutine()
    {
        animatorHandler.playDestroyAnimation();
        yield return new WaitUntil(() => animatorHandler.getReadyToDestroy());
        Destroy(gameObject);
    }

    // new 
    public void onInteract(IInteractor interactor)
    {
        // what happens to the snack when it interacts with the object ?
        // what happen when we have to pass a effect to a object
        fall.stopFall();
        destroySnack();
        // here we pass the effect to the a object that implements the IInteractor interface
        interactor.applyEffect(snackEffect);


    }

    public void applyEffect(ObjectEffect effect)
    {
        // what happens or what we can do with this effects in this object ?.
         // here we can apply the effect a effect to the snack
    }
    void loadConfig()
    {
        snackEffect = snackConfig.objectEffect;
        fall.setFallStats(snackConfig.fallStats);
        animatorHandler.setAnimator(snackConfig.animator);
        animatorHandler.setScale(initScale);
        animatorHandler.setLeaveDuration(leaveDuration);
        // animatorHandler.setSprite(snackConfig.spriteRenderer);
    }
    public void setConfig(SnackConfig config)
    {
        snackConfig = config;
         
        // animatorHandler.setSprite(snackConfig.spriteRenderer);
    }

    internal void setOrderInLayer(int count)
    {
        animatorHandler.setOrderInLayer(count);
    }
}
