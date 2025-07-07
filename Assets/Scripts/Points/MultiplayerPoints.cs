using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerPoints : MonoBehaviour
{
    [SerializeField] private UIMultiplayerPointsBar pointsBar;
    [SerializeField] float maxPoints = 100f;
    [SerializeField] float pointsPerSnack = 10f;
    [SerializeField] float pointsDiscountPerSecond = 1f; // put the discount in seconds in Update.
    [SerializeField] float numberOfPartitions = 9;
    [SerializeField] int fillLevel = 0;
    [SerializeField] int attack1Cost = 3;
    [SerializeField] int attack2Cost = 6;
    [SerializeField] int attack3Cost = 9;


    void OnEnable()
    {
        GameEvents.onSnackCaptured += fillWhenSnackCaptured;
    }
    void OnDisable()
    {
        GameEvents.onSnackCaptured -= fillWhenSnackCaptured;
    }
    void Update()
    {

        if (InputManager.multiplayerAttackPressed)
        {
            trySendAttack();

        }

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     fillUp();
        // }
    }

    private void trySendAttack()
    {
        if (fillLevel < attack1Cost)
        {
            Debug.Log("Not enough points to send attack.");
            return;
        }
        if (fillLevel >= attack1Cost && fillLevel < attack2Cost)
        {
            sendAttack(attack1Cost, AttackType.weak);
        }
        else if (fillLevel >= attack2Cost && fillLevel < attack3Cost)
        {
            sendAttack(attack2Cost, AttackType.medium);
        }
        else if (fillLevel >= attack3Cost)
        {
            sendAttack(attack3Cost, AttackType.strong);

        }
        
        
        


    }

    // private void sendAttack(int attackCost)
    // {
    //     fillLevel -= attackCost;
    //     fillToLevel(fillLevel);
        
    //     MultiplayerGameEvents.triggerPlayerSendAttack(attackType);
    //     print("Sending attack with fill amount: " + fillLevel);

        
    // }
    private void sendAttack(int attackCost, AttackType attackType)
    {
        fillLevel -= attackCost;
        fillToLevel(fillLevel);
        
        MultiplayerGameEvents.triggerPlayerSendAttack(attackType);
        print("Sending attack with fill amount: " + fillLevel +" and attack type: " + attackType.ToString());

        
    }

    void Start()
    {
        fillPointsBar(0, maxPoints);
    }

    private void discountPointsPerSecond()
    {
        if (pointsBar != null && pointsBar.FillImage.fillAmount >= 0)
        {
            fillPointsBar(pointsBar.FillImage.fillAmount * maxPoints - pointsDiscountPerSecond * Time.deltaTime, maxPoints);
        }
    }

    public void fillWhenSnackCaptured(bool wasInDash)
    {
        if (wasInDash)
        {
            fillUp();
            fillUp();
        }
        else
        {
            fillUp();
           
        }

    }
    public enum AttackType
    {
        weak,
        medium,
        strong
    }
    public void fillPointsBar(float current, float max)
    {

        if (pointsBar != null)
        {
            fillLevel = Mathf.Clamp(Mathf.RoundToInt(current / (max / numberOfPartitions)), 0, (int)numberOfPartitions);
            pointsBar.updateMultiplayerBarPoints(current, max);
        }
    }
    public void fillToLevel(int level)
    {
        if (level < 0 || level > numberOfPartitions)
        {
            Debug.LogError("Invalid fill level: " + level);
            return;
        }
        fillLevel = level;
        float current = fillLevel * (maxPoints / numberOfPartitions);
        fillPointsBar(current, maxPoints);
    }
    public void fillDown()
    {
        fillLevel--;
        float current = fillLevel * (maxPoints / numberOfPartitions);
        fillPointsBar(current, maxPoints);
    }
    public void fillUp()
    {
        fillLevel++;
        float current = fillLevel * (maxPoints / numberOfPartitions);
        fillPointsBar(current, maxPoints);
    }
    public void empty()
    {
        fillLevel = 0;
        fillPointsBar(0, maxPoints);
    }
 
}
