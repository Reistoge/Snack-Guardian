using UnityEngine;
using Netly;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

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

        string eventName = extractEventName(json);
        if (!string.IsNullOrEmpty(eventName))
        {
            handleServerData(eventName, data);
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
            //case "online-players":
              //  handleGetConnectedPlayers(jsonData);
                //break;
            case "player-name-changed":
                handlePlayerNameChanged(jsonData);
                break;
            case "change-name":
                handleChangeNameResponse(jsonData);
                break;
            case "player-data":
                handlePlayerDataResponse(jsonData);
                break;
            case "send-match-request":
                handleSendMatchRequest(jsonData);
                break;
            case "online-players":
                handleOnlinePlayers2(jsonData);
                break;
            case "match-request-received":
                HandleMatchRequestReceived(data);
                break;
            case "match-accepted":
                HandleMatchAccepted(data);
                break;
            case "match-rejected":
                HandleMatchRejected(data);
                break;
            case "accept-match":
                HandleAcceptMatchResponse(data);
                break;
            case "reject-match":
                SendMatchRejectNotification(data);
                break;
            case "connect-match":
                HandleConnectMatchResponse(data);
                break;
            case "players-ready":
                    HandlePlayersReady(data);
                break;
            case "ping-match":
                HandlePingMatchResponse(data);
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

            string desiredName = "olaolaola"; // Puedes usar un campo configurable aquí si deseas
            changePlayerName(desiredName);

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
            string name = publicMessageWrapper.data.playerName;
            string message = publicMessageWrapper.data.playerMsg;

            Debug.Log($"Received message from {name}: {message}");
            MultiplayerGameEvents.triggerChatMessageReceived(name, message); // ← Se envía el nombre, no el ID
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

    private void handleSendMatchRequest(string jsonData)
    {
        var response = JsonUtility.FromJson<SendMatchRequestResponse>(jsonData);
        if (response != null && response.status == "OK" && response.data != null)
        {
            Debug.Log($"Match request sent! Match ID: {response.data.matchId}");
            MultiplayerGameEvents.triggerMatchRequestSent(response.data.matchId);
        }   
        else
        {
            Debug.LogError("Failed to send match request or invalid response.");
        }
    }

    private void handleOnlinePlayers(string jsonData)
    {
        var response = JsonUtility.FromJson<OnlinePlayersResponse>(jsonData);
        if (response != null && response.status == "OK" && response.data != null)
        {
            Debug.Log($"Received list of {response.data.Count} online players:");
            MultiplayerGameEvents.triggerOnlinePlayersReceived(response.data);
        }
        else
        {
            Debug.LogWarning("Failed to parse online players list");
        }
    }

    private void handleOnlinePlayers2(string jsonData)
    {
        var response = JsonUtility.FromJson<OnlinePlayersResponse>(jsonData);
        if (response != null && response.status == "OK" && response.data != null)
        {
            Debug.Log($"[NetworkManager] Online players received: {response.data.Count}");

            MultiplayerGameEvents.triggerOnlinePlayersReceived(response.data);
        }
        else
        {
            Debug.LogWarning("Failed to parse online players list");
        }
    }

    private string extractEventName(string json)
    {
        try
        {
            int start = json.IndexOf("\"event\":\"") + 9;
            int end = json.IndexOf("\"", start);
            return json.Substring(start, end - start);
        }
        catch
        {
            return null;
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

    private void HandleMatchRequestReceived(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        MatchRequestReceivedEvent matchRequest = JsonUtility.FromJson<MatchRequestReceivedEvent>(json);

        // Aquí manejas el mensaje y el matchId
        Debug.Log(matchRequest.msg);

        // Llamamos a un evento en MultiplayerGameEvents para actualizar la UI o mostrar un mensaje
        MultiplayerGameEvents.triggerMatchRequestReceived(matchRequest.data.playerId, matchRequest.data.matchId);
    }

    private void HandleMatchAccepted(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        MatchAcceptedEvent matchAcceptedEvent = JsonUtility.FromJson<MatchAcceptedEvent>(json);

        // Procesar el evento
        Debug.Log($"Match accepted by {matchAcceptedEvent.data.playerId} for match {matchAcceptedEvent.data.matchId}, Status: {matchAcceptedEvent.data.matchStatus}");

        // Actualizar la UI con el estado del match
        MultiplayerGameEvents.triggerMatchAccepted(matchAcceptedEvent.data.matchId, matchAcceptedEvent.data.matchStatus);
    }



    private void HandleMatchRejected(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        MatchRejectedEvent matchRejectedEvent = JsonUtility.FromJson<MatchRejectedEvent>(json);

        // Mostrar el mensaje en la UI del jugador que hizo la solicitud
        Debug.Log($"Received match-rejected event: {matchRejectedEvent.msg}");

        // Llamamos a un evento que notificará a la interfaz del usuario que la solicitud fue rechazada
        MultiplayerGameEvents.triggerMatchRejected(matchRejectedEvent.data.playerId);
    }
    private void SendMatchRejectNotification(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        MatchRejectedEvent matchRejectedEvent = JsonUtility.FromJson<MatchRejectedEvent>(json);

        // Aquí manejamos el evento y creamos la notificación para el jugador
        Debug.Log($"La solicitud de partida ha sido rechazada");

        // Enviar una notificación o evento al jugador que hizo la solicitud
    }
    private void HandleRejectMatchResponse(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        Debug.Log($"Raw reject-match response: {json}");

        RejectMatchResponse response = JsonUtility.FromJson<RejectMatchResponse>(json);

        if (response == null)
        {
            Debug.LogError("Failed to parse reject-match response");
            return;
        }

        Debug.Log($"Reject-match response - Status: {response.status}, Message: {response.msg}");

        if (response.status == "OK" && response.data != null)
        {
            Debug.Log($"Match rejection processed successfully for player: {response.data.playerId}");

            // Disparar evento para notificar que el rechazo fue procesado
            MultiplayerGameEvents.triggerMatchRejectionReceived(response.data.playerId, response.msg);
        }
        else
        {
            Debug.LogError($"Error processing match rejection: {response.msg}");
        }
    }

    private void HandleAcceptMatchResponse(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        Debug.Log($"Raw accept-match response: {json}");

        AcceptMatchResponse response = JsonUtility.FromJson<AcceptMatchResponse>(json);

        if (response == null)
        {
            Debug.LogError("Failed to parse accept-match response");
            return;
        }

        Debug.Log($"Accept-match response - Status: {response.status}, Message: {response.msg}");

        if (response.status == "ERROR")
        {
            Debug.LogWarning($"Error accepting match: {response.msg}");

            string playerStatus = response.data?.playerStatus ?? "UNKNOWN";
            Debug.Log($"Player status updated to: {playerStatus}");

            // Actualizar el estado del jugador local si es necesario
            if (currentPlayerData != null)
            {
                currentPlayerData.status = playerStatus;
            }

            // Disparar evento de error
            MultiplayerGameEvents.triggerMatchAcceptanceError(response.msg, playerStatus);
        }
        else if (response.status == "OK")
        {
            Debug.Log($"Match accepted successfully: {response.msg}");

            // Disparar evento de éxito
            MultiplayerGameEvents.triggerMatchAcceptanceSuccess(response.msg);
        }
        else
        {
            Debug.LogWarning($"Unknown accept-match response status: {response.status}");
        }
    }
    private void HandleConnectMatchResponse(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        Debug.Log($"Raw connect-match response: {json}");

        ConnectMatchResponse response = JsonUtility.FromJson<ConnectMatchResponse>(json);

        if (response == null)
        {
            Debug.LogError("Failed to parse connect-match response");
            return;
        }

        Debug.Log($"Connect-match response - Status: {response.status}, Message: {response.msg}");

        if (response.status == "OK" && response.data != null)
        {
            Debug.Log($"Successfully connected to match: {response.data.matchId}");
            MultiplayerGameEvents.triggerConnectMatchSuccess(response.data.matchId, response.msg);
        }
        else
        {
            Debug.LogError($"Error connecting to match: {response.msg}");
            MultiplayerGameEvents.triggerConnectMatchError(response.msg);
        }
    }

    public void sendConnectMatchRequest()
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot send connect match request - not connected");
            return;
        }

        var connectMatchRequest = new ConnectMatchRequest();

        string json = JsonUtility.ToJson(connectMatchRequest);
        Debug.Log($"Sending connect match request: {json}");

        webSocket.To.Data(json, HTTP.Text);

        // Disparar evento local
        MultiplayerGameEvents.triggerConnectMatchSent();
    }

    // Implementación de HandlePlayersReady usando System.Text.Encoding y JsonUtility
    private void HandlePlayersReady(byte[] data)
    {
        // 1. Convertir el byte[] a string usando UTF8 encoding
        string jsonString = Encoding.UTF8.GetString(data);

        // 2. Deserializar el string JSON a tu objeto PlayersReadyEvent
        PlayersReadyEvent playersReadyEvent = JsonUtility.FromJson<PlayersReadyEvent>(jsonString);

        Debug.Log($"[NetworkManager] Players Ready Event Received: {playersReadyEvent.msg}, Match ID: {playersReadyEvent.data.matchId}");

        // Activa el evento en MultiplayerGameEvents para que LobbyUIManager lo pueda escuchar
        MultiplayerGameEvents.triggerPlayersReady(playersReadyEvent.data.matchId, playersReadyEvent.msg);

        // Si debes enviar "ping-match" automáticamente, descomenta la siguiente línea:
        // sendPingMatchRequest(playersReadyEvent.data.matchId);
    }

    public void sendAcceptMatchRequest()
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot send accept match request - not connected");
            return;
        }

        // Crear el mensaje de aceptación (solo necesita el evento)
        var acceptMatchRequest = new AcceptMatchRequest();

        string json = JsonUtility.ToJson(acceptMatchRequest);
        Debug.Log($"Sending accept match request: {json}");

        webSocket.To.Data(json, HTTP.Text);
    
        // Disparar evento local
        MultiplayerGameEvents.triggerMatchAcceptanceSent();
    }

    public void sendAcceptMatchRequest(string matchId)
    {
        // Ahora simplemente llama al método sin parámetros
        sendAcceptMatchRequest();
    }

    public void sendRejectMatchRequest(string matchId)
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot send reject match request - not connected");
            return;
        }

        // Crear el mensaje de rechazo (solo necesita el evento)
        var rejectMatchRequest = new RejectMatchRequest();

        string json = JsonUtility.ToJson(rejectMatchRequest);
        Debug.Log($"Sending reject match request: {json}");

        webSocket.To.Data(json, HTTP.Text);
    }
    public void sendPingMatchRequest(string matchId = null) // matchId es opcional si el servidor no lo requiere en la solicitud
    {
        var pingMatchRequest = new PingMatchRequest();

        var json = JsonUtility.ToJson(pingMatchRequest);
        Debug.Log($"[NetworkManager] Sending ping-match request: {json}");

        webSocket.To.Data(json, HTTP.Text);

        MultiplayerGameEvents.triggerPingMatchSent(); // Activa el evento para la UI u otros sistemas
    }

    // Crea este nuevo método para manejar la respuesta del servidor al ping-match
    private void HandlePingMatchResponse(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        PingMatchResponse pingMatchResponse = JsonUtility.FromJson<PingMatchResponse>(jsonString);

        if (pingMatchResponse.status == "OK")
        {
            Debug.Log($"[NetworkManager] Ping Match successful: {pingMatchResponse.msg}, Match ID: {pingMatchResponse.data.matchId}");
            MultiplayerGameEvents.triggerPingMatchSuccess(pingMatchResponse.data.matchId, pingMatchResponse.msg);
        }
        else
        {
            Debug.LogError($"[NetworkManager] Ping Match failed: {pingMatchResponse.msg}");
            MultiplayerGameEvents.triggerPingMatchError(pingMatchResponse.msg);
        }
    }

    public class RejectMatchRequest
    {
        public string @event = "reject-match";
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

    /// AQUI VAN LAS ACCIONES RESPECTO A LOBBY-EVENTS

    public void sendMatchRequest(string targetPlayerId)
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot send match request - not connected");
            return;
        }

        var matchRequest = new MatchRequestData { playerId = targetPlayerId };
        var serverMessage = new ServerMessage<MatchRequestData>
        {
            @event = "send-match-request",
            data = matchRequest
        };

        string json = JsonUtility.ToJson(serverMessage);
        Debug.Log($"Sending match request to player {targetPlayerId}: {json}");
        webSocket.To.Data(json, HTTP.Text);
    }

    public void requestOnlinePlayers()
    {
        if (!webSocket.IsOpened)
        {
            Debug.LogWarning("Cannot request online players - not connected");
            return;
        }

        var serverMessage = new ServerMessage<object>
        {
            @event = "online-players",
            data = null
        };

        string json = JsonUtility.ToJson(serverMessage);
        Debug.Log($"Requesting online players: {json}");
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
    public string id;
    public string name;
    public GameInfo game;
    public string status;
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
    public string playerName;
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

//playerDatas
[Serializable]
public class OnlinePlayersResponse
{
    public string @event;
    public string status;
    public string msg;
    public List<ConnectionData> data;
}

//para los MATCH

[Serializable]
public class MatchRequestData
{
    public string playerId; // jugador al que se envía la solicitud
    public string matchId;
}

[Serializable]
public class SendMatchRequestResponse
{
    public string @event;
    public string status;
    public string msg;
    public MatchIdData data;
}

public class MatchRequestReceivedEvent
{
    public string eventType;
    public string msg;
    public MatchRequestData data;
}

[Serializable]
public class MatchIdData
{
    public string matchId;
}

[Serializable]
public class MatchAcceptedData
{
    public string playerId;
    public string matchId;
    public string matchStatus; // "WAITING_PLAYERS", "IN_PROGRESS", etc.
}

[Serializable]
public class MatchAcceptedEvent
{
    public string @event;
    public string msg;
    public MatchAcceptedData data;
}

[Serializable]
public class MatchRejectedData
{
    public string playerId;  // Jugador que ha rechazado la solicitud
}

[Serializable]
public class MatchRejectedEvent
{
    public string @event;
    public string msg;
    public MatchRejectedData data;
}

[Serializable]
public class AcceptMatchResponseData
{
    public string playerStatus;
}

[Serializable]
public class AcceptMatchResponse
{
    public string @event;
    public string status;
    public string msg;
    public AcceptMatchResponseData data;
}

[Serializable]
public class RejectMatchResponseData
{
    public string playerId;  // ID del jugador que rechazó la solicitud
}

[Serializable]
public class RejectMatchResponse
{
    public string @event;
    public string status;
    public string msg;
    public RejectMatchResponseData data;
}

[Serializable]
public class AcceptMatchRequest
{
    public string @event = "accept-match";
}

[Serializable]
public class ConnectMatchRequest
{
    public string @event = "connect-match";
}

[Serializable]
public class ConnectMatchResponseData
{
    public string matchId;
}

[Serializable]
public class ConnectMatchResponse
{
    public string @event;
    public string status;
    public string msg;
    public ConnectMatchResponseData data;
}

[Serializable]
public class PlayersReadyData
{
    public string matchId;
}

[Serializable]
public class PlayersReadyEvent
{
    public string @event;
    public string msg;
    public PlayersReadyData data;
}

[Serializable]
public class PingMatchRequest
{
    public string @event = "ping-match";
}

[Serializable]
public class PingMatchResponseData
{
    public string matchId;
}

[Serializable]
public class PingMatchResponse
{
    public string @event;
    public string status;
    public string msg;
    public PingMatchResponseData data;
}
#endregion
