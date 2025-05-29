using UnityEngine;
using Netly;
using System;
using System.Net.WebSockets;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    public HTTP.WebSocket WebSocket { get => webSocket; set => webSocket = value; }

    [SerializeField] private string serverUrl = "ws://ucn-game-server.martux.cl:4010/";
    private HTTP.WebSocket webSocket;
    [SerializeField] private string playerId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            print("NetworkManager instance created");
            DontDestroyOnLoad(gameObject);
            initializeWebSocket();
            connect();

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
            Debug.Log($"WebSocket Event received: {eventName}"); // <-- Este debug confirma que llega el evento
            handleServerEvent(eventName, data);
        });

        webSocket.On.Data((byte[] data, HTTP.MessageType messageType) =>
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received raw JSON data: {json}");

            // Extraemos solo el campo 'event'
            var baseEvent = JsonUtility.FromJson<ServerMessageBase>(json);
            if (baseEvent != null && !string.IsNullOrEmpty(baseEvent.@event))
            {
                handleServerEvent(baseEvent.@event, data);
            }
            else
            {
                Debug.LogWarning("No 'event' field found in message JSON");
            }
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
        Debug.Log($"Handling event '{eventName}' with data: {jsonData}");

        switch (eventName)
        {
            case "connected-to-server":
                var connectionWrapper = JsonUtility.FromJson<ServerMessage<ConnectionData>>(jsonData);
                if (connectionWrapper == null || connectionWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse connected-to-server data");
                    return;
                }
                playerId = connectionWrapper.data.id;
                Debug.Log($"Connected with ID: {playerId}");
                MultiplayerGameEvents.triggerPlayerConnected(playerId);
                break;

            case "public-message":
                var publicMessageWrapper = JsonUtility.FromJson<ServerMessage<ChatMessage>>(jsonData);
                if (publicMessageWrapper == null || publicMessageWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse public-message data");
                    return;
                }
                Debug.Log($"Received message from {publicMessageWrapper.data.id}: {publicMessageWrapper.data.msg}");
                MultiplayerGameEvents.triggerChatMessageReceived(publicMessageWrapper.data.id, publicMessageWrapper.data.msg);
                break;

            default:
                Debug.LogWarning($"Unhandled event: {eventName}");
                break;
        }
    }

    public void sendPublicMessage(string message)
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot send message - not connected");
            return;
        }

        var msgData = new MessageData { message = message };
        string json = JsonUtility.ToJson(msgData);
        Debug.Log($"Sending message: {json}"); // Debug line
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
    public string msg;
    public string id;
}

[Serializable]
public class PlayerData
{
    public string msg;
    public string id;
}

[Serializable]
public class ChatMessage
{
    public string id;
    public string msg;
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

[Serializable]
public class ReadyStateData
{
    public bool isReady;
}

[Serializable]
public class ServerMessage<T>
{
    public string @event;
    public T data;
}

public class ServerMessageBase
{
    public string @event;
}