using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITurnOffLights : MonoBehaviour
{
    [SerializeField] private WarningNotification warningNotificationPrefab;
    WarningNotification warningNotificationInstance;
    [SerializeField] string warningMessage = "Lights off!";

    private void Start()
    {
        warningNotificationInstance = Instantiate(warningNotificationPrefab, transform);
    }
 
    public void playUIWarning()
    {
    
        warningNotificationInstance.setMessage(warningMessage);
        warningNotificationInstance.showNotification();
    }
}