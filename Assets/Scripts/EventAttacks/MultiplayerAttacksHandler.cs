using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MultiplayerAttacksHandler : MonoBehaviour
{
    void OnEnable()
    {
        MultiplayerGameEvents.onPlayerSendAttack += handlePlayerSendAttack;
        MultiplayerGameEvents.onPlayerReceiveAttack += handlePlayerReceiveAttack;
    }

    private void handlePlayerReceiveAttack(string obj)
    {
        if (string.IsNullOrEmpty(obj))
        {
            Debug.LogWarning("Received empty attack data");
            return;
        }
        Debug.Log($"Received attack data: {obj}");
        if (System.Enum.TryParse(obj, out MultiplayerPoints.AttackType attackType))
        {
            EventsAttacksHandler eventsInstance = EventsAttacksHandler.Instance;
            if (eventsInstance)
            {
                    switch (attackType)
                    {
                        case MultiplayerPoints.AttackType.weak:
                        eventsInstance.applyWeakDebuff();
                            break;
                        case MultiplayerPoints.AttackType.medium:
                        eventsInstance.applyMediumDebuff();
                            break;
                        case MultiplayerPoints.AttackType.strong:
                        eventsInstance.applyStrongDebuff();
                            break;
                    }
            }

        }
        else
        {
            Debug.LogWarning($"Failed to parse attack type from string: {obj}");
        }





    }

    private void handlePlayerSendAttack(MultiplayerPoints.AttackType attack)
    {
        NetworkManager.Instance.sendAttackToServer(attack);
    }

}
