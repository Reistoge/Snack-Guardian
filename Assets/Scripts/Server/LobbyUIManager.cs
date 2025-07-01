using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatDisplay; // Change to TextMeshProUGUI
    [SerializeField] private TextMeshProUGUI playerList;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button sendMessageButton;
    [SerializeField] private Transform dynamicPlayerListContainer;
    [SerializeField] private Button refreshPlayersButton;

    ///la ventana del matchup
    ///
    [SerializeField] private GameObject onlinePlayersWindow;

    [SerializeField] private Transform playerListContainer; // El 'Content' del Scroll View
    [SerializeField] private GameObject playerRowPrefab;   // El prefab que acabas de crear
    [SerializeField] private GameObject matchRequestPanel;  // El Panel con la ventana de la solicitud
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private TextMeshProUGUI matchInfoText;  // Para mostrar el mensaje de la solicitud
    [SerializeField] private Button pingButton;  // Botón para enviar el ping
    [SerializeField] private GameObject playersReadyPanel; // ¡Arrastra tu panel desde el Inspector de Unity!
    [SerializeField] private TextMeshProUGUI playersReadyInfoText; // Para mostrar el mensaje "Both players are ready..."


    private string matchId;
    private string playerId;

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

        refreshPlayersButton.onClick.AddListener(requestOnlinePlayers);

        // Subscribe to all events
        MultiplayerGameEvents.onConnectedToServer += handleConnectedToServer;
        MultiplayerGameEvents.onChatMessageReceived += handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected += handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected += handlePlayerDisconnected;
        MultiplayerGameEvents.onPlayersListCleared += handlePlayersListCleared;
        MultiplayerGameEvents.onMatchRequestSent += handleMatchRequestSent;
        MultiplayerGameEvents.onOnlinePlayersReceived += displayInteractivePlayerList;
        NetworkManager.Instance.requestOnlinePlayers();
        MultiplayerGameEvents.onOnlinePlayersReceived += handleOnlinePlayersReceived;
        /////
        MultiplayerGameEvents.onMatchRequestReceived += HandleMatchRequestReceived;

        NetworkManager.Instance.requestOnlinePlayers();
        MultiplayerGameEvents.onPlayerConnected += (_) => NetworkManager.Instance.requestOnlinePlayers();
        MultiplayerGameEvents.onPlayerDisconnected += (_) => NetworkManager.Instance.requestOnlinePlayers();

        ///Aqui falta crear esto, para poder mostrarlo en el juego
        //MultiplayerGameEvents.onMatchAccepted += HandleMatchAccepted;
        //MultiplayerGameEvents.onMatchRejected += HandleMatchRejected;
        //MultiplayerGameEvents.onMatchAccepted += HandleMatchAccepted;
        //

        MultiplayerGameEvents.onMatchRejectionReceived += HandleMatchRejectionReceived;


        MultiplayerGameEvents.onMatchAccept += HandleMatchAccept;
        MultiplayerGameEvents.onMatchReject += HandleMatchReject;
        MultiplayerGameEvents.onMatchAccepted += HandleMatchAccepted;

        MultiplayerGameEvents.onMatchAcceptanceSent += HandleMatchAcceptanceSent;
        MultiplayerGameEvents.onMatchAcceptanceError += HandleMatchAcceptanceError;
        MultiplayerGameEvents.onMatchAcceptanceSuccess += HandleMatchAcceptanceSuccess;

        MultiplayerGameEvents.onConnectMatchSent += HandleConnectMatchSent;
        MultiplayerGameEvents.onConnectMatchSuccess += HandleConnectMatchSuccess;
        MultiplayerGameEvents.onConnectMatchError += HandleConnectMatchError;

        MultiplayerGameEvents.onPingMatchSent += HandlePingMatchSent;
        MultiplayerGameEvents.onPingMatchSuccess += HandlePingMatchSuccess;
        MultiplayerGameEvents.onPingMatchError += HandlePingMatchError;

        MultiplayerGameEvents.onPlayersReady += HandlePlayersReady;
        MultiplayerGameEvents.onMatchStart += HandleMatchStartEvent;

        MultiplayerGameEvents.onFinishGameSent += HandleFinishGameSent;
        MultiplayerGameEvents.onFinishGameSuccess += HandleFinishGameSuccess;
        MultiplayerGameEvents.onFinishGameError += HandleFinishGameError;
        //MultiplayerGameEvents.onPlayersReadyResponse += HandlePlayersReadyResponse;

        MultiplayerGameEvents.onQuitMatchSent += HandleQuitMatchSent;
        MultiplayerGameEvents.onQuitMatchSuccess += HandleQuitMatchSuccess;
        MultiplayerGameEvents.onQuitMatchError += HandleQuitMatchError;

        // Configura los botones y los listeners
        acceptButton.onClick.AddListener(OnAcceptClick);
        rejectButton.onClick.AddListener(OnRejectClick);


        pingButton.onClick.AddListener(OnPingButtonClicked); // Lo mantenemos oculto inicialmente

        // Inicialmente desactivar la ventana
        matchRequestPanel.SetActive(false);

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

    // Update the handleChatMessage method to show better player identification:
    private void handleChatMessage(string playerId, string message)
    {
        Debug.Log($"Handling chat message: {playerId}: {message}");

        if (!this || !chatDisplay) return;

        // Check if it's your own message by comparing with your player name or ID
        string displayName;
        if (playerId == NetworkManager.Instance.PlayerName || playerId == NetworkManager.Instance.PlayerId)
        {
            displayName = "<color=#4CAF50>You</color>";
        }
        else
        {
            displayName = $"<color=#2196F3>{playerId}</color>";
        }

        string formattedMessage = $"\n{displayName}: {message}";
        chatDisplay.text += formattedMessage;

        // Force UI update and scroll
        Canvas.ForceUpdateCanvases();
  
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

    private void handleMatchRequestSent(string matchId)
    {
        chatDisplay.text += $"\n<color=cyan>Solicitud de partida enviada. ID: {matchId}</color>";
    }

    private void displayInteractivePlayerList(List<ConnectionData> players)
    {
        Debug.Log($"[LobbyUI] Mostrando {players.Count} jugadores en lista interactiva");

        foreach (Transform child in dynamicPlayerListContainer)
            Destroy(child.gameObject);

        foreach (var player in players)
        {
         //   if (player.id == NetworkManager.Instance.PlayerId)
           //     continue;

            GameObject buttonObj = Instantiate(playerRowPrefab, dynamicPlayerListContainer);
            buttonObj.SetActive(true);

            // Asigna textos específicos por nombre
            foreach (var t in buttonObj.GetComponentsInChildren<TextMeshProUGUI>())
            {
                if (t.name == "NameText") t.text = player.name;
                else if (t.name == "StatusText") t.text = player.status;
                else if (t.name == "GameText") t.text = $"{player.game.name} - {player.game.team}";
            }

            // Configura el botón correctamente
            var matchButton = buttonObj.transform.Find("MatchButton").GetComponent<Button>();
            matchButton.onClick.RemoveAllListeners();
            matchButton.onClick.AddListener(() =>
            {
                NetworkManager.Instance.sendMatchRequest(player.id);
                chatDisplay.text += $"\n<color=cyan>Solicitud enviada a {player.name}</color>";
            });
            matchButton.interactable = (player.status == "AVAILABLE");

            Debug.Log($"Agregado: {player.name} - {player.status}");
        }
    }

    private void requestOnlinePlayers()
    {
        NetworkManager.Instance.requestOnlinePlayers();
        chatDisplay.text += "\n<color=gray>Solicitando jugadores online...</color>";
    }

    private void handleOnlinePlayersReceived(List<ConnectionData> players)
    {


        playerList.text = "Jugadores en línea:\n";

        foreach (var p in players)
        {
            playerList.text += $"{p.name} - {p.status}\n";
        }
    }

    private void HandleMatchRequestReceived(string playerId, string matchId)
    {
        this.matchId = matchId;
        this.playerId = playerId;

        // Mostrar el mensaje en la UI
        matchInfoText.text = $"Solicitud de partida recibida de {playerId}. ¿Aceptar?";

        // Activar la ventana
        matchRequestPanel.SetActive(true);
    }

    private void HandleMatchAccept(string matchId)
    {
        // El jugador ha aceptado la solicitud, actualizamos la UI
        Debug.Log($"Jugador ha aceptado la solicitud para el Match {matchId}");
        chatDisplay.text += $"\n<color=green>Has aceptado la solicitud para el Match {matchId}</color>";
    }

    private void HandleMatchReject(string matchId)
    {
        // Ahora, actualizamos el estado del jugador a "AVAILABLE"

        // Eliminar la ventana de solicitud
        Debug.Log($"La solicitud de partida {matchId} ha sido rechazada.");
        chatDisplay.text += $"\n<color=red>La solicitud de partida {matchId} ha sido rechazada.</color>";
        //RemoveMatchRequestWindow(matchId);
    }

    private void HandleMatchAccepted(string matchId, string matchStatus)
    {
        // El servidor ha aceptado la solicitud, actualizamos la UI

        Debug.Log($"[LobbyUIManager] ¡Partida aceptada por el oponente! ID de partida: {matchId}, Estado: {matchStatus}");
        chatDisplay.text += $"\n<color=green>¡Tu solicitud de partida ha sido ACEPTADA! ID de partida: {matchId}. Estado: {matchStatus}</color>";
        this.matchId = matchId; // Guarda el ID de la partida
        NetworkManager.Instance.sendConnectMatchRequest();
    }

    public static void triggerMatchAccepted(string matchId, string matchStatus)
    {
    //onMatchAccepted?.Invoke(matchId, matchStatus);
    }

    private void OnAcceptClick()
    {
        // Notificar que el jugador ha aceptado la solicitud de la partida
        NetworkManager.Instance.sendAcceptMatchRequest();

        // Cerrar la ventana de la solicitud de partida
        matchRequestPanel.SetActive(false);

        // Mostrar mensaje local en la UI (el mensaje del servidor llegará después)
        chatDisplay.text += $"\n<color=cyan>Procesando aceptación de partida...</color>";

        StartCoroutine(ConnectToMatchAfterDelay());
    }

    private System.Collections.IEnumerator ConnectToMatchAfterDelay()
    {
        // Esperar un poco para que se procese la aceptación
        yield return new WaitForSeconds(1f);
        NetworkManager.Instance.sendConnectMatchRequest();
    }

    private void OnRejectClick()
    {
        // Llamar a la función del NetworkManager para enviar el mensaje de rechazo
        NetworkManager.Instance.sendRejectMatchRequest(matchId);

        // Cerrar la ventana de la solicitud de partida
        matchRequestPanel.SetActive(false);

        // Mostrar un mensaje local en la UI
        chatDisplay.text += $"\n<color=red>Has rechazado la solicitud de partida {matchId}.</color>";
    }



    private void HandleMatchRejected(string playerId)
    {
        // Mostrar el mensaje en la UI del jugador que hizo la solicitud
        chatDisplay.text += $"\n<color=red>El jugador '{playerId}' ha rechazado tu solicitud de partida.</color>";
    }

    private void HandleMatchRejectionReceived(string playerId, string message)
    {
        Debug.Log($"Match rejection received: {message}");
        chatDisplay.text += $"\n<color=orange>{message}</color>";
    }

    private void HandleMatchAcceptanceSent()
    {
        Debug.Log("Match acceptance sent to server");
        chatDisplay.text += $"\n<color=cyan>Enviando aceptación de partida...</color>";
    }

    private void HandleMatchAcceptanceError(string message, string playerStatus)
    {
        Debug.Log($"Match acceptance error: {message}, Player status: {playerStatus}");
        chatDisplay.text += $"\n<color=red>Error: {message}</color>";

    }

    private void HandleMatchAcceptanceSuccess(string message)
    {
        Debug.Log($"Match acceptance success: {message}");
        chatDisplay.text += $"\n<color=green>¡Partida aceptada exitosamente! {message}</color>";
    }

    private void HandleConnectMatchSent()
    {
        Debug.Log("Connect match request sent to server");
        chatDisplay.text += $"\n<color=cyan>Conectando a la partida...</color>";
    }

    private void HandleConnectMatchSuccess(string matchId, string message)
    {
        Debug.Log($"Connect match success: {message}, Match ID: {matchId}");
        chatDisplay.text += $"\n<color=green>¡Conectado a la partida exitosamente!</color>";
        chatDisplay.text += $"\n<color=green>Match ID: {matchId}</color>";
        chatDisplay.text += $"\n<color=yellow>{message}</color>";
    }

    private void HandleConnectMatchError(string errorMessage)
    {
        Debug.Log($"Connect match error: {errorMessage}");
        chatDisplay.text += $"\n<color=red>Error al conectar a la partida: {errorMessage}</color>";
    }

    private void HandlePingMatchSent()
    {
        chatDisplay.text += "\n<color=yellow>Enviando ping de sincronización...</color>";
    }

    private void HandlePingMatchSuccess(string matchId, string message)
    {
        chatDisplay.text += $"\n<color=green>Sincronización de partida exitosa! Match ID: {matchId}. Mensaje: {message}</color>";
        // Una vez que el ping es exitoso, podrías iniciar la cuenta atrás o la carga de la escena del juego
        // También podrías ocultar el panel de "players-ready" si aún está visible
        if (playersReadyPanel != null && playersReadyPanel.activeSelf)
        {
            playersReadyPanel.SetActive(false);
        }
    }

    public void EnablePingButton(string matchId)
    {
        // Aquí habilitas el botón y cambias su texto si es necesario
        pingButton.gameObject.SetActive(true);
        pingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enviar Ping";  // Cambia el texto si es necesario
    }

    private void OnPingButtonClicked()
    {
        // Asegúrate de tener el matchId disponible aquí. Podrías guardarlo cuando recibes 'match-accepted'
        // o 'players-ready'. Por ejemplo:

        NetworkManager.Instance.sendPingMatchRequest();
    }


    private void HandlePingMatchError(string errorMessage)
    {
        Debug.Log($"Ping match error: {errorMessage}");
        chatDisplay.text += $"\n<color=red>Error al enviar ping: {errorMessage}</color>";
    }

    private void HandlePlayersReadyResponse(string matchId)
    {
        // Habilitar el botón PingMatch
        EnablePingButton(matchId);

        // Mostrar un mensaje en el chat que ambos jugadores están listos
        chatDisplay.text += "\n<color=green>Ambos jugadores están listos para comenzar la partida.</color>";
    }

    private void HandlePlayersReady(string matchId, string message)
    {
        Debug.Log($"[LobbyUIManager] Recibido evento Players Ready. Match ID: {matchId}, Mensaje: {message}");

        // Activa el panel de la ventana
        if (playersReadyPanel != null)
        {
            playersReadyPanel.SetActive(true);
            if (playersReadyInfoText != null)
            {
                playersReadyInfoText.text = $"Jugadores listos!, clickea cuando quieras";
            }
        }
        else
        {
            Debug.LogWarning("[LobbyUIManager] playersReadyPanel no está asignado en el Inspector.");
            chatDisplay.text += $"\n<color=orange>¡Jugadores Listos para empezar! ID: {matchId}. Mensaje: {message}</color>";
        }

        // Aquí podrías añadir un botón en tu UI para que el jugador haga click y se envíe el "ping-match"
        // o enviarlo automáticamente si ese es el comportamiento deseado.
        // Si lo envías automáticamente, asegúrate de que el NetworkManager ya lo hace al recibir 'players-ready'.
        // Si quieres un botón en la UI, configura su listener para llamar a NetworkManager.Instance.sendPingMatchRequest(matchId);
    }

    private void HandleMatchStartEvent(string matchId, string message)
    {
        Debug.Log($"[LobbyUIManager] ¡Partida lista para iniciar! Cargando escena de juego. Match ID: {matchId}");
        chatDisplay.text += $"\n<color=cyan>¡LA PARTIDA VA A COMENZAR! ID: {matchId}. {message}</color>";

        // Llama a GameManager para cambiar la escena
        if (GameManager.Instance != null)
        {
            GameManager.Instance.loadScene("GameScene"); // Llama a tu método para cargar la escena
        }
        else
        {
            Debug.LogError("[LobbyUIManager] GameManager.Instance no está disponible. No se puede cargar la escena.");
        }

        // Opcional: Oculta cualquier UI del lobby/pre-partida
        if (playersReadyPanel != null && playersReadyPanel.activeSelf)
        {
            playersReadyPanel.SetActive(false);
        }
        // Puedes también desactivar todo el LobbyUIManager si no lo necesitas en la nueva escena
        // gameObject.SetActive(false);
    }

    private void HandleFinishGameSent()
    {
        chatDisplay.text += "\n<color=yellow>Enviando solicitud de fin de partida...</color>";
    }

    private void HandleFinishGameSuccess(string matchId, string message)
    {
        Debug.Log($"[LobbyUIManager] ¡Partida Finalizada! ID: {matchId}, Mensaje: {message}");
        chatDisplay.text += $"\n<color=purple>¡PARTIDA TERMINADA! ID: {matchId}. {message}</color>";

        // Aquí puedes:
        // 1. Mostrar una pantalla de "Game Over" o "Resultados".
        // 2. Mostrar el ganador (si los datos del evento lo incluyen).
        // 3. Ofrecer opciones para volver al lobby, jugar de nuevo, etc.
        // 4. Volver al lobby automáticamente:
        // if (GameManager.Instance != null)

        //     GameManager.Instance.loadScene("LobbySceneName"); // Reemplaza con el nombre de tu escena de lobby

    }

    private void HandleFinishGameError(string message)
    {
        Debug.LogError($"[LobbyUIManager] Error al finalizar la partida: {message}");
        chatDisplay.text += $"\n<color=red>Error al finalizar la partida: {message}</color>";
    }

    // Manejador cuando se envía la solicitud de quit-match
    private void HandleQuitMatchSent()
    {
        chatDisplay.text += "\n<color=yellow>Enviando solicitud para salir de la partida...</color>";
        // Opcional: Desactivar botón de "salir de partida" para evitar spam
    }

    // Manejador cuando el servidor confirma que has salido de la partida
    private void HandleQuitMatchSuccess(string playerStatus, string message)
    {
        Debug.Log($"[LobbyUIManager] Has salido de la partida. Estado: {playerStatus}. Mensaje: {message}");
        chatDisplay.text += $"\n<color=blue>Has salido de la partida: {message}</color>";

        // Aquí es donde típicamente regresarías al lobby o a la pantalla principal
        if (GameManager.Instance != null)
        {
            // Asume que "LobbyScene" es el nombre de tu escena de lobby
            GameManager.Instance.loadScene("Multiplayer");
        }
        else
        {
            Debug.LogError("[LobbyUIManager] GameManager.Instance no disponible para cargar la escena del lobby.");
        }

        // Opcional: Actualizar el estado del jugador si tu UI muestra eso
        // Por ejemplo, si tienes un indicador de "disponible" vs "en partida"
        // currentPlayerData.Status = playerStatus; // Si currentPlayerData es accesible
    }

    // Manejador si hay un error al intentar salir de la partida
    private void HandleQuitMatchError(string message)
    {
        Debug.LogError($"[LobbyUIManager] Error al salir de la partida: {message}");
        chatDisplay.text += $"\n<color=red>Error al salir de la partida: {message}</color>";
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events
        MultiplayerGameEvents.onConnectedToServer -= handleConnectedToServer;
        MultiplayerGameEvents.onChatMessageReceived -= handleChatMessage;
        MultiplayerGameEvents.onPlayerConnected -= handlePlayerConnected;
        MultiplayerGameEvents.onPlayerDisconnected -= handlePlayerDisconnected;
        MultiplayerGameEvents.onPlayersListCleared -= handlePlayersListCleared;
        MultiplayerGameEvents.onMatchRequestSent -= handleMatchRequestSent;
        MultiplayerGameEvents.onOnlinePlayersReceived -= displayInteractivePlayerList;
        MultiplayerGameEvents.onMatchRequestReceived -= HandleMatchRequestReceived;
        MultiplayerGameEvents.onMatchRejected -= HandleMatchRejected;
        MultiplayerGameEvents.onMatchReject -= HandleMatchReject;

        MultiplayerGameEvents.onMatchAcceptanceSent -= HandleMatchAcceptanceSent;
        MultiplayerGameEvents.onMatchAcceptanceError -= HandleMatchAcceptanceError;
        MultiplayerGameEvents.onMatchAcceptanceSuccess -= HandleMatchAcceptanceSuccess;

        MultiplayerGameEvents.onConnectMatchSent -= HandleConnectMatchSent;
        MultiplayerGameEvents.onConnectMatchSuccess -= HandleConnectMatchSuccess;
        MultiplayerGameEvents.onConnectMatchError -= HandleConnectMatchError;

        // MultiplayerGameEvents.onPingMatchSent -= HandlePingMatchSent;
        // MultiplayerGameEvents.onPingMatchSuccess -= HandlePingMatchSuccess;
        // MultiplayerGameEvents.onPingMatchError -= HandlePingMatchError;

        MultiplayerGameEvents.onMatchRejectionReceived -= HandleMatchRejectionReceived;
        //MultiplayerGameEvents.onMatchRejected -= HandleMatchRejected;

        MultiplayerGameEvents.onPingMatchSent -= HandlePingMatchSent;
        MultiplayerGameEvents.onPingMatchSuccess -= HandlePingMatchSuccess;
        MultiplayerGameEvents.onPingMatchError -= HandlePingMatchError;
        MultiplayerGameEvents.onMatchStart -= HandleMatchStartEvent;

        MultiplayerGameEvents.onFinishGameSent -= HandleFinishGameSent;
        MultiplayerGameEvents.onFinishGameSuccess -= HandleFinishGameSuccess;
        MultiplayerGameEvents.onFinishGameError -= HandleFinishGameError;

        MultiplayerGameEvents.onQuitMatchSent -= HandleQuitMatchSent;
        MultiplayerGameEvents.onQuitMatchSuccess -= HandleQuitMatchSuccess;
        MultiplayerGameEvents.onQuitMatchError -= HandleQuitMatchError;

        // Remueve el listener del botón de ping
        if (pingButton != null)
        {
            pingButton.onClick.RemoveListener(OnPingButtonClicked);
        }
    }



}
