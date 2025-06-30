using UnityEngine;

[CreateAssetMenu(fileName = "AudioAsset", menuName = "Snack Guardian/PlayerAudio", order = 1)]

public class PlayerAudio : ScriptableObject
{
    
    [SerializeField] AudioConfig damageAudio;
    [SerializeField] AudioConfig touchWallAudio;
    [SerializeField] AudioConfig firstJumpAudio;
    [SerializeField] AudioConfig secondJumpAudio;
    [SerializeField] AudioConfig dashAudio;



  

    public AudioConfig TouchWallAudio => touchWallAudio;
    public AudioConfig DamageAudio => damageAudio;
    public AudioConfig FirstJumpAudio => firstJumpAudio;
    public AudioConfig SecondJumpAudio => secondJumpAudio;
    public AudioConfig DashAudio => dashAudio;
}