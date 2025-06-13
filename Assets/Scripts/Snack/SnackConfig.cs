using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SnackConfig", menuName = "Snack Guardian/SnackConfig", order = 1)]
public class SnackConfig : ScriptableObject
{
    public ObjectEffect objectEffect;
    public FallStats fallStats;
    public RuntimeAnimatorController animator;
    public GameObject destroyParticlesPrefab;
    public Sprite icon;
    public bool destroyOnFall = false;
    public bool isRock = false;

   
    
 
}
