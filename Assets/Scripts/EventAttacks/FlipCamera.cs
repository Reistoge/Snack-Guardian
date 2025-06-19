using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCamera : MonoBehaviour, GameDebuff
{
    [SerializeField] RuntimeAnimatorController animatorController;
    public static readonly string ON_ENTER = "onEnter";
    public static readonly string ON_EXIT = "onExit";
    private static readonly int ON_ENTER_HASH = Animator.StringToHash(ON_ENTER);
    private static readonly int ON_EXIT_HASH = Animator.StringToHash(ON_EXIT);
    Animator cameraAnim;
   
    public bool debuffIsActive;
    public bool DebuffIsActive { get =>debuffIsActive; set => debuffIsActive=value; }

    public GameDebuff applyDebuff()
    {

        StartCoroutine(applyDebuffCoroutine());
        return this;
      
    }
    IEnumerator applyDebuffCoroutine()
    {
        debuffIsActive = true;
        Camera.main.gameObject.AddComponent<Animator>();

        Camera.main.GetComponent<Animator>().runtimeAnimatorController = animatorController;
        cameraAnim = Camera.main.GetComponent<Animator>();
        cameraAnim.Play(ON_ENTER_HASH);
        yield return new WaitForSeconds(3f);
        cameraAnim.Play(ON_EXIT_HASH);
        yield return new WaitForSeconds(5f);
        Destroy(Camera.main.GetComponent<Animator>());
        debuffIsActive = false;
        
    }

    // Start is called before the first frame update 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            applyDebuff();
        }
    }

    public string getDebuffName()
    {
        return "Flip Camera";
    }
}
