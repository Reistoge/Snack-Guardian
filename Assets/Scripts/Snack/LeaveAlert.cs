using UnityEngine;


public class LeaveAlert : MonoBehaviour
{
    public static readonly string ON_ENTER_ANIMATION_STATE = "onEnter";
    public static readonly string ON_EXIT_ANIMATION_STATE = "onExit";


    public readonly int ON_ENTER_ANIMATION_HASH = Animator.StringToHash(ON_ENTER_ANIMATION_STATE);
    public readonly int ON_EXIT_ANIMATION_HASH = Animator.StringToHash(ON_EXIT_ANIMATION_STATE);



    [SerializeField] Animator anim;

    public void playAlert()
    {
        anim.Play(ON_ENTER_ANIMATION_HASH);

    }
    public void stopAlert()
    {
        anim.Play(ON_EXIT_ANIMATION_HASH);
    }



}