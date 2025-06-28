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

    void OnDisable()
    {
        MultiplayerGameEvents.onPlayerSendAttack -= handlePlayerSendAttack;
        MultiplayerGameEvents.onPlayerReceiveAttack -= handlePlayerReceiveAttack;
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
                // You can call a method or trigger an event for player 1's attack
                break;
            case "2":
                Debug.Log("Player 2 attack received");
                // Handle player 2 attack logic here
                // You can call a method or trigger an event for player 2's attack
                break;
            case "3":
                Debug.Log("Player 3 attack received");
                // Handle player 3 attack logic here
                // You can call a method or trigger an event for player 3's attack
                break;
        }


    }

 

    private void handlePlayerSendAttack(string obj)
    {
        if (string.IsNullOrEmpty(obj))
        {
            Debug.LogWarning("Sent empty attack data");
            return;
        }
        Debug.Log($"Sending attack data: {obj}");
        NetworkManager.Instance.sendAttack(obj);
    }
}
