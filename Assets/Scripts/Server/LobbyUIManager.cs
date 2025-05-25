using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatDisplay;
    [SerializeField] private TextMeshProUGUI playerList;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    private bool isReady = false;
    private void Start()
    {
        startGameButton.interactable = false;
        readyButton.onClick.AddListener(onReadyClick);

        // Subscribe to events
        MultiplayerGameEvents.onChatMessageReceived += handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected += handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected += handlePlayerDisconnected;
    }

    public void sendChatMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            NetworkManager.Instance.sendPublicMessage(chatInput.text);
            chatInput.text = "";
        }
    }

    private void handleChatMessage(string playerId, string message)
    {
        chatDisplay.text += $"\n{playerId}: {message}";
    }

    private void handlePlayerConnected(string playerId)
    {
        updatePlayerList();
    }

    private void handlePlayerDisconnected(string playerId)
    {
        updatePlayerList();
    }

    private void updatePlayerList()
    {
        // Update player list UI
    }



    private void onReadyClick()
    {
        isReady = !isReady;
        var readyData = new ReadyStateData { isReady = isReady };
        string json = JsonUtility.ToJson(readyData);
        NetworkManager.Instance.WebSocket.To.Event("player-ready", json, Netly.HTTP.Text);

        // Update button text
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text =
            isReady ? "Not Ready" : "Ready";
    }

    private void OnDestroy()
    {
        MultiplayerGameEvents.onChatMessageReceived -= handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected -= handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected -= handlePlayerDisconnected;
    }
}

[Serializable]
public class ReadyStateData
{
    public bool isReady;
}