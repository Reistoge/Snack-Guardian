using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOrderCompleted : MonoBehaviour
{
    [SerializeField] CelebrationNotification celebrationNotificationPrefab;
    CelebrationNotification celebrationNotificationInstance;
    string celebrationMessage="";
    public void Start()
    {
        celebrationNotificationInstance = Instantiate(celebrationNotificationPrefab, transform);
    }
 
    
    public void playUICelebration()
    {
        celebrationMessage = $"All Orders Completed!";
        celebrationNotificationInstance.setMessage(celebrationMessage);
        celebrationNotificationInstance.showNotification();


    }
}
