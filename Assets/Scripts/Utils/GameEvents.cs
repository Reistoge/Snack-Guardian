using System;
using UnityEngine;

public static class GameEvents
{
    // Health events
    public static event Action<float, float> OnHealthChanged; // current, max
    public static event Action onPlayerDeath;
    public static event Action onInvincibilityStart;
    public static event Action onInvincibilityEnd;
    public static event Action onConnectedToServer;
    public static event Action onSnackIsRequested;

    // Scene/Player events
    public static event Action<GameObject> onPlayerSpawned;

    public static void triggerHealthChanged(float current, float max)
    {
        OnHealthChanged?.Invoke(current, max);
    }
    public static void triggerConnectedToServer()
    {
        onConnectedToServer?.Invoke();
        
    }
    public static void triggerOnSnackIsRequested(){
        onSnackIsRequested?.Invoke();
    }

    public static void triggerPlayerDeath()
    {
        onPlayerDeath?.Invoke();
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
}