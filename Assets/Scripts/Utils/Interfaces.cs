using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void onInteract(IInteractor interactor);
}

public interface IInteractor
{
    void applyEffect(ObjectEffect effect);
}