
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            getConnectedPlayers();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            sendPublicMessage("Hello from Unity!");
        }


    }
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
            getConnectedPlayers();
            // Request player list after successful connection

        });

        // Handle standard events
        webSocket.On.Event((eventName, data, type) =>
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            //Debug.Log($"Received raw data: {json}");

            handleServerEvent(eventName, data);
        });

        // Handle raw data responses
        webSocket.On.Data((byte[] data, HTTP.MessageType messageType) =>
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            //Debug.Log($"Received raw data: {json}");
            try
            {
                var baseMessage = JsonUtility.FromJson<ServerMessageBase>(json);
                if (baseMessage != null && !string.IsNullOrEmpty(baseMessage.@event))
                {
                    handleServerData(baseMessage.@event, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing data message: {e.Message}");
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
        Debug.Log($"Handling event: {eventName} with data: {jsonData}");

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
                //Debug.Log($"Connected with ID: {playerId}");
                MultiplayerGameEvents.triggerPlayerConnected(playerId);
                //getConnectedPlayers();

                break;
            case "player-connected":
                var playerConnectedWrapper = JsonUtility.FromJson<ServerMessage<PlayerData>>(jsonData);
                if (playerConnectedWrapper == null || playerConnectedWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse player-connected data");
                    return;
                }
                //Debug.Log($"Player connected: {playerConnectedWrapper.data.id}");
                MultiplayerGameEvents.triggerPlayerConnected(playerConnectedWrapper.data.id);
                //getConnectedPlayers();

                break;

            case "public-message":
                var publicMessageWrapper = JsonUtility.FromJson<ServerMessage<ChatMessage>>(jsonData);
                if (publicMessageWrapper == null || publicMessageWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse public-message data");
                    return;
                }
                //Debug.Log($"Received message from {publicMessageWrapper.data.id}: {publicMessageWrapper.data.msg}");
                MultiplayerGameEvents.triggerChatMessageReceived(publicMessageWrapper.data.id, publicMessageWrapper.data.msg);
                break;
            case "private-message":
                var privateMessageWrapper = JsonUtility.FromJson<ServerMessage<ChatMessage>>(jsonData);
                if (privateMessageWrapper == null || privateMessageWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse private-message data");
                    return;
                }
                //Debug.Log($"Received message from {privateMessageWrapper.data.id}: {privateMessageWrapper.data.msg}");
                MultiplayerGameEvents.triggerChatMessageReceived(privateMessageWrapper.data.id, privateMessageWrapper.data.msg);
                break;
            case "get-connected-players":
                var playersWrapper = JsonUtility.FromJson<ServerMessage<string[]>>(jsonData);
                if (playersWrapper?.data != null)
                {
                    Debug.Log($"Connected players: {string.Join(", ", playersWrapper.data)}");
                    foreach (var id in playersWrapper.data)
                    {
                        if (id != playerId) // Don't trigger for self
                        {
                            MultiplayerGameEvents.triggerPlayerConnected(id);
                        }
                    }
                }
                break;
        }
    }
    private void handleServerData(string eventName, byte[] data)
    {
        string jsonData = System.Text.Encoding.UTF8.GetString(data);
        Debug.Log($"Handling event: {eventName} with data: {jsonData}");

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
                //Debug.Log($"Connected with ID: {playerId}");
                MultiplayerGameEvents.triggerPlayerConnected(playerId);
                //getConnectedPlayers();

                break;
            case "player-connected":
                var playerConnectedWrapper = JsonUtility.FromJson<ServerMessage<PlayerData>>(jsonData);
                if (playerConnectedWrapper == null || playerConnectedWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse player-connected data");
                    return;
                }
                //Debug.Log($"Player connected: {playerConnectedWrapper.data.id}");
                MultiplayerGameEvents.triggerPlayerConnected(playerConnectedWrapper.data.id);
                //getConnectedPlayers();

                break;
            case "player-disconnected":
                var disconnectWrapper = JsonUtility.FromJson<ServerMessage<PlayerData>>(jsonData);
                if (disconnectWrapper?.data != null)
                {
                    MultiplayerGameEvents.triggerPlayerDisconnected(disconnectWrapper.data.id);
                }
                break;


            case "public-message":
                var publicMessageWrapper = JsonUtility.FromJson<ServerMessage<ChatMessage>>(jsonData);
                if (publicMessageWrapper == null || publicMessageWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse public-message data");
                    return;
                }
                //Debug.Log($"Received message from {publicMessageWrapper.data.id}: {publicMessageWrapper.data.msg}");
                MultiplayerGameEvents.triggerChatMessageReceived(publicMessageWrapper.data.id, publicMessageWrapper.data.msg);
                break;
            case "private-message":
                var privateMessageWrapper = JsonUtility.FromJson<ServerMessage<ChatMessage>>(jsonData);
                if (privateMessageWrapper == null || privateMessageWrapper.data == null)
                {
                    Debug.LogWarning("Failed to parse private-message data");
                    return;
                }
                //Debug.Log($"Received message from {privateMessageWrapper.data.id}: {privateMessageWrapper.data.msg}");
                MultiplayerGameEvents.triggerChatMessageReceived(privateMessageWrapper.data.id, privateMessageWrapper.data.msg);
                break;
            case "get-connected-players":
                var playersWrapper = JsonUtility.FromJson<ServerMessage<string[]>>(jsonData);
                if (playersWrapper?.data != null)
                {
                    Debug.Log($"Connected players: {string.Join(", ", playersWrapper.data)}");
                    foreach (var id in playersWrapper.data)
                    {
                        if (id != playerId) // Don't trigger for self
                        {
                            MultiplayerGameEvents.triggerPlayerConnected(id);
                        }
                    }
                }
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

        // var msgData = new MessageData { message = message };
        // string json = JsonUtility.ToJson(msgData);
        // //Debug.Log($"Sending message: {json}"); // Debug line
        // webSocket.To.Event("send-public-message", json, HTTP.Text);
        // MultiplayerGameEvents.triggerChatMessageReceived(playerId, message);
        // //webSocket.To.Data("send-public-message", HTTP.Text);
        // //webSocket.To.Data("send-public-message", HTTP.Text);
        var msgData = new MessageData { message = message };
        string json = JsonUtility.ToJson(msgData);
        webSocket.To.Event("send-public-message", json, HTTP.Text);


    }
    public void getConnectedPlayers()
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot get connected players - not connected");
            return;
        }

        // Send empty data for get-connected-players request
        webSocket.To.Event("get-connected-players", "{}", HTTP.Text);
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
    //public string msg; // Change from 'message' to 'msg' to match server format
    public string message; // Change from 'message' to 'msg' to match server format
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