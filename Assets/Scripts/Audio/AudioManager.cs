using System;
using System.Collections;
using System.Collections.Generic;
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

    public void playGamePlayMusic()
    {
        if (snackGuardianMusic != null && snackGuardianMusic.GameplayMusic != null)
        {
            playSFX(snackGuardianMusic.GameplayMusic);
        }
    }

    public void playMainMenuMusic()
    {
        if (snackGuardianMusic != null && snackGuardianMusic.MainMenuMusic != null)
        {

            playSFX(snackGuardianMusic.MainMenuMusic);
        }
    }

    //public void playDamagedAudio()
    //{
        //if (snackGuardianSFX != null && snackGuardianSFX.damageAudio != null)
        //{
            //playSFX(snackGuardianSFX.damageAudio);
        //}
    //}

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
    // [SerializeField] SnackGuardianSFX snackGuardianSFX;

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
    // public SnackGuardianSFX SnackGuardianSFX { get => snackGuardianSFX; set => snackGuardianSFX = value; }
}
