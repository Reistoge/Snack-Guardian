using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering;
 
using Random = UnityEngine.Random;

public class AudioManager : GenericSingleton<AudioManager>
{

    [SerializeField] AudioPlayer audioPlayerPrefab;
    [SerializeField] List<AudioPlayer> audioPlayers = new List<AudioPlayer>();

    [SerializeField] List<AudioConfig> audioConfigs = new List<AudioConfig>();

    public void OnEnable()
    {
        GameManager.OnGameSceneLoaded += playGamePlayMusic;
        

    }

    private void playGamePlayMusic()
    {

    }
    public void playMainMenuMusic()
    {
        if (snackGuardianMusic != null && snackGuardianMusic.MainMenuMusic != null)
        {

            playSFX(snackGuardianMusic.MainMenuMusic);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            playSFX(audioConfigs[Random.Range(0, audioConfigs.Count)]);
        }
    }

    public void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= playGamePlayMusic;

    }
    // music repository
    [SerializeField] SnackGuardianMusic snackGuardianMusic;

    // function to basically play any sfx

    public void playSFX(AudioConfig audioConfig)
    {
        if (audioConfig == null || audioConfig.clip == null)
        {
            Debug.LogWarning("AudioConfig or AudioClip is null");
            return;
        }
        foreach (AudioPlayer audio in audioPlayers)
        {
            if (audio.IsPlaying == false)
            {
                audio.playAudio(audioConfig);
                destroyInactiveAudioPlayers();


                return;
            }
        }

        AudioPlayer audioPlayer = Instantiate(audioPlayerPrefab, transform);
        audioPlayer.playAudio(audioConfig);
        audioPlayers.Add(audioPlayer);

    }

    private void destroyInactiveAudioPlayers()
    {

        for (int i = audioPlayers.Count - 1; i >= 0; i--)
        {
            if (!audioPlayers[i].IsPlaying)
            {
                Destroy(audioPlayers[i].gameObject);
                audioPlayers.RemoveAt(i);
            }
        }

    }
    public void stopAllAudio()
    {
        for (int i = audioPlayers.Count - 1; i >= 0; i--)
        {

            Destroy(audioPlayers[i].gameObject);
            audioPlayers.RemoveAt(i);

        }
    }

    public SnackGuardianMusic SnackGuardianMusic { get => snackGuardianMusic; set => snackGuardianMusic = value; }
}
