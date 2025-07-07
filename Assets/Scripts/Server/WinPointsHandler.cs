

using System;
using UnityEngine;

public class WinPointsHandler : MonoBehaviour
{
    [SerializeField] int amount = 10; // win is equivalent to captured 10 snacks 
    void OnEnable()
    {
        MultiplayerGameEvents.onOpponentLost += addWinPoints;
    }

    private void addWinPoints()
    {
        for (int i = 0; i < amount; i++)
        {
            PointsManager.Instance.addPoints(true);
        }
    }

    void OnDisable()
    {
        MultiplayerGameEvents.onOpponentLost -= addWinPoints;
    }



}