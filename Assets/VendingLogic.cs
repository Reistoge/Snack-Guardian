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
    
    public static event Action trayRemoved;
    public static event Action OnOrderComplete;
    public static event Action OnOrderStarted;
    public static VendingLogic instance;

    private Queue<ITray> currentOrder = new Queue<ITray>();
    private bool isProcessingOrder = false;
    private Coroutine orderRoutine;
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

    }
    void OnDisable()
    {
        TrayManager.onTraysRegistered -= OnTraysRegistered;
    }
    private void OnTraysRegistered()
    {
        Debug.Log("Trays registered, starting new order");
        // Add small delay to ensure trays are fully initialized
        StartCoroutine(DelayedStartOrder());
    }

    private IEnumerator DelayedStartOrder()
    {
        yield return new WaitForSeconds(0.2f); // Give more time for initialization

        List<ITray> trays = GetAvailableTrays();
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
        StartNewOrder();
    }

    private void StartNewOrder()
    {
        if (isProcessingOrder) return;

        
        // Get available trays (those with snacks)
        List<ITray> availableTrays = GetAvailableTrays();

        if (availableTrays.Count == 0)
        {
            Debug.Log("No trays available - Vending complete");
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
            print(i + ": Random index: " + randomIndex + " - Tray ID: " + selectedTray.getTrayId());
            currentOrder.Enqueue(selectedTray);
            availableTrays.RemoveAt(randomIndex);
        }
        OnOrderStarted?.Invoke();


        Debug.Log($"New order created with {currentOrder.Count} trays");
        

        // Start processing the order
        if (orderRoutine != null)
        {
            StopCoroutine(orderRoutine);
        }
        orderRoutine = StartCoroutine(ProcessOrderRoutine());
    }

    private List<ITray> GetAvailableTrays()
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

    private IEnumerator ProcessOrderRoutine()
    {
        isProcessingOrder = true;

        while (currentOrder.Count > 0)
        {
            // Wait for the release strategy to complete before continuing
            yield return StartCoroutine(releaseStrategy2());

        }

        isProcessingOrder = false;
        yield return new WaitForSeconds(timeBetweenOrders);
        Debug.Log("Order complete");
        OnOrderComplete?.Invoke();
        // Start new order if there are still trays available
        StartNewOrder();
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

        if (currentTray != null && currentTray.hasSnacksAvailable())
        {
            currentTray.releaseSnack();
            yield return new WaitForSeconds(timeBetweenSnacks);
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
        if (orderRoutine != null)
        {
            StopCoroutine(orderRoutine);
        }
    }



}