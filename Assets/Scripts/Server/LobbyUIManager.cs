using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatDisplay; // Change to TextMeshProUGUI
    [SerializeField] private TextMeshProUGUI playerList;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button sendMessageButton;
    
    private bool isReady = false;
    private List<string> connectedPlayers = new List<string>();

    private void Start()
    {
        startGameButton.interactable = false;
        readyButton.onClick.AddListener(onReadyClick);
        sendMessageButton.onClick.AddListener(sendChatMessage);
        
        // Subscribe to all events
        MultiplayerGameEvents.onConnectedToServer += handleConnectedToServer;
        Debug.Log("Subscribed to onChatMessageReceived");
        MultiplayerGameEvents.onChatMessageReceived += handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected += handlePlayerConnected;
        // Initialize chat display
        chatDisplay.text = "Welcome to chat...";
    }

    private void onReadyClick()
    {
        // throw new NotImplementedException();
    }

    private void handlePlayerDisconnected(string obj)
    {
         //throw new NotImplementedException();
    }

    public void sendChatMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            Debug.Log($"Sending message: {chatInput.text}"); // Debug line
            NetworkManager.Instance.sendPublicMessage(chatInput.text);
            chatInput.text = "";
        }
    }

    private void handleChatMessage(string playerId, string message)
    {
        Debug.Log($"Handling chat message: {playerId}: {message}"); // Debug line
        
        // Ensure UI updates happen on main thread
        if (!this) return;

        chatDisplay.text += $"\n{playerId}: {message}";
        Canvas.ForceUpdateCanvases(); // Force UI refresh
    }

    private void handleConnectedToServer()
    {
        Debug.Log("Connected to server, clearing chat"); // Debug line
        chatDisplay.text = "Connected to chat room";
        connectedPlayers.Clear();
        updatePlayerList();
    }

    private void handlePlayerConnected(string playerId)
    {
        Debug.Log($"Player connected: {playerId}"); // Debug line
        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
            updatePlayerList();
            chatDisplay.text += $"\n<color=green>Player {playerId} joined</color>";
        }
    }

    private void updatePlayerList()
    {
        playerList.text = "Jugadores conectados:\n";
        foreach (var playerId in connectedPlayers)
        {
            playerList.text += playerId + "\n";
        }

    }

    private void OnDestroy()
    {
        MultiplayerGameEvents.onConnectedToServer -= handleConnectedToServer;
        MultiplayerGameEvents.onChatMessageReceived -= handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected -= handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected -= handlePlayerDisconnected;
    }
}