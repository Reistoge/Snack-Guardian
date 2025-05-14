using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{

    public void loadScene(string sceneName)
    {
        GameManager.Instance.loadScene(sceneName);
    }
}
