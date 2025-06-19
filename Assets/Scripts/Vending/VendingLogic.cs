using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(100)] // initialization after TrayManager and TraySpawner
public class VendingLogic : MonoBehaviour
{
    [Header("Vending Settings")]
    [SerializeField] private float timeBetweenOrders = 2f;
    [SerializeField] private float timeBetweenSnacks = 1f;
    [SerializeField] private int minTraysPerOrder = 2;
    [SerializeField] private int maxTraysPerOrder = 4;

    [SerializeField] float ordersCount = 0;
    public static event Action trayRemoved;
    public static event Action onOrderComplete;
    public static event Action onOrderStarted;
    public static VendingLogic instance;

    private Queue<ITray> currentOrder = new Queue<ITray>();
    private bool isProcessingOrder = false;
    private Coroutine orderRoutine;

    public float OrdersCount { get => ordersCount; set => ordersCount = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public Queue<ITray> getCurrentOrder()
    {
        return currentOrder;
    }

    // Ensure this script runs after TrayManager has initialized
    void OnEnable()
    {
        TrayManager.onTraysRegistered += OnTraysRegistered;
        GameEvents.onGameOver += stopOrderProcessing;


    }
    void OnDisable()
    {
        TrayManager.onTraysRegistered -= OnTraysRegistered;
        GameEvents.onGameOver -= stopOrderProcessing;
    }
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.S)) // Changed from GetKey to GetKeyDown
        // {
        //     stopOrderProcessing();
        //     print($"Order processing stopped : {currentOrder.Count}");
        // }
        //if (Input.GetKeyDown(KeyCode.R)) // Changed from GetKey to GetKeyDown
        // {
        //     restartOrderProcessing();
        //     print($"Order processing restarted : {currentOrder.Count}");
        // }
        if (Input.GetKeyDown(KeyCode.E)) // Changed from GetKey to GetKeyDown
        {
            List<string> list = new List<string> { "A", "B", "C", "D" };
            TrayManager.instance.addRocksOnTrays(list[Random.Range(0, list.Count)].ToCharArray()[0]);
        }
        if (Input.GetKey(KeyCode.I))
        {
            Time.timeScale = 10f;
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            Time.timeScale = 1f;
        }

    }

    private void OnTraysRegistered()
    {
        // Debug.Log("Trays registered, starting new order");
        // Add small delay to ensure trays are fully initialized
        StartCoroutine(delayedStartOrder());
    }

    private IEnumerator delayedStartOrder()
    {
        yield return new WaitForSeconds(0.5f); // Give more time for initialization

        List<ITray> trays = getAvailableTrays();
        if (trays.Count == 0)
        {
            Debug.LogError("No trays available after initialization!");
            yield break;
        }

        // Verify trays are properly initialized
        bool allTraysValid = trays.All(t =>
            t != null &&
            t?.hasSnacksAvailable() == true
        );

        if (!allTraysValid)
        {
            Debug.LogError("Some trays are not properly initialized!");
            yield break;
        }
        startNewOrder();
    }
    IEnumerator waitForOrderCompletion()
    {
        UIGameNotifications.instance.playOrderCompletedNotification();
        yield return new WaitForSeconds(5);
        startNewOrder();
    }
    private void startNewOrder()
    {
        if (isProcessingOrder) return;

        // Get available trays (those with snacks)
        List<ITray> availableTrays = getAvailableTrays();

        if (availableTrays.Count == 0)
        {
            Debug.Log("No trays available - Vending complete");
            GameEvents.triggerVendingComplete();
            
            StartCoroutine(waitForOrderCompletion());
            
            return;
        }


        // Clear previous order
        currentOrder.Clear();

        // Determine order size
        int orderSize = Mathf.Min(
            Random.Range(minTraysPerOrder, maxTraysPerOrder + 1),
            availableTrays.Count
        );

        // Randomly select trays for the order
        for (int i = 0; i < orderSize; i++)
        {
            int randomIndex = Random.Range(0, availableTrays.Count);
            ITray selectedTray = availableTrays[randomIndex];
            //print(i + ": Random index: " + randomIndex + " - Tray ID: " + selectedTray.getTrayId());
            currentOrder.Enqueue(selectedTray);
            availableTrays.RemoveAt(randomIndex);
        }
        onOrderStarted?.Invoke();


        OrdersCount++;


        // Debug.Log($"New order created with {currentOrder.Count} trays");


        // Start processing the order
        if (orderRoutine != null)
        {
            StopCoroutine(orderRoutine);
        }

        orderRoutine = StartCoroutine(processOrderRoutine());
    }

    private List<ITray> getAvailableTrays()
    {
        var trays = TrayManager.instance.getTrays();
        if (trays == null || trays.Count == 0)
        {
            Debug.LogError("No trays registered in TrayManager!");
            return new List<ITray>();
        }

        return trays.FindAll(tray =>
            tray != null &&
            tray.hasSnacksAvailable()
        );
    }
    public void stopOrderProcessing()
    {
        if (orderRoutine != null)
        {
            StopCoroutine(orderRoutine);
            orderRoutine = null;
        }
        isProcessingOrder = false;
        currentOrder.Clear(); // Always clear when stopping
        trayRemoved?.Invoke();
        Debug.Log("Order processing stopped and cleared");
    }
    public void stopOrderProcessing(float forSeconds)
    {
        if (orderRoutine != null)
        {
            StopCoroutine(orderRoutine);
            orderRoutine = null;
        }
        isProcessingOrder = false;
        currentOrder.Clear(); // Clear current order

        // Cancel any previous invoke before setting new one
        CancelInvoke(nameof(restartOrderProcessing));
        Invoke(nameof(restartOrderProcessing), forSeconds);
        Debug.Log($"Order processing stopped for {forSeconds} seconds");
    }
    public void restartOrderProcessing()
    {
        // Cancel any pending invoke
        CancelInvoke(nameof(restartOrderProcessing));

        // Stop any existing coroutine
        if (orderRoutine != null)
        {
            StopCoroutine(orderRoutine);
            orderRoutine = null;
        }

        // Reset state and start fresh
        isProcessingOrder = false;
        currentOrder.Clear();

        // Start a completely new order
        startNewOrder();
        Debug.Log("Order processing restarted with new order");
    }

    private IEnumerator processOrderRoutine()
    {
        isProcessingOrder = true;
        onOrderStarted?.Invoke();
        UIGameNotifications.instance.playStartingOrderNotification();
        yield return new WaitForSeconds(2f); // Wait for notification to play
        while (currentOrder.Count > 0)
        {
            // Wait for the release strategy to complete before continuing
            yield return StartCoroutine(releaseStrategy2());

        }

        isProcessingOrder = false;

        yield return new WaitForSeconds(2f); // Small delay before next order
        UIGameNotifications.instance.playRocksComingNotification();
        yield return new WaitForSeconds(3f); // Wait for notification to play
        TrayManager.instance.addRocksOnTrays();
        yield return new WaitForSeconds(timeBetweenOrders);
        Debug.Log("Order complete");

        // UIGameNotifications.instance.playOrderCompletedNotification();
        yield return new WaitForSeconds(2f); // Wait for notification to play

        onOrderComplete?.Invoke();

        // Only start new order if we're not manually stopping
        if (orderRoutine != null) // Check if we haven't been stopped
        {
            startNewOrder();
        }
    }

    private IEnumerator releaseStrategy1()
    {
        ITray currentTray = currentOrder.Peek();

        if (currentTray.hasSnacksAvailable())
        {
            currentTray.releaseSnack();
            yield return new WaitForSeconds(timeBetweenSnacks);
        }
        else
        {
            // Remove empty tray
            currentOrder.Dequeue();
            TrayManager.instance.unRegisterTray(currentTray);
            Debug.Log($"Tray {currentTray.getTrayId()} removed - no snacks left");
        }
    }
    private IEnumerator releaseStrategy2()
    {
        if (currentOrder.Count == 0) yield break;

        ITray currentTray = currentOrder.Peek();

        if (currentTray != null && currentTray.hasSnacksAvailable() && currentOrder.Count > 0)
        {
            currentTray.releaseSnack();
            yield return new WaitForSeconds(timeBetweenSnacks);
            if (currentOrder.Count == 0) yield break; // Check again after waiting
            currentOrder.Dequeue();
            trayRemoved?.Invoke();
        }
        else
        {
            // Remove empty tray
            if (currentTray != null)
            {
                TrayManager.instance.unRegisterTray(currentTray);
                Debug.Log($"Tray {currentTray.getTrayId()} removed - no snacks left");
            }
            currentOrder.Dequeue();
        }
    }

    private void OnDestroy()
    {
        TrayManager.onTraysRegistered -= OnTraysRegistered;
        CancelInvoke(); // Cancel all invokes
        if (orderRoutine != null)
        {
            StopCoroutine(orderRoutine);
        }
    }



}