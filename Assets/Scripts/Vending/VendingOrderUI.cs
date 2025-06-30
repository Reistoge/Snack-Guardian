using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VendingOrderUI : MonoBehaviour
{
    [SerializeField] private Transform orderContainer; // Parent for UI elements
    [SerializeField] private GameObject orderItemPrefab; // Prefab with Image and Text components

    [SerializeField] float scaleFactor; // Adjust this for overall scale
    [SerializeField] float separation;

    void OnEnable()
    {
        VendingLogic.onOrderStarted += showOrderUI;
        VendingLogic.trayRemoved += showOrderUI;// Subscribe to the event
    }
    void OnDisable()
    {
        VendingLogic.onOrderStarted -= showOrderUI;
        VendingLogic.trayRemoved -= showOrderUI; // Unsubscribe from the event
    }


    public void showOrderUI()
    {
        List<ITray> order = VendingLogic.instance.getCurrentOrder().ToList();
        StartCoroutine(updateOrderUIRoutine(order));
    }

    private IEnumerator updateOrderUIRoutine(List<ITray> order)
    {
        // Clear existing order display first

        foreach (Transform child in orderContainer)
        {
            Destroy(child.gameObject);
        }

        // Wait one frame to ensure all destructions are complete
        yield return null;

        // Now create new order items
        for (int i = 0; i < order.Count; i++)
        {
            ITray tray = order[i];

            // Get tray ID
            TrayId trayId = TrayManager.instance.getTrayIdSO(tray.getTrayId());

            // Get current snack sprite
            Sprite snackSprite = null;

            SnackSpawner spawner = tray.getSnackSpawnerLoaded();
            if (spawner != null)
            {
                snackSprite = spawner.getCurrentSnackSprite();
            }


            // Create UI element
            GameObject orderItem = Instantiate(orderItemPrefab, orderContainer);

            // // Calculate decreasing scale for each item (e.g., linear or exponential)
            // float scale = scaleFactor * Mathf.Pow(0.92f, i); // 0.92f is a tweakable factor for how fast it shrinks
            // orderItem.transform.localScale = new Vector3(scale, scale, 1f);

            // Adjust separation according to scale (so items don't overlap and spacing feels natural)
            float adjustedSeparation = separation; //* scale;
            float yPos = -adjustedSeparation * i;
            orderItem.transform.localPosition = new Vector2(0, yPos);

            // Setup UI
            OrderItemUI itemUI = orderItem.GetComponent<OrderItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(trayId, snackSprite);
            }
            if (i == 0)
            {
                foreach (Transform t in itemUI.transform)
                {
                    t.gameObject.SetActive(true); // Ensure the first item is visible
                }
            }

            // Optional: Add a small delay between items for visual effect
            yield return new WaitForSeconds(0.1f);
        }
    }
}
