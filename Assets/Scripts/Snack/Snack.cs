using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snack : MonoBehaviour, IInteractable,IInteractor
{
    [SerializeField] Fall fall;
    [SerializeField] ObjectEffect snackEffect;
    [SerializeField] SnackAnimHandler animatorHandler;
    [SerializeField] SnackInteract catchDetector;
    Coroutine leavingSpiralTrayRoutine;



    void Update()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            startLeavingSpiralTray();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            stopLeavingSpiralTrayCorutine();
        }
    }
    void Awake()
    {
        fall.stopFall();

    }
    void Start()
    {
        animatorHandler.playIdleAnimation();
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
    }
    public void isTouchedByPlayer()
    {
        fall.stopFall();
        destroySnack();

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
}
