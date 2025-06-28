using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartingOrder : MonoBehaviour
{
    [SerializeField] CelebrationNotification simpleCelebrationNotificationPrefab;
    CelebrationNotification simpleCelebrationNotificationInstance;
    string celebrationMessage="";
    public void Start()
    {
        simpleCelebrationNotificationInstance = Instantiate(simpleCelebrationNotificationPrefab, transform);
    }
 
    public void playUIStartingOrder()
    {
        celebrationMessage = $"Starting order {VendingLogic.instance.OrdersCount}";
        simpleCelebrationNotificationInstance.setMessage(celebrationMessage);
        simpleCelebrationNotificationInstance.showNotification();


    }
}
