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
        MultiplayerGameEvents.onPlayerReceiveAttack += handleReceiveAttack;
    }
    void OnDisable()
    {
        GameEvents.onSnackCaptured -= fillWhenSnackCaptured;
        MultiplayerGameEvents.onPlayerReceiveAttack -= handleReceiveAttack;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
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
            sendAttack(attack1Cost, "Attack1");
        }
        else if (fillLevel >= attack2Cost && fillLevel < attack3Cost)
        {
            sendAttack(attack2Cost, "Attack2");
        }
        else if (fillLevel >= attack3Cost)
        {
            sendAttack(attack3Cost, "Attack3");
        }
        
        
        


    }
    // public enum AttackType
    // {
    //   Attack1,
    //   Attack2,
    //   Attack3
    //}

    private void sendAttack(int attackCost, string attackType)
    {
        fillLevel -= attackCost;
        fillToLevel(fillLevel);
        print("Sending attack with fill amount: " + fillLevel);
        MultiplayerGameEvents.triggerPlayerSendAttack(attackType);
        switch (attackType)
        {
            case "Attack1":
                // Implement attack 1 logic here
                Debug.Log("Attack 1 sent!");
                 
                
                break;
            case "Attack2":
                // Implement attack 2 logic here
                Debug.Log("Attack 2 sent!");
                break;
            case "Attack3":
                // Implement attack 3 logic here
                Debug.Log("Attack 3 sent!");
                break;
        }

        
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

    private void handleReceiveAttack(string attackType)
    {
        Debug.Log($"📥 Recibido ataque: {attackType}");
        switch (attackType)
        {
            case "Attack1":
                // Ejemplo: Reducir puntos o aplicar penalización
                fillDown(); // baja un nivel
                Debug.Log("Ataque 1 recibido");
                break;
            case "Attack2":
                fillDown();
                fillDown();
                Debug.Log("Ataque 2 recibido");
                break;
            case "Attack3":
                empty();
                Debug.Log("Ataque 3 recibido");
                break;
            default:
                Debug.LogWarning("Ataque desconocido recibido");
                break;
        }
    }
}