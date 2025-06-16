using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MainMenuGlobalLight : MonoBehaviour
{
    [SerializeField] Animator animator;
    readonly int GLOBAL_LIGHT_ON_HASH = Animator.StringToHash("onEnter");

    void OnEnable()
    {
        
        MainMenuCigarette.onCigarette += activateGlobalLight;
    }

    private void activateGlobalLight()
    {
        animator.Play(GLOBAL_LIGHT_ON_HASH, -1, 0f);
    }

    void OnDisable()
    {
       MainMenuCigarette.onCigarette -= activateGlobalLight;
    }
}
