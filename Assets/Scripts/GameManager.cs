using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
 
[DefaultExecutionOrder(-10)]
public class GameManager : GenericSingleton<GameManager>
{
    // Start is called before the first frame update
     
    public static Action OnGameSceneLoaded;
    public static Action onPlayerLoaded;
    GameObject player;


    void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnEnable()
    {
        // Remove from OnEnable since we're handling it in Awake
        // SceneManager.sceneLoaded += OnLevelFinishedLoading;
        
    }

    

    void OnDestroy()
    {
        // Make sure to unsubscribe when the GameObject is destroyed
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {   
        Debug.Log($"Scene loaded: {scene.name} in mode: {mode}");
        
        switch (scene.name)
        {
            case "MainMenu":
                Debug.Log("Main Menu scene initialized");
                break;
                
            case "GameScene":
                Debug.Log("Game Scene initialized");
                OnGameSceneLoaded?.Invoke();
                break;
                
            default:
                Debug.Log($"Unknown scene loaded: {scene.name}");
                break;
        }
    }
    bool checkSceneLoaded(string sceneName)
    {
        // Check if the scene is loaded
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
    }
    // void getPlayerOnGameScene()
    // {
    //     // Check if the player is already spawned
    //     if (playerOnGameScene == null)
    //     {
    //         playerOnGameScene = GameObject.FindGameObjectWithTag("Player");
    //     }
         
    // }
    public void setPlayer(GameObject player)
    {
        this.player = player;
        onPlayerLoaded?.Invoke();
    }
    public HealthSystem getPlayerHealthSystem()
    {
        if (player != null)
        {
           Debug.Log("Player correctly referenced");
           return player.GetComponent<HealthSystem>();
            
        }
        else
        {
            Debug.LogError("Player is not referenced in GameManager ");
            player = GameObject.FindGameObjectWithTag("Player");
            if(player){
                Debug.LogError("Player found by ObjectWithTag");
                return player.GetComponent<HealthSystem>();
            }else{
                Debug.LogError("Player not found ");
                return null;
            }
            
             
        }
    }
    public void loadScene(string sceneName)
    {
        // Load the scene asynchronously
        SceneManager.LoadScene(sceneName);
    }
 



}
