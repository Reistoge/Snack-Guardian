using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
