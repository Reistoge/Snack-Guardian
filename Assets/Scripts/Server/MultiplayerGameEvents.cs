using System;
using UnityEngine;

public static class MultiplayerGameEvents
{
    // Network events
    public static event Action onConnectedToServer;
    public static event Action onDisconnectedFromServer;
    public static event Action<string> onConnectionError;
    public static event Action<string> onPlayerConnected;
    public static event Action<string> onPlayerDisconnected;
    public static event Action<string, string> onChatMessageReceived;
    public static event Action<string, bool> onPlayerReadyStateChanged;

    public static event Action<string> onPlayerSendAttack;
    public static event Action<string> onPlayerReceiveAttack;
   
    public static event Action onGameStarted;
    public static event Action onPlayersListCleared;
    public static event Action<string, string> onPlayerLoggedIn;
    public static event Action<string> onLoginFailed;

    public static void triggerConnectedToServer() => onConnectedToServer?.Invoke();
    public static void triggerDisconnectedFromServer() => onDisconnectedFromServer?.Invoke();
    public static void triggerConnectionError(string error) => onConnectionError?.Invoke(error);
    public static void  triggerPlayerConnected(string id) => onPlayerConnected?.Invoke(id);
    public static void triggerPlayerDisconnected(string id) => onPlayerDisconnected?.Invoke(id);
    public static void triggerChatMessageReceived(string id, string msg) => onChatMessageReceived?.Invoke(id, msg);
    public static void triggerGameStarted() => onGameStarted?.Invoke();
    public static void triggerPlayerReadyStateChanged(string playerId, bool isReady)
    {
        onPlayerReadyStateChanged?.Invoke(playerId, isReady);
    }
    public static void triggerPlayerSendAttack(string attackData) => onPlayerSendAttack?.Invoke(attackData);
    public static void triggerPlayerReceiveAttack(string attackData) => onPlayerReceiveAttack?.Invoke(attackData);
    public static void triggerPlayersListCleared() => onPlayersListCleared?.Invoke(); 
    public static void triggerPlayerLoggedIn(string playerId, string playerName) => onPlayerLoggedIn?.Invoke(playerId, playerName);
    public static void triggerLoginFailed(string errorMessage) => onLoginFailed?.Invoke(errorMessage);
}