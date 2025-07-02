using System;
using System.Collections.Generic;
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
   public static event Action<string, string> onMatchRequestReceived;
    public static event Action onGameStarted;
    public static event Action onPlayersListCleared;
    public static event Action<string, string> onPlayerLoggedIn;
    public static event Action<string> onLoginFailed;
    public static Action<string> onMatchRequestSent;

    public static event Action<string> onMatchRejectionSent;
    public static event Action<string, string> onMatchRejectionReceived; // (playerId, message)

    // cambio de nombre
    public static Action<string, string> onPlayerNameChanged;

    ///enviar match
    public static Action<List<ConnectionData>> onOnlinePlayersReceived;

    public static event Action<string> onMatchAccept;
    public static event Action<string> onMatchReject;
    public static event Action<string, string> onMatchAccepted;
    public static event Action<string> onMatchRejected;

    public static event Action onMatchAcceptanceSent;
    public static event Action<string, string> onMatchAcceptanceError; // (message, playerStatus)
    public static event Action<string> onMatchAcceptanceSuccess; // (message)

    // Evento cuando el servidor rechaza la solicitud
    //recibir match
    public static event Action onConnectMatchSent;
    public static event Action<string, string> onConnectMatchSuccess; // (matchId, message)
    public static event Action<string> onConnectMatchError; // (errorMessage)

    public static event Action onPingMatchSent;
    public static event Action<string, string> onPingMatchSuccess; // (matchId, message)
    public static event Action<string> onPingMatchError; // (message)

    public static event Action<string, string> onMatchStart;
    public static event Action<string, string> onPlayersReady;

    public static event Action onFinishGameSent;
    public static event Action<string, string> onFinishGameSuccess; // (matchId, message)
    public static event Action<string> onFinishGameError; // (message)

    public static event Action onQuitMatchSent;
    public static event Action<string, string> onQuitMatchSuccess; // (playerStatus, message)
    public static event Action<string> onQuitMatchError; // (message)

    public static event Action<string, string> onShowPrivateMessagePanel;

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
    public static void triggerMatchRequestSent(string matchId)
    {
        onMatchRequestSent?.Invoke(matchId);
    }

    public static void triggerPlayerSendAttack(string attackData) => onPlayerSendAttack?.Invoke(attackData);
    public static void triggerPlayerReceiveAttack(string attackData) => onPlayerReceiveAttack?.Invoke(attackData);
    public static void triggerPlayersListCleared() => onPlayersListCleared?.Invoke(); 
    public static void triggerPlayerLoggedIn(string playerId, string playerName) => onPlayerLoggedIn?.Invoke(playerId, playerName);
    public static void triggerLoginFailed(string errorMessage) => onLoginFailed?.Invoke(errorMessage);

    public static void triggerOnlinePlayersReceived(List<ConnectionData> players)
    {
        onOnlinePlayersReceived?.Invoke(players);
    }
    public static void triggerMatchRequestReceived(string playerId, string matchId)
    {
        onMatchRequestReceived?.Invoke(playerId, matchId);
    }

    // Funciones para disparar los eventos
    public static void triggerMatchAccept(string matchId)
    {
        onMatchAccept?.Invoke(matchId);
    }

    public static void triggerMatchReject(string playerId)
    {
        onMatchReject?.Invoke(playerId);
    }

    public static void triggerMatchAccepted(string matchId, string matchStatus)
    {
        onMatchAccepted?.Invoke(matchId, matchStatus);
    }

    public static void triggerMatchRejected(string playerId)
    {
        onMatchRejected?.Invoke(playerId);
    }

    public static void triggerMatchRejectionSent(string playerId)
    {
        onMatchRejectionSent?.Invoke(playerId);
    }

    public static void triggerMatchRejectionReceived(string playerId, string message)
    {
        onMatchRejectionReceived?.Invoke(playerId, message);
    }

    // Agregar estos métodos trigger:
    public static void triggerMatchAcceptanceSent()
    {
        onMatchAcceptanceSent?.Invoke();
    }

    public static void triggerMatchAcceptanceError(string message, string playerStatus)
    {
        onMatchAcceptanceError?.Invoke(message, playerStatus);
    }

    public static void triggerMatchAcceptanceSuccess(string message)
    {
        onMatchAcceptanceSuccess?.Invoke(message);
    }

    public static void triggerConnectMatchSent()
    {
        onConnectMatchSent?.Invoke();
    }

    public static void triggerConnectMatchSuccess(string matchId, string message)
    {
        onConnectMatchSuccess?.Invoke(matchId, message);
    }

    public static void triggerConnectMatchError(string errorMessage)
    {
        onConnectMatchError?.Invoke(errorMessage);
    }

    public static void triggerPlayersReady(string matchId, string message)
    {
        onPlayersReady?.Invoke(matchId, message);
    }

    public static void triggerPingMatchSent()
    {
        onPingMatchSent?.Invoke();
    }

    public static void triggerPingMatchSuccess(string matchId, string message)
    {
        onPingMatchSuccess?.Invoke(matchId, message);
    }

    public static void triggerPingMatchError(string message)
    {
        onPingMatchError?.Invoke(message);
    }

    public static void triggerMatchStart(string matchId, string message)
    {
        onMatchStart?.Invoke(matchId, message);
    }

    public static void triggerFinishGameSent()
    {
        onFinishGameSent?.Invoke();
    }

    public static void triggerFinishGameSuccess(string matchId, string message)
    {
        onFinishGameSuccess?.Invoke(matchId, message);
    }

    public static void triggerFinishGameError(string message)
    {
        onFinishGameError?.Invoke(message);
    }

    public static void triggerQuitMatchSent()
    {
        onQuitMatchSent?.Invoke();
    }

    public static void triggerQuitMatchSuccess(string playerStatus, string message)
    {
        onQuitMatchSuccess?.Invoke(playerStatus, message);
    }

    public static void triggerQuitMatchError(string message)
    {
        onQuitMatchError?.Invoke(message);
    }

    public static void triggerShowPrivateMessagePanel(string playerId, string playerName)
    {
        onShowPrivateMessagePanel?.Invoke(playerId, playerName);
    }

}
