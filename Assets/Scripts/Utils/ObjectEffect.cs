using UnityEngine;

public enum EffectType
{
    Heal,
    Damage,
    Nothing,
     
}

[CreateAssetMenu(fileName = "New Effect", menuName = "Snack Guardian/Object Effect")]
public class ObjectEffect : ScriptableObject
{
    public EffectType type;
    public int amount;
    public Color effectColor = Color.white;
    public GameObject effectParticles;
    
}