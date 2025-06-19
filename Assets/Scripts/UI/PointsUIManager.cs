using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsUIManager : MonoBehaviour
{
    [SerializeField] PointsToAddTextUI addPointsUI;
    [SerializeField] PointsTextUI totalPointsUI;
    [SerializeField] SnackStreakUI snackStreakUI;
   

    public static PointsUIManager Instance { get; private set; }


    void Start()
    {
        updatePointsUI("0", 0f);
        updateStreakUI(0);
    }
    public void updatePointsUI(string v, float pointsToAdd)
    {
        totalPointsUI.addPoints(pointsToAdd);
        addPointsUI.addPoints(v);
    }
    public void updateStreakUI(int snackStreak)
    {
        snackStreakUI.playStreakAnimation(snackStreak);
        Debug.Log($"Snack streak updated to: {snackStreak}");
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

 

}
