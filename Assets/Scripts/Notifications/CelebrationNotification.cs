using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CelebrationNotification : MonoBehaviour
{

    [SerializeField] Animator anim;
    [SerializeField] string message;
    [SerializeField] TMPro.TextMeshProUGUI text;
    public static readonly string ON_ENTER_ANIMATION_STATE = "onEnter";
    public static readonly int ON_ENTER_ANIMATION_HASH = Animator.StringToHash(ON_ENTER_ANIMATION_STATE);

    public void showNotification()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(ON_ENTER_ANIMATION_STATE))
        {
            // If already playing the animation, do not restart it
            return;
        }
        text.text = message;
        anim.Play(ON_ENTER_ANIMATION_HASH);
    }

    public void setMessage(string message)
    {
        this.message = message;
    }
}
