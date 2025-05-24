using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SpawnerConfig", menuName = "Snack Guardian/SpawnerConfig", order = 1)]
public class SpawnerConfig : ScriptableObject
{
    
    [SerializeField] public SnackConfig[] snackConfigs;
    [SerializeField] public int maxSnacks = 10;
    [SerializeField] public float maxScale = 1f;
    [SerializeField] public float leaveDurationSpeedFactor = 1f;
    [SerializeField] public float spawnDelay = 3f;
    [SerializeField] public float spawnChance = 0.5f;



}
