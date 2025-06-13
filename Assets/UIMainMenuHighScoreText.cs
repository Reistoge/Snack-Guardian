using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMainMenuHighScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;
 
    
    private void Start()
    {
        if (GameManager.Instance.GameData != null && highScoreText != null)
        {
            highScoreText.text = "High Score: " + GameManager.Instance.GameData.highScore.ToString("F0");
        }
        else
        {
            Debug.LogWarning("GameDataSO or HighScoreText is not set.");
        }
    }
}
