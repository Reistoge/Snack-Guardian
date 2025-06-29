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

    // private void Start()
    // {
    //     startGameButton.interactable = false;
    //     readyButton.onClick.AddListener(onReadyClick);
    //     sendMessageButton.onClick.AddListener(sendChatMessage);

    //     // Subscribe to all events
    //     MultiplayerGameEvents.onConnectedToServer += handleConnectedToServer;
    //     //Debug.Log("Subscribed to onChatMessageReceived");
    //     MultiplayerGameEvents.onChatMessageReceived += handleChatMessage;
    //     MultiplayerGameEvents.onPlayerConnected += handlePlayerConnected;
    //     MultiplayerGameEvents.onPlayerDisconnected += handlePlayerDisconnected;

    //     // Initialize chat display
    //     chatDisplay.text = "Welcome to chat...";
    // }
    private void Start()
    {
        startGameButton.interactable = false;
        readyButton.onClick.AddListener(onReadyClick);
        sendMessageButton.onClick.AddListener(sendChatMessage);

        // Subscribe to all events
        MultiplayerGameEvents.onConnectedToServer += handleConnectedToServer;
        MultiplayerGameEvents.onChatMessageReceived += handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected += handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected += handlePlayerDisconnected;
        MultiplayerGameEvents.onPlayersListCleared += handlePlayersListCleared;

        chatDisplay.text = "Welcome to chat...";
    }

    private void onReadyClick()
    {
        // throw new NotImplementedException();
    }

    private void handlePlayerDisconnected(string obj)
    {
        Debug.Log($"Player disconnected: {obj}"); // Debug line
        if (connectedPlayers.Contains(obj))
        {
            connectedPlayers.Remove(obj);
            updatePlayerList();
            chatDisplay.text += $"\n<color=red>Player {obj} left</color>";
        }
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


    private void handlePlayersListCleared()
    {
        connectedPlayers.Clear();
        updatePlayerList();
    }

    private void handleConnectedToServer()
    {
        chatDisplay.text = "Connected to chat room";
        connectedPlayers.Clear();
        updatePlayerList();
        NetworkManager.Instance.getConnectedPlayers(); // Request player list after connection
    }

    private void handlePlayerConnected(string playerId)
    {
        //Debug.Log($"Player connected: {playerId}"); // Debug line
        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
            updatePlayerList();
            chatDisplay.text += $"\n<color=green>Player {playerId} joined</color>";
        }
    }

    private void updatePlayerList()
    {
        // throw new NotImplementedException();
        playerList.text = "Jugadores conectados\n";
        foreach (var playerId in connectedPlayers)
        {
            playerList.text += "id: " + playerId + "\n";
        }

    }

    private void OnDestroy()
    {
        // Unsubscribe from all events
        MultiplayerGameEvents.onConnectedToServer -= handleConnectedToServer;
        MultiplayerGameEvents.onChatMessageReceived -= handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected -= handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected -= handlePlayerDisconnected;
        MultiplayerGameEvents.onPlayersListCleared -= handlePlayersListCleared;
    }
}
