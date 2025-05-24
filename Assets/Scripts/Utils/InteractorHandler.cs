using System;
using UnityEngine;
public static class InteractorHandler
{

    public static void handleInteraction(IInteractable interactable, IInteractor interactor, ObjectEffect effect)
    {
        Debug.Log($"Handling interaction between {interactable.GetType().Name} and {interactor.GetType().Name}");
        if (interactable is Snack && interactor is PlayerInteract)
        {
            //Snack apply object effect to the player.
            PlayerInteract playerInteractor = interactor as PlayerInteract;
            playerInteractor.interactionWithSnack(effect);
            // playerInteractor.applyEffect(effect);
            //Debug.Log($"Snack effect applied to player: {effect.type}");
            


        }

    }
}