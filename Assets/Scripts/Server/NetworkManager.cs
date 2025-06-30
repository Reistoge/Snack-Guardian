using UnityEngine;
using Netly;
using System;

public class NetworkManager : MonoBehaviour
{
    #region Singleton
    public static NetworkManager Instance { get; private set; }
    #endregion

    #region WebSocket Properties
    public HTTP.WebSocket WebSocket { get => webSocket; set => webSocket = value; }
    public string PlayerId => playerId;
    public string PlayerName => playerName;
    public ConnectionData CurrentPlayerData => currentPlayerData;

    private const string GAME_KEY = "GJNTOU";
    #endregion

    #region Serialized Fields
    [SerializeField] private string serverUrl = "ws://ucn-game-server.martux.cl:4010/?gameId=H&name=Snack Guardian";
    [SerializeField] private ConnectionData currentPlayerData;
    #endregion

    #region Private Fields
    private HTTP.WebSocket webSocket;
    private string playerId;
    private string playerName;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        initializeSingleton();
    }

    private void Update()
    {
        handleDebugInput();
    }

    private void OnDestroy()
    {
        disconnect();
    }
    #endregion

    #region Initialization
    private void initializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
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

        SetupWebSocketEvents();
    }

    private void SetupWebSocketEvents()
    {
        webSocket.On.Open(onWebSocketOpen);
        webSocket.On.Close(onWebSocketClose);
        webSocket.On.Error(onWebSocketError);
        webSocket.On.Event(onWebSocketEvent);
        webSocket.On.Data(onWebSocketData);
    }
    #endregion

    #region WebSocket Event Handlers
    private void onWebSocketOpen()
    {
        MultiplayerGameEvents.triggerConnectedToServer();
        getConnectedPlayers();
    }

    private void onWebSocketClose()
    {
        MultiplayerGameEvents.triggerDisconnectedFromServer();
    }

    private void onWebSocketError(Exception exception)
    {
        Debug.LogError($"WebSocket Error: {exception.Message}");
        MultiplayerGameEvents.triggerConnectionError(exception.Message);
    }

    private void onWebSocketEvent(string eventName, byte[] data, HTTP.MessageType type)
    {
        Debug.Log($"WebSocket Event received: {eventName} with data: {System.Text.Encoding.UTF8.GetString(data)}");
        handleServerData(eventName, data);
    }

    private void onWebSocketData(byte[] data, HTTP.MessageType messageType)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        Debug.Log($"Received raw JSON data: {json}");

        var baseEvent = JsonUtility.FromJson<ServerMessageBase>(json);
        if (baseEvent != null && !string.IsNullOrEmpty(baseEvent.@event))
        {
            handleServerData(baseEvent.@event, data);
        }
        else
        {
            Debug.LogWarning("No 'event' field found in message JSON");
        }
    }
    #endregion

    #region Server Event Handling
    private void handleServerData(string eventName, byte[] data)
    {
        string jsonData = System.Text.Encoding.UTF8.GetString(data);
        Debug.Log($"Handling event '{eventName}' with data: {jsonData}");

        switch (eventName)
        {
            case "connected-to-server":
                handleConnectedToServer(jsonData);
                break;
            case "login":
                handleLoginResponse(jsonData);
                break;
            case "player-connected":
                handlePlayerConnected(jsonData);
                break;
            case "player-disconnected":
                handlePlayerDisconnected(jsonData);
                break;
            case "public-message":
                handlePublicMessage(jsonData);
                break;
            case "private-message":
                handlePrivateMessage(jsonData);
                break;
            case "send-public-message":
                handleSendPublicMessage(jsonData);
                break;
            case "send-private-message":
                handleSendPrivateMessage(jsonData);
                break;
            case "online-players":
                handleGetConnectedPlayers(jsonData);
                break;
            case "player-name-changed":
                handlePlayerNameChanged(jsonData);
                break;
            case "change-name":
                handleChangeNameResponse(jsonData);
                break;
            case "player-data":
                handlePlayerDataResponse(jsonData);
                break;
            default:
                Debug.LogWarning($"Unhandled event: {eventName}");
                break;
        }
    }

    private void handleConnectedToServer(string jsonData)
    {
        var connectionResponse = JsonUtility.FromJson<ConnectedToServerResponse>(jsonData);

        if (connectionResponse?.data == null)
        {
            Debug.LogWarning("Failed to parse connected-to-server data");
            return;
        }

        StorePlayerData(connectionResponse.data);
        logConnectionInfo();
        triggerConnectionEvents();
        handleLoginStatus(connectionResponse.data.status);


    }



    private void handleLoginStatus(string status)
    {
        if (status == "NO_LOGIN")
        {
            Debug.Log("Player needs to login. Sending login request...");
            sendLoginRequest();
        }
        else
        {
            Debug.Log($"Player is already logged in with status: {status}");
            MultiplayerGameEvents.triggerPlayerLoggedIn(playerId, playerName);
        }
    }
 
    private void sendLoginRequest()
    {
        var loginMessage = new ServerMessage<LoginData>
        {
            @event = "login",
            data = new LoginData { gameKey = GAME_KEY } // Use your actual game key
        };

        string json = JsonUtility.ToJson(loginMessage);
        Debug.Log($"Sending login request: {json}");
        webSocket.To.Data(json, HTTP.Text);
    }
    private void handleLoginResponse(string jsonData)
    {
        var loginResponse = JsonUtility.FromJson<LoginResponse>(jsonData);

        if (loginResponse == null)
        {
            Debug.LogWarning("Failed to parse login response");
            return;
        }

        Debug.Log($"Login Response - Status: {loginResponse.status}, Message: {loginResponse.msg}");

        if (loginResponse.status == "OK" && loginResponse.data != null)
        {
            // Update player data with login response
            StorePlayerData(loginResponse.data);
            logConnectionInfo();

            Debug.Log("Login successful! Player is now logged in.");
            MultiplayerGameEvents.triggerPlayerLoggedIn(playerId, playerName);
        }
        else
        {
            Debug.LogError($"Login failed: {loginResponse.msg}");
            MultiplayerGameEvents.triggerLoginFailed(loginResponse.msg);
        }
    }

    private void handlePlayerConnected(string jsonData)
    {
        var playerWrapper = JsonUtility.FromJson<PlayerConnectedResponse>(jsonData);
        if (playerWrapper?.data != null)
        {
            Debug.Log($"Player connected: {playerWrapper.data.id}");
            MultiplayerGameEvents.triggerPlayerConnected(playerWrapper.data.id);
        }
    }

    private void handlePlayerDisconnected(string jsonData)
    {
        var playerDisconnectedWrapper = JsonUtility.FromJson<PlayerDisconnectedResponse>(jsonData);
        if (playerDisconnectedWrapper?.data != null)
        {
            Debug.Log($"Player disconnected: {playerDisconnectedWrapper.data.id}");
            MultiplayerGameEvents.triggerPlayerDisconnected(playerDisconnectedWrapper.data.id);
        }
    }

    private void handlePublicMessage(string jsonData)
    {
        var publicMessageWrapper = JsonUtility.FromJson<PublicMessageResponse>(jsonData);
        if (publicMessageWrapper?.data != null)
        {
            Debug.Log($"Received message from {publicMessageWrapper.data.playerId}: {publicMessageWrapper.data.playerMsg}");
            MultiplayerGameEvents.triggerChatMessageReceived(publicMessageWrapper.data.playerId, publicMessageWrapper.data.playerMsg);
        }
    }

    private void handleSendPublicMessage(string jsonData)
    {
        var sendPublicMessageWrapper = JsonUtility.FromJson<ServerMessage<MessageData>>(jsonData);
        if (sendPublicMessageWrapper?.data != null)
        {
            Debug.Log($"Public message sent: {sendPublicMessageWrapper.data.message}");
            MultiplayerGameEvents.triggerChatMessageReceived("You", sendPublicMessageWrapper.data.message);
        }
    }

    private void handlePrivateMessage(string jsonData)
    {
        var privateMessageWrapper = JsonUtility.FromJson<ServerMessage<PrivateMessageData>>(jsonData);
        if (privateMessageWrapper?.data != null)
        {
            Debug.Log($"Received private message from {privateMessageWrapper.data.id}: {privateMessageWrapper.data.message}");
            // Trigger private message event if you want to handle private messages differently
            // For now, we'll treat it like a chat message but you can create a separate event
            MultiplayerGameEvents.triggerChatMessageReceived($"[Private] {privateMessageWrapper.data.id}", privateMessageWrapper.data.message);
        }
    }

    private void handleSendPrivateMessage(string jsonData)
    {
        var sendPrivateMessageWrapper = JsonUtility.FromJson<ServerMessage<PrivateMessageData>>(jsonData);
        if (sendPrivateMessageWrapper?.data != null)
        {
            Debug.Log($"Private message sent to {sendPrivateMessageWrapper.data.id}: {sendPrivateMessageWrapper.data.message}");
        }
    }

    private void handleGetConnectedPlayers(string jsonData)
    {
        var playersConnectedWrapper = JsonUtility.FromJson<GetConnectedPlayersResponse>(jsonData);
        if (playersConnectedWrapper?.data != null)
        {
            Debug.Log($"Connected players: {string.Join(", ", playersConnectedWrapper.data)}");
            updateConnectedPlayersList(playersConnectedWrapper.data);
        }
    }

    private void handlePlayerNameChanged(string jsonData)
    {
        var playerNameWrapper = JsonUtility.FromJson<PlayerNameChangedResponse>(jsonData);
        if (playerNameWrapper?.data != null)
        {
            Debug.Log($"Player {playerNameWrapper.data.playerId} changed name to {playerNameWrapper.data.playerName}");
            // You can add a specific event for name changes if needed
        }
    }

    private void handleChangeNameResponse(string jsonData)
    {
        var changeNameResponse = JsonUtility.FromJson<ChangeNameResponse>(jsonData);
        if (changeNameResponse != null)
        {
            Debug.Log($"Change name response - Status: {changeNameResponse.status}, Message: {changeNameResponse.msg}");
            
            if (changeNameResponse.status == "OK" && changeNameResponse.data != null)
            {
                playerName = changeNameResponse.data.name;
                Debug.Log($"Player name successfully changed to: {playerName}");
            }
            else
            {
                Debug.LogError($"Failed to change name: {changeNameResponse.msg}");
            }
        }
    }

    private void handlePlayerDataResponse(string jsonData)
    {
        var playerDataResponse = JsonUtility.FromJson<PlayerDataResponse>(jsonData);
        if (playerDataResponse != null)
        {
            Debug.Log($"Player data response - Status: {playerDataResponse.status}, Message: {playerDataResponse.msg}");
            
            if (playerDataResponse.status == "OK" && playerDataResponse.data != null)
            {
                StorePlayerData(playerDataResponse.data);
                logConnectionInfo();
                Debug.Log("Player data successfully retrieved and updated");
            }
            else
            {
                Debug.LogError($"Failed to get player data: {playerDataResponse.msg}");
            }
        }
    }
    #endregion

    #region Helper Methods
    private void StorePlayerData(ConnectionData data)
    {
        currentPlayerData = data;
        playerId = data.id;
        playerName = data.name;
    }

    private void logConnectionInfo()
    {
        Debug.Log($"Connected with ID: {playerId}, Name: {playerName}");
        Debug.Log($"Game: {currentPlayerData.game.name} by {currentPlayerData.game.team}");
        Debug.Log($"Status: {currentPlayerData.status}");
    }

    private void triggerConnectionEvents()
    {
        MultiplayerGameEvents.triggerConnectedToServer();
        MultiplayerGameEvents.triggerPlayerConnected(playerId);
    }

    private void updateConnectedPlayersList(string[] playerIds)
    {
        MultiplayerGameEvents.triggerPlayersListCleared();
        foreach (string id in playerIds)
        {
            MultiplayerGameEvents.triggerPlayerConnected(id);
        }
    }

    private void handleDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            getConnectedPlayers();
        }
    }
    #endregion

    #region Public API
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
        Debug.Log($"Sending message: {json}");
        webSocket.To.Data(json, HTTP.Text);
    }

    public void sendPrivateMessage(string targetId, string message)
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot send private message - not connected");
            return;
        }

        var msgData = new PrivateMessageData { id = targetId, message = message };
        var serverMessage = new ServerMessage<PrivateMessageData>
        {
            @event = "send-private-message",
            data = msgData
        };

        string json = JsonUtility.ToJson(serverMessage);
        Debug.Log($"Sending private message: {json}");
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
            @event = "online-players",
            data = null
        };

        string json = JsonUtility.ToJson(getMessage);
        webSocket.To.Data(json, HTTP.Text);
    }

    public void sendReadyState(bool isReady)
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot send ready state - not connected");
            return;
        }

        var readyData = new ReadyStateData { isReady = isReady };
        var serverMessage = new ServerMessage<ReadyStateData>
        {
            @event = "send-ready-state",
            data = readyData
        };

        string json = JsonUtility.ToJson(serverMessage);
        Debug.Log($"Sending ready state: {json}");
        webSocket.To.Data(json, HTTP.Text);
    }

    public void receiveAttackFromPlayer(string attackData)
    {
        MultiplayerGameEvents.triggerPlayerReceiveAttack(attackData);
    }

    public void changePlayerName(string newName)
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot change name - not connected");
            return;
        }

        var nameData = new ChangeNameData { name = newName };
        var serverMessage = new ServerMessage<ChangeNameData>
        {
            @event = "change-name",
            data = nameData
        };

        string json = JsonUtility.ToJson(serverMessage);
        Debug.Log($"Changing player name: {json}");
        webSocket.To.Data(json, HTTP.Text);
    }

    public void getPlayerData()
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot get player data - not connected");
            return;
        }

        var serverMessage = new ServerMessage<object>
        {
            @event = "player-data",
            data = null
        };

        string json = JsonUtility.ToJson(serverMessage);
        Debug.Log($"Requesting player data: {json}");
        webSocket.To.Data(json, HTTP.Text);
    }

    public GameInfo GetGameInfo() => currentPlayerData?.game;
    public string GetPlayerStatus() => currentPlayerData?.status;
    public bool IsPlayerLoggedIn() => currentPlayerData?.status != "NO_LOGIN";
    #endregion
}
[Serializable]
public class ServerMessage<T>
{
    public string @event;
    public T data;
}

[Serializable]
public class ServerMessageBase
{
    public string @event;
}

[Serializable]
public class LoginData
{
    public string gameKey;
}

[Serializable]
public class LoginResponse
{
    public string @event;
    public string status;
    public string msg;
    public ConnectionData data;
}

#region Data Models
[Serializable]
public class ConnectionData
{
    public string id;
    public string name;
    public GameInfo game;
    public string status;
}

[Serializable]
public class GameInfo
{
    public string id;
    public string name;
    public string team;
}

[Serializable]
public class ConnectedToServerResponse
{
    public string @event;
    public string msg;
    public ConnectionData data;
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
public class PlayerConnectedResponse
{
    public string @event;
    public string msg;
    public ConnectionData data;
}

[Serializable]
public class PlayerDisconnectedResponse
{
    public string @event;
    public string msg;
    public ConnectionData data;
}

[Serializable]
public class GetConnectedPlayersResponse
{
    public string @event;
    public string status;
    public string msg;
    public string[] data;
}

[Serializable]
public class PlayerNameChangedResponse
{
    public string @event;
    public string msg;
    public PlayerNameChangedData data;
}

[Serializable]
public class PlayerNameChangedData
{
    public string playerId;
    public string playerName;
}

[Serializable]
public class ChangeNameData
{
    public string name;
}

[Serializable]
public class ChangeNameResponse
{
    public string @event;
    public string status;
    public string msg;
    public ChangeNameData data;
}

[Serializable]
public class PublicMessageResponse
{
    public string @event;
    public string msg;
    public PublicMessageData data;
}

[Serializable]
public class PublicMessageData
{
    public string playerId;
    public string playerMsg;
}

[Serializable]
public class PlayerDataResponse
{
    public string @event;
    public string status;
    public string msg;
    public ConnectionData data;
}
#endregion