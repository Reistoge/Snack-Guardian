using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using System.Drawing;
using JetBrains.Annotations;
using System.Threading.Tasks;

public class Connection : MonoBehaviour
{
    WebSocket websocket;
    [SerializeField] string url = "ws://ucn-game-server.martux.cl:4010/";
    [SerializeField] string playerId = "";


    // Start is called before the first frame update
    async void Start()
    {
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(bytes);
            // handleMessage(bytes);

            // getting the message as a string
            // var message = System.Text.Encoding.UTF8.GetString(bytes);
            // Debug.Log("OnMessage! " + message);
        };

        // Keep sending messages at every 0.3s
        InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        await websocket.Connect();
    }

    private void handleMessage(byte[] bytes)
    {
        print("datos recibidos");
        string json = System.Text.Encoding.UTF8.GetString(bytes);
        Debug.Log($"Received raw JSON data: {json}");

        // Extraemos solo el campo 'event'
        var baseEvent = JsonUtility.FromJson<ServerMessageBase>(json);
        if (baseEvent != null && !string.IsNullOrEmpty(baseEvent.@event))
        {
            handleServerEvent(baseEvent.@event, bytes);
        }
        else
        {
            Debug.LogWarning("No 'event' field found in message JSON");
        }
    }

    private void handleServerEvent(string @event, byte[] bytes)
    {
        string jsonData = System.Text.Encoding.UTF8.GetString(bytes);
        Debug.Log($"Handling event '{@event}' with data: {jsonData}");


    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
        if (Input.GetKeyDown(KeyCode.P))
        {
            getConnectedPlayers();

        }
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText("plain text message");
        }
    }
    async void getConnectedPlayers()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            // await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText("get-connected-players");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}