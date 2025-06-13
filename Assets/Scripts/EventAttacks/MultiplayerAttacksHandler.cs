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
        obj = obj.Trim();
        Debug.Log($"Received attack data: {obj}");

        switch (obj)
        {
            case "1":
                Debug.Log("Player 1 attack received");
                // Handle player 1 attack logic here
                break;
            case "2":
                Debug.Log("Player 2 attack received");
                // Handle player 2 attack logic here
                break;
            case "3":
                Debug.Log("Player 3 attack received");
                // Handle player 3 attack logic here
                break;
        }


    }
 

    private void handlePlayerSendAttack(string obj)
    {

    }
    
}
