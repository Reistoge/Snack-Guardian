using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TraySpawnerConfig", menuName = "Snack Guardian/TraySpawnerConfig", order = 1)]
public class TraySpawnerConfig : ScriptableObject
{
    public TrayConfig trayConfig;
    public float traySeparation = 0.1f;
    public int trayCount = 10;
    
    


}
