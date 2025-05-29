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
    private HashSet<string> connectedPlayers = new HashSet<string>(); // Changed to HashSet for unique values

    private void Start()
    {
        chatDisplay.text = "";
        startGameButton.interactable = false;
        readyButton.onClick.AddListener(onReadyClick);
        sendMessageButton.onClick.AddListener(sendChatMessage);

        // Subscribe to all events
        MultiplayerGameEvents.onConnectedToServer += handleConnectedToServer;
        //Debug.Log("Subscribed to onChatMessageReceived");
        MultiplayerGameEvents.onChatMessageReceived += handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected += handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected += removePlayerFromUI;

        // Initialize chat display
        chatDisplay.text = "Welcome to chat...";
        chatInput.onSubmit.AddListener((value) => sendChatMessage());
        
    }

    private void removePlayerFromUI(string obj)
    {
        // Debug.Log($"Removing player from UI: {obj}");
        if (connectedPlayers.Contains(obj))
        {
            connectedPlayers.Remove(obj);
            updatePlayerList();
            AddSystemMessage($"Player {obj} left");
        }
        else
        {
            Debug.LogWarning($"Player {obj} not found in connected players list.");
        }
        
        // Optionally, you can also update the chat display
        chatDisplay.text += $"\n<color=#FF0000>Player {obj} has disconnected.</color>";
    }

    private void onReadyClick()
    {
        // throw new NotImplementedException();
    }

    private void handlePlayerDisconnected(string obj)   
    {
        //throw new NotImplementedException();
        Debug.Log($"Player disconnected: {obj}");
    }

    public void sendChatMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            string message = chatInput.text;
            //Debug.Log($"Sending message: {message}");

            NetworkManager.Instance.sendPublicMessage(message);

            // Clear input and refocus
            chatInput.text = "";
            chatInput.ActivateInputField();
        }
    }
    private void handleChatMessage(string playerId, string message)
    {
        // Debug.Log($"Handling chat message from {playerId}: {message}");
        
        if (this == null || chatDisplay == null) return;

        // Format message with player ID
        string formattedMessage = $"\n<color=#4CAF50>{playerId}</color>: {message}";
        
        // Add to chat display
        chatDisplay.text += formattedMessage;
        
        // Force layout update
        Canvas.ForceUpdateCanvases();
        
        // Auto scroll if we have a scroll rect
        if (chatDisplay.transform.parent.TryGetComponent<ScrollRect>(out var scrollRect))
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void handleConnectedToServer()
    {
        //Debug.Log("Connected to server, clearing chat"); // Debug line
        chatDisplay.text = "Connected to chat room";
     
        connectedPlayers.Clear();
        updatePlayerList();
        
        // Request initial player list
        //NetworkManager.Instance.getConnectedPlayers();
    }

    private void handlePlayerConnected(string playerId)
    {
        // Debug.Log($"Player connected: {playerId}"); // Debug line
        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
            updatePlayerList();
            AddSystemMessage($"Player {playerId} joined");
        }
    }

    private void updatePlayerList()
    {
        if (!playerList) return;
        
        playerList.text = "Connected Players:\n";
        foreach (var id in connectedPlayers)
        {
            playerList.text += $"• {id}\n";
        }
        
        // Update UI
        Canvas.ForceUpdateCanvases();
    }

    private void AddSystemMessage(string message)
    {
        if (!chatDisplay) return;
        chatDisplay.text += $"\n<color=#FFD700>{message}</color>";
    }

    private void OnDestroy()
    {
        MultiplayerGameEvents.onConnectedToServer -= handleConnectedToServer;
        MultiplayerGameEvents.onChatMessageReceived -= handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected -= handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected -= handlePlayerDisconnected;
        MultiplayerGameEvents.onPlayerDisconnected -= removePlayerFromUI;

    }
}