using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnackInteract : MonoBehaviour, IInteractor
{
    [SerializeField] Snack snack;

    [SerializeField] Collider2D snackDetectorCollider;





    void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.CompareTag("Player"))
        // {
        //     if (snack != null)
        //     {
        //         snackDetectorCollider.enabled = false;

        //         snack.isTouchedByPlayer();
        //     }
        // }

        var interactor = collision.GetComponent<IInteractor>();
        if (interactor != null && snack != null)
        {
            // invertir logica
            snackDetectorCollider.enabled = false;
            snack.onInteract(interactor);
            
             
             

        }
    }
 
    //template, dont change this,
    //this method pass the effect to the main player script.
    public void applyEffect(ObjectEffect effect)
    {
        // here we handle the effects that are applied to the snack
        if (snack != null)
        {
            snack.applyEffect(effect);
        }
    }
    public void desactivateCollider()
    {
        snackDetectorCollider.enabled = false;
    }
    public void activateCollider()
    {
        snackDetectorCollider.enabled = true;
    }


}
