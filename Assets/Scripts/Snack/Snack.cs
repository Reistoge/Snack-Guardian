using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Snack : MonoBehaviour, IInteractable, IInteractor
{
    [SerializeField] SnackConfig snackConfig;
    [SerializeField] Fall fall;
    [SerializeField] SnackAnimHandler animatorHandler;
    [SerializeField] SnackInteract catchDetector;
    [SerializeField] bool landed = false;

    ObjectEffect snackEffect;
    Coroutine leavingSpiralTrayRoutine;
    [SerializeField] float initScale = 0.5f;
    [SerializeField] float leaveDuration = 1f;
    GameObject particles;

    public bool Landed { get => landed; set => landed = value; }

    public float getInitScale()
    {
        return initScale;
    }
    public void setInitScale(float scale)
    {
        initScale = scale;
        animatorHandler.setScale(initScale);
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
        fall.getOnObjectLanded().AddListener(() => snackLandedOnGround()); // it would be better with actions.

    }



    void Start()
    {
        Landed = false;
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


        //fall.setActiveBody(true);
        // fall.setActiveFeet(true);
    }

    public void destroySnack()
    {
        destructionParticles();
        stopLeavingSpiralTrayCorutine();
        StartCoroutine(destroyObjectCorutine());
    }

    private void destructionParticles()
    {
        particles = Instantiate(snackConfig.destroyParticlesPrefab, transform.position, Quaternion.identity);
    }
    public void destroyParticles()
    {
        Destroy(particles);
    }

    IEnumerator startLeavingSpiralTrayCorutine()
    {
        animatorHandler.playLeavingSpiralTrayAnimation();
        yield return new WaitUntil(() => animatorHandler.isReadyToFall());

        startFalling();
        leavingSpiralTrayRoutine = null;

    }

    IEnumerator destroyObjectCorutine()
    {

        animatorHandler.playDestroyAnimation();
        yield return new WaitUntil(() => animatorHandler.getReadyToDestroy());
        destroyParticles();
        Destroy(gameObject);
    }

    // new 
    public void onInteract(IInteractor interactor)
    {
        // what happens to the snack when it interacts with the object ?
        // what happen when we have to pass a effect to a object
        fall.stopFall();
        // here we pass the effect to the a object that implements the IInteractor interface

        // if player is not dashing -> make damage
        // if player is dashing -> make apply second effect
        interactor.applyEffect(snackEffect);

        InteractorHandler.handleInteraction(this as IInteractable, interactor, snackEffect);
        // GameManager.Instance.handleInteraction(this as IInteractable, interactor);
        destroySnack();




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
        // print(initScale);
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
    void snackLandedOnGround()
    {
        //animatorHandler.playTouchGroundAnimation();
        // catchDetector.desactivateCollider();

        if (snackConfig.isRock)
        {
            //prevent the player to be damaged when the rock is already destroyed.
            canBeCatched(false);
        }
        if (snackConfig.destroyOnFall || snackConfig.isRock)
        {
            destroySnack();
            return;
        }
        
        canBeCatched(false);
        int direction = Random.Range(0, 2) == 0 ? 1 : -1;
        float randomOffset = Random.Range(0, 0.15f);
        transform.position = new Vector3(transform.position.x + (randomOffset * direction), transform.position.y, 0);

        //destroySnack();
        if (Random.Range(0, 2) == 1)
        {
            //destroy drop particles.
        }
        else
        {

            // do not destroy just stay in ground.
        }
        GameEvents.triggerSnackLandedOnGround();
        Landed = true;

    }
    public void canBeCatched(bool canBeCatched)
    {
        if (canBeCatched)
        {
            catchDetector.activateCollider();
        }
        else
        {
            catchDetector.desactivateCollider();
        }
    }


    public bool isReadyToFall()
    {
        return leavingSpiralTrayRoutine == null;
    }

    public SnackConfig getConfig()
    {
        return snackConfig;
    }
}
