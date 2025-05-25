using UnityEngine;
using Netly;
using System;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    public HTTP.WebSocket WebSocket { get => webSocket; set => webSocket = value; }

    [SerializeField] private string serverUrl = "ws://your-server.com/ws";
    private HTTP.WebSocket webSocket;
    private string playerId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            initializeWebSocket();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void initializeWebSocket()
    {
        webSocket = new HTTP.WebSocket();

        webSocket.On.Open(() =>
        {
            Debug.Log("Connected to server");
            MultiplayerGameEvents.triggerConnectedToServer();
        });

        webSocket.On.Close(() =>
        {
            Debug.Log("Disconnected from server");
            MultiplayerGameEvents.triggerDisconnectedFromServer();
        });

        webSocket.On.Error((exception) =>
        {
            Debug.LogError($"WebSocket Error: {exception.Message}");
            MultiplayerGameEvents.triggerConnectionError(exception.Message);
        });

        webSocket.On.Event((eventName, data, type) =>
        {
            handleServerEvent(eventName, data);
        });
    }

    public void connect()
    {
        if (!webSocket.IsOpened)
        {
            webSocket.To.Open(new Uri(serverUrl));
        }
    }

    public void disconnect()
    {
        if (webSocket.IsOpened)
        {
            webSocket.To.Close();
        }
    }

    private void handleServerEvent(string eventName, byte[] data)
    {
        string jsonData = System.Text.Encoding.UTF8.GetString(data);

        switch (eventName)
        {
            case "connected-to-server":
                var connectionData = JsonUtility.FromJson<ConnectionData>(jsonData);
                playerId = connectionData.Id;
                break;

            case "player-connected":
                var playerData = JsonUtility.FromJson<PlayerData>(jsonData);
                MultiplayerGameEvents.triggerPlayerConnected(playerData.Id);
                break;

            case "public-message":
                var publicMsg = JsonUtility.FromJson<ChatMessage>(jsonData);
                MultiplayerGameEvents.triggerChatMessageReceived(publicMsg.Id, publicMsg.Msg);
                break;
            case "player-ready":
                var readyData = JsonUtility.FromJson<ReadyStateData>(jsonData);
                MultiplayerGameEvents.triggerPlayerReadyStateChanged(playerId, readyData.isReady);
                break;
        }
    }

    public void sendPublicMessage(string message)
    {
        var msgData = new MessageData { message = message };
        string json = JsonUtility.ToJson(msgData);
        webSocket.To.Event("send-public-message", json, HTTP.Text);
    }

    public void sendPrivateMessage(string targetId, string message)
    {
        var msgData = new PrivateMessageData { id = targetId, message = message };
        string json = JsonUtility.ToJson(msgData);
        webSocket.To.Event("send-private-message", json, HTTP.Text);
    }

    private void OnDestroy()
    {
        disconnect();
    }
    public void sendReadyState(bool isReady)
    {
        var readyData = new ReadyStateData { isReady = isReady };
        string json = JsonUtility.ToJson(readyData);
        webSocket.To.Event("player-ready", json, HTTP.Text);
    }
    

 
}

[Serializable]
public class ConnectionData
{
    public string Msg;
    public string Id;
}

[Serializable]
public class PlayerData
{
    public string Msg;
    public string Id;
}

[Serializable]
public class ChatMessage
{
    public string Id;
    public string Msg;
}

[Serializable]
public class MessageData
{
    public string message;
}

[Serializable]
public class PrivateMessageData
{
    public string id;
    public string message;
}