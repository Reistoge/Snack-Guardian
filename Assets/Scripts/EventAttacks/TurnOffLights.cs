using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffLights : MonoBehaviour, GameDebuff
{

    [SerializeField] Animator anim;

    private const string ON_ENTER = "onEnter";
    private const string ON_EXIT = "onExit";
    private readonly int onEnterHash = Animator.StringToHash(ON_ENTER);
    private readonly int onExitHash = Animator.StringToHash(ON_EXIT);
    [SerializeField] float onEnterAnimationDelay = 0.5f; // Adjust this value as needed
    [SerializeField] float turnOnLightDelay = 3f; // Adjust this value as needed

    public bool debuffIsActive;
    public bool DebuffIsActive { get => debuffIsActive; set => debuffIsActive = value; }
    public void playTurnOffLightAnimation()
    {
        // Assuming you have an Animator component attached to this GameObject
        GameEvents.triggerOnTurnOffLights();
        UIGameNotifications.instance.playTurnOffLightsNotification();
        Animator globalLightAnimator;
        GameObject.Find("GlobalLight").TryGetComponent<Animator>(out globalLightAnimator);
        if (globalLightAnimator == null)
        {
            Debug.LogError("GlobalLight Animator not found!");
            return;
        }
        else
        {
            globalLightAnimator.runtimeAnimatorController = anim.runtimeAnimatorController;
            globalLightAnimator.Play(onEnterHash, 0);
        }
        // anim.Play(onEnterHash, 0);
    }
    public void playTurnOnLightAnimation()
    {
        // Assuming you have an Animator component attached to this GameObject
        GameEvents.triggerOnTurnOnLights();
        Animator globalLightAnimator;
        GameObject.Find("GlobalLight").TryGetComponent<Animator>(out globalLightAnimator);
        if (globalLightAnimator == null)
        {
            Debug.LogError("GlobalLight Animator not found!");
            return;
        }
        else
        {
            globalLightAnimator.runtimeAnimatorController = anim.runtimeAnimatorController;
            globalLightAnimator.Play(onExitHash, 0);
        }
        // anim.Play(onEnterHash, 0);
    }

    IEnumerator playturnOffLightAnimationDelay()
    {
        debuffIsActive = true;
        yield return new WaitForSeconds(onEnterAnimationDelay);
        playTurnOffLightAnimation();
        yield return new WaitForSeconds(turnOnLightDelay);
        playTurnOnLightAnimation();
        debuffIsActive = false;


    }

    public GameDebuff applyDebuff()
    {
        StartCoroutine(playturnOffLightAnimationDelay());
        return this;
    }
    
    public string getDebuffName()
    {
        return "turnOffLights";
    }
    
}
