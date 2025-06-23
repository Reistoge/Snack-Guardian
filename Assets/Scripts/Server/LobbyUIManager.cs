using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager Instance { get; private set; }
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatDisplay; // Change to TextMeshProUGUI
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button sendMessageButton;
    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private Transform playerListContainer; // donde se instanciarán

    private bool isReady = false;
    private string oponentId = "";
    
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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
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
        if (!string.IsNullOrEmpty(oponentId))
        {
            NetworkManager.Instance.sendPrivateMessage(oponentId, "ready-confirmed");
            StartGameWith(oponentId);
        }
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
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var playerId in connectedPlayers)
        {
            GameObject entryObj = Instantiate(playerEntryPrefab, playerListContainer, false);
            ConnectedPlayerEntry entry = entryObj.GetComponent<ConnectedPlayerEntry>();
            entry.Initialize(playerId);
            Debug.Log("Cantidad de botones hijos: " + playerListContainer.childCount);
        }
    }

    public void ShowReadyPopup(string playerId)
    {
        // Show a popup to confirm readiness
        Debug.Log($"Show ready popup for {playerId}");
        // Implement your popup logic here
        // For example, you can use a UI panel with buttons to confirm or decline
        // For now, just log the action
        chatDisplay.text += $"\n<color=yellow>{playerId} is ready to play!</color>";
        Canvas.ForceUpdateCanvases();
    }
    
    public void StartGameWith(string playerId)
    {
        Debug.Log($"Starting game with {playerId}");
        oponentId = playerId;
        isReady = true;
        startGameButton.interactable = true;

        // Notify the server to start the game
        NetworkManager.Instance.sendPrivateMessage(playerId, "ready-confirmed");
        // Optionally, you can also start the game locally
        // For example, load the game scene or transition to the game state
        GameManager.Instance.loadScene("GameScene"); // Uncomment if you want to load a scene
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

    public void setOponentId(string id)
    {
        oponentId = id;
        Debug.Log($"Oponent ID set to: {oponentId}");
    }
}
