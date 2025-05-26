using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Add this helper class for the UI prefab
[System.Serializable]
public class OrderItemUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer snackImage;
    [SerializeField] private SpriteRenderer trayIdText;

    public void Setup(TrayId trayId, Sprite snackSprite)
    {
        if (snackImage != null && snackSprite != null)
        {
            snackImage.sprite = snackSprite;
        }

        if (trayIdText != null && trayId != null)
        {
            trayIdText.sprite = trayId.idImage; // Assuming TrayId has an icon property
        }
    }
}