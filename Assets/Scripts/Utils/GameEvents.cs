using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action onGameSceneEnded;


    // Health events
    public static event Action<float, float> onHealthChanged; // current, max
    public static event Action onGameOver;
    public static event Action onPlayerDamaged;
    public static event Action onInvincibilityStart;
    public static event Action onInvincibilityEnd;
    public static event Action onConnectedToServer;
    public static event Action onSnackIsRequested;
    public static event Action<bool> onSnackCaptured;
    public static event Action onSnackLandedOnGround;
    public static event Action onRocksAdded;
    public static event Action multiplierPointsIncreased;
    public static event Action onTraysCleared;

    public static event Action turnOnLight;
    public static event Action turnOffLight;


    // Scene/Player events
    public static event Action<GameObject> onPlayerSpawned;

 
    public static event Action onGameDebuffIsActive;
    public static event Action onGameDebuffEnded;
    

    public static void triggerHealthChanged(float current, float max)
    {
        onHealthChanged?.Invoke(current, max);
    }
    public static void triggerSnackCaptured(bool wasInDash)
    {
        onSnackCaptured?.Invoke(wasInDash);
    }
    public static void triggerConnectedToServer()
    {
        onConnectedToServer?.Invoke();
    }
    public static void triggerRocksAdded()
    {
        onRocksAdded?.Invoke();
    }
    public static void triggerGameDebuffIsActive()
    {
      
        onGameDebuffIsActive?.Invoke();
        
    }
    public static void triggerGameDebuffEnded()
    {
        onGameDebuffEnded?.Invoke();
       
    }


    public static void triggerOnSnackIsRequested()
    {
        onSnackIsRequested?.Invoke();
    }

    public static void triggerGameOver()
    {
        onGameOver?.Invoke();
    }

    public static void triggerPlayerSpawned(GameObject player)
    {
        onPlayerSpawned?.Invoke(player);
    }
    public static void triggerInvincibilityStart()
    {
        onInvincibilityStart?.Invoke();
    }
    public static void triggerInvincibilityEnd()
    {
        onInvincibilityEnd?.Invoke();
    }

    public static void triggerPlayerIsDamaged()
    {
        Debug.Log("Player is damaged");
        onPlayerDamaged?.Invoke();
    }

    public static void triggerSnackLandedOnGround()
    {
        Debug.Log("Snack landed on ground");
        onSnackLandedOnGround?.Invoke();
    }

    public static void triggerVendingComplete()
    {
        Debug.Log("Vending complete");
        onTraysCleared?.Invoke();
    }

    public static void triggerOnTurnOnLights()
    {
        Debug.Log("Turn on lights");
        turnOnLight?.Invoke();
    }
    public static void triggerOnTurnOffLights()
    {
        Debug.Log("Turn off lights");
        turnOffLight?.Invoke();
    }

    public static void triggerGameSceneEnded()
    {
        onGameSceneEnded?.Invoke();
    }
}