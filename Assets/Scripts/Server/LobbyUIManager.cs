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

        MultiplayerGameEvents.onMatchAccept += HandleMatchAccept;
        MultiplayerGameEvents.onMatchReject += HandleMatchReject;
        MultiplayerGameEvents.onMatchAccepted += HandleMatchAccepted;



        // Configura los botones y los listeners
        acceptButton.onClick.AddListener(OnAcceptClick);
        rejectButton.onClick.AddListener(OnRejectClick);

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

    }

    public static void triggerMatchAccepted(string matchId, string matchStatus)
    {
    //    onMatchAccepted?.Invoke(matchId, matchStatus);
    }

    private void OnAcceptClick()
    {
        // Notificar que el jugador ha aceptado la solicitud de la partida
        MultiplayerGameEvents.triggerMatchAccept(matchId);  // Usamos el evento que hemos definido previamente en MultiplayerGameEvents

        // Cerrar la ventana de la solicitud de partida
        matchRequestPanel.SetActive(false);

        // Mostrar mensaje en la UI (si lo deseas)
        chatDisplay.text += $"\n<color=green>Has aceptado la solicitud para el Match {matchId}</color>";
    }

    private void OnRejectClick()
    {


        // Llamar a la función del NetworkManager para enviar el mensaje de rechazo
        NetworkManager.Instance.sendRejectMatchRequest(matchId);  // Esto envía el mensaje al servidor

        // Cerrar la ventana de la solicitud de partida (la ventana que muestra la solicitud)
        matchRequestPanel.SetActive(false);

        // Mostrar un mensaje en la UI para indicar que la solicitud ha sido rechazada
        chatDisplay.text += $"\n<color=red>La solicitud de partida {matchId} ha sido rechazada.</color>";
    }


    private void HandleMatchRejected(string playerId)
    {
        // Mostrar el mensaje en la UI del jugador que hizo la solicitud
        chatDisplay.text += $"\n<color=red>El jugador '{playerId}' ha rechazado tu solicitud de partida.</color>";
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
        //MultiplayerGameEvents.onMatchRejected -= HandleMatchRejected;
    }



}
