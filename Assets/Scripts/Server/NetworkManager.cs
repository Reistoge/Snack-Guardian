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
            //print("NetworkManager instance created");
            DontDestroyOnLoad(gameObject);
            initializeWebSocket();
            connect();

        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            getConnectedPlayers();
        }
    }

    private void initializeWebSocket()
    {
        webSocket = new HTTP.WebSocket();

        webSocket.On.Open(() =>
        {
            //Debug.Log("Connected to server");
            MultiplayerGameEvents.triggerConnectedToServer();
            getConnectedPlayers();
        });

        webSocket.On.Close(() =>
        {
            //Debug.Log("Disconnected from server");
            MultiplayerGameEvents.triggerDisconnectedFromServer();
        });
        webSocket.On.Event((eventName, data, type) =>
        {
            Debug.Log($"WebSocket Event received: {eventName} with data: {System.Text.Encoding.UTF8.GetString(data)}");
            handleServerData(eventName, data);
        });
        webSocket.On.Error((exception) =>
        {
            Debug.LogError($"WebSocket Error: {exception.Message}");
            MultiplayerGameEvents.triggerConnectionError(exception.Message);
        });

        webSocket.On.Data((byte[] data, HTTP.MessageType messageType) =>
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received raw JSON data: {json}");

            // Extraemos solo el campo 'event'
            var baseEvent = JsonUtility.FromJson<ServerMessageBase>(json);
            if (baseEvent != null && !string.IsNullOrEmpty(baseEvent.@event))
            {
                handleServerData(baseEvent.@event, data);
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

    private void handleServerData(string eventName, byte[] data)
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
                // Debug.Log($"Connected with ID: {playerId}");
                MultiplayerGameEvents.triggerPlayerConnected(playerId);
                break;
            case "player-connected":
                var playerWrapper = JsonUtility.FromJson<ServerMessage<PlayerData>>(jsonData);
                if (playerWrapper == null || playerWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse player-connected data");
                    return;
                }
                Debug.Log($"Player connected: {playerWrapper.data.id}");
                MultiplayerGameEvents.triggerPlayerConnected(playerWrapper.data.id);

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
            case "send-public-message":
                var sendPublicMessageWrapper = JsonUtility.FromJson<ServerMessage<MessageData>>(jsonData);
                if (sendPublicMessageWrapper == null || sendPublicMessageWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse public-message data");
                    return;
                }
                // Debug.Log($"Received message from {sendPublicMessageWrapper.data.id}: {sendPublicMessageWrapper.data.msg}");
                Debug.Log($"Public message sent: {sendPublicMessageWrapper.data.message}");
                break;
            case "get-connected-players":
                var playersConnectedWrapper = JsonUtility.FromJson<PlayersConnectedData>(jsonData);
                print(playersConnectedWrapper);
                if (playersConnectedWrapper?.data != null)
                {
                    Debug.Log($"Connected players: {string.Join(", ", playersConnectedWrapper.data)}");
                    // Clear existing players first
                    MultiplayerGameEvents.triggerPlayersListCleared();
                    // Add each player
                    foreach (string playerId in playersConnectedWrapper.data)
                    {
                        print(playerId);
                        MultiplayerGameEvents.triggerPlayerConnected(playerId);
                    }
                }
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
        var serverMessage = new ServerMessage<MessageData>
        {
            @event = "send-public-message",
            data = msgData
        };
        string json = JsonUtility.ToJson(serverMessage);
        Debug.Log($"Sending message: {json}"); // Debug line
        webSocket.To.Data(json, HTTP.Text);
    }

    public void sendPrivateMessage(string targetId, string message)
    {
        var msgData = new PrivateMessageData { id = targetId, message = message };
        string json = JsonUtility.ToJson(msgData);
        webSocket.To.Data(json, HTTP.Text);
    }
    public void getConnectedPlayers()
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot get connected players - not connected");
            return;
        }

        var getMessage = new ServerMessage<object>
        {
            @event = "get-connected-players",
            data = null
        };
        string json = JsonUtility.ToJson(getMessage);
        webSocket.To.Data(json, HTTP.Text);
    }
    private void OnDestroy()
    {
        disconnect();
    }
    public void sendReadyState(bool isReady)
    {
        var readyData = new ReadyStateData { isReady = isReady };
        string json = JsonUtility.ToJson(readyData);
        webSocket.To.Data(json, HTTP.Text);
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
// [Serializable]
// public class PlayersConnectedData
// {
//     public string data;
// }
[Serializable]
public class PlayersConnectedData
{
    public string @event;
    public string[] data; // Changed from string to string[] to match server response
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