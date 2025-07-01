using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{

    void OnEnable()
    {
        MainMenuCigarette.onCigarette += activateChilds;


    }
    void OnDisable()
    {
        MainMenuCigarette.onCigarette -= activateChilds;
    }
    public void activateChilds()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    public void onChangePlayerName(TextMeshProUGUI text)
    {

        if (GameManager.Instance != null && GameManager.Instance.GameData != null)
        {
            GameManager.Instance.GameData.playerName = text.text;
            Debug.Log("Player name changed to: " + GameManager.Instance.GameData.playerName);
        }
        else
        {
            Debug.LogWarning("GameManager or GameData is not set.");
        }


    }
    public void changeToPlayerName(TextMeshProUGUI text)
    {
        if (GameManager.Instance != null && GameManager.Instance.GameData != null)
        {
            text.text = GameManager.Instance.GameData.playerName;
            text.RecalculateMasking();
         
        }
        else
        {
            Debug.LogWarning("GameManager or GameData is not set.");
        }
    }
   

}
