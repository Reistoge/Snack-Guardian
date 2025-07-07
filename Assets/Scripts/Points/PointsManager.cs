using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    [Header("Points Configuration")]
    [SerializeField] float points = 0f;
    [SerializeField] float pointsPerSnack = 100f;
    [SerializeField] float bonusPointsPerDash = 50f; // Not used in this script, but can be used for future functionality
    [SerializeField] int snackStreak = 0;
    [SerializeField] float streakMultiplier = 0.1f;

    private static PointsManager instance;

    public static PointsManager Instance { get => instance; set => instance = value; }
    public float Points { get => points; set => points = value; }

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







    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     addPoints(false);
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     addPoints(true);
        // }
    }

    void OnEnable()
    {
        GameEvents.onSnackCaptured += addPoints;
        GameEvents.onSnackLandedOnGround += resetStreak;
        GameEvents.onTraysCleared += addTraysClearedPoints;




    }


    private void addTraysClearedPoints()
    {
        float traysClearedPoints = 10000f; // Points awarded for clearing trays
        Points += traysClearedPoints;
        PointsUIManager.Instance.updatePointsUI("Trays Cleared", traysClearedPoints);
        Debug.Log($"Trays cleared, {traysClearedPoints} points added. Total Points: {Points}");
    }

    private void resetStreak()
    {
        snackStreak = 0;
        Debug.Log("Streak reset to zero.");
        PointsUIManager.Instance.updateStreakUI(snackStreak);

        streakMultiplier = streakMultiplier - 0.5f;
        string a = math.clamp(float.Parse(streakMultiplier.ToString("F2").Substring(0, 4)), 1f, Mathf.Infinity).ToString("F2");
        streakMultiplier = float.Parse(a);



    }


    public void addPoints(bool wasInDash)
    {

        snackStreak++;
        PointsUIManager.Instance.updateStreakUI(snackStreak);
        if (snackStreak % 5 == 0)
        {
            streakMultiplier = streakMultiplier + 0.5f;
            string a = streakMultiplier.ToString("F2").Substring(0, 4);
            streakMultiplier = float.Parse(a);

            Debug.Log($"Streak multiplier increased to: {streakMultiplier}");

        }
        // show streak in ui
        Debug.Log($"Snack streak increased to: {snackStreak}");

        float multiplier = (snackStreak * streakMultiplier);
        // show multiplier in ui

        float pointsToAdd = pointsPerSnack * multiplier;
        if (!wasInDash)
        {
            Points += pointsToAdd;
            PointsUIManager.Instance.updatePointsUI(pointsPerSnack + " x " + snackStreak + " x " + streakMultiplier, pointsToAdd);
        }

        if (wasInDash)
        {
            pointsToAdd += bonusPointsPerDash; // Add bonus points for dashing
            Points += pointsToAdd;// Add bonus points for dashing
            // show bonus points in ui
            Debug.Log("Snack captured in dash, bonus points added.");
            PointsUIManager.Instance.updatePointsUI(pointsPerSnack + " x " + snackStreak + " x " + streakMultiplier + " + " + bonusPointsPerDash, pointsToAdd);

        }

        // show points added in ui
        Debug.Log($"Points added: {pointsToAdd}, Total Points: {Points}, Current Streak: {snackStreak}");


    }
    
}
