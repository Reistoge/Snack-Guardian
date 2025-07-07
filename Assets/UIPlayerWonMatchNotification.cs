using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerWonMatchNotification : MonoBehaviour
{
    [SerializeField] CelebrationNotification celebrationNotification;
    void OnEnable()
    {
        MultiplayerGameEvents.onOpponentLost += celebrationNotification.showNotification;
    }
    void OnDisable()
    {
        MultiplayerGameEvents.onOpponentLost -= celebrationNotification.showNotification;
    }
}
