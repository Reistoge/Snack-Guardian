using System;

public static class MultiplayerGameEvents 
{
    // Network events
    public static event Action onConnectedToServer;
    public static event Action onDisconnectedFromServer;
    public static event Action<string> onConnectionError;
    public static event Action<string> onPlayerConnected;
    public static event Action<string> onPlayerDisconnected;
    public static event Action<string, string> onChatMessageReceived;
    public static event Action onGameStarted;

    public static void triggerConnectedToServer() => onConnectedToServer?.Invoke();
    public static void triggerDisconnectedFromServer() => onDisconnectedFromServer?.Invoke();
    public static void triggerConnectionError(string error) => onConnectionError?.Invoke(error);
    public static void triggerPlayerConnected(string id) => onPlayerConnected?.Invoke(id);
    public static void triggerPlayerDisconnected(string id) => onPlayerDisconnected?.Invoke(id);
    public static void triggerChatMessageReceived(string id, string msg) => onChatMessageReceived?.Invoke(id, msg);
    public static void triggerGameStarted() => onGameStarted?.Invoke();
    
    internal static void triggerPlayerReadyStateChanged(string playerId, bool isReady)
    {
      
    }
}