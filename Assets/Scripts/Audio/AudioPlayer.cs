using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    

    [SerializeField] public bool IsPlaying { get { return audioSource.isPlaying; } }

    public void playAudio(AudioConfig audioConfig)
    {
        if (audioConfig == null || audioConfig.clip == null)
        {
            Debug.LogWarning("AudioConfig or AudioClip is null");
            return;
        }

        loadConfig(audioConfig);

        audioSource.Play();
    }
    

    internal void removeInactive()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    private void loadConfig(AudioConfig audioConfig)
    {
        audioSource.clip = audioConfig.clip;
        audioSource.volume = audioConfig.sfxVolume;
        audioSource.priority = audioConfig.priority;
        audioSource.spatialBlend = audioConfig.spatialBlend;
        audioSource.panStereo = audioConfig.stereoPan;
        if (audioConfig.randomPitch > 0)
        {
            audioSource.pitch = UnityEngine.Random.Range(audioConfig.pitch - audioConfig.randomPitch, audioConfig.pitch + audioConfig.randomPitch);
        }
    
        audioSource.pitch = audioConfig.pitch;
        audioSource.loop = audioConfig.loop;
    }
}
