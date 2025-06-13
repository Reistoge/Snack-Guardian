using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameNotifications : MonoBehaviour
{
    public static UIGameNotifications instance;
    [SerializeField] UIRocksComing rocksComingNotification;
    [SerializeField] UIOrderCompleted orderCompletedNotification;
    [SerializeField] UIStartingOrder startingOrderNotification;
    [SerializeField] UITurnOffLights turnOffLightsNotification;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void playRocksComingNotification()
    {
        if (rocksComingNotification != null)
        {
            rocksComingNotification.playUIWarning();
        }
        else
        {
            Debug.LogWarning("Rocks Coming Notification is not set.");
        }
    }
    public void playOrderCompletedNotification()
    {
        if (orderCompletedNotification != null)
        {
            orderCompletedNotification.playUICelebration();
        }
        else
        {
            Debug.LogWarning("Order Completed Notification is not set.");
        }
    }
    public void playStartingOrderNotification()
    {
        if (startingOrderNotification != null)
        {
            startingOrderNotification.playUIStartingOrder();
        }
        else
        {
            Debug.LogWarning("Starting Order Notification is not set.");
        }
    }

    public void playTurnOffLightsNotification()
    {
        if (turnOffLightsNotification != null)
        {
            turnOffLightsNotification.playUIWarning();
        }
        else
        {
            Debug.LogWarning("Starting Order Notification is not set.");
        }
    }
}
