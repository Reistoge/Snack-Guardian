using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrayConfig", menuName = "Snack Guardian/TrayConfig", order = 1)]
public class TrayConfig : ScriptableObject
{
    public GameObject trayPrefab;
    public SnackSpawner spawnerPrefabTemplate;
    public Platform platformPrefabTemplate;
    public SpawnerConfig[] spawnerConfigs;
    public  SnackConfig rockConfig;
    public float chanceToReleaseRandomSnack = 0.5f; // 50% chance to release a random snack.
    public float platformShouldBreakOnStartChance = 0.5f; // 50% chance to be broken.


}
