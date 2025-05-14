using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "SnackConfig", menuName = "Snack Guardian/SnackConfig", order = 1)]
public class SnackConfig : ScriptableObject
{
    public ObjectEffect objectEffect;
    public FallStats fallStats;
    public AnimatorController animator;

   
    
 
}
