using UnityEngine;

[CreateAssetMenu(fileName = "SnackGuardianSFX", menuName = "Snack Guardian/SnackGuardianSFX", order = 1)]
public class SnackGuardianSFX : ScriptableObject
{
    public AudioConfig damageAudio;
    public AudioConfig jumpAudio;
    public AudioConfig powerUpAudio;
    public AudioConfig snackFallAudio;
}
