using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AudioAsset", menuName = "Snack Guardian/AudioAsset", order = 1)]
public class AudioConfig : ScriptableObject
{

    [Header("Audio Settings")]
    [SerializeField, Range(0, 1)] public float sfxVolume = 0.7f;
    [SerializeField, Range(0, 256)] public int priority = 128;
    [SerializeField, Range(0, 1)] public float spatialBlend = 0.5f;
    [SerializeField, Range(-1, 1)] public float stereoPan = 0.5f;
    [SerializeField, Range(-3, 3)] public float pitch = 0.5f;
    [SerializeField] public bool loop = false;
    [SerializeField] public AudioClip clip;





}