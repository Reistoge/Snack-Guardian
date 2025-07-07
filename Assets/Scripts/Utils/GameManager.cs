using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PlayerMovementStats defaultMovementStats;
    [SerializeField] GameDataSO gameData;
    public static Action OnGameSceneLoaded;
    public static Action OnMainMenuLoaded;
    public static Action OnPlayerSpawned;

    
    



    private GameObject currentPlayer;
    [SerializeField] private GameObject transitionAnimPrefab;

    public GameDataSO GameData { get => gameData; set => gameData = value; }

    void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += onLevelFinishedLoading;
        GameEvents.onGameOver += tryUpdateHighScore;

    }
    public string getPlayerName()
    {
        if (GameData != null)
        {
            return GameData.playerName;
        }
        else
        {
            Debug.LogWarning("GameData is not set in GameManager.");
            return "DefaultPlayer";
        }
    }
    


    private void tryUpdateHighScore()
    {
        if (PointsManager.Instance != null && PointsManager.Instance.Points > GameData.highScore)
        {
            GameData.highScore = PointsManager.Instance.Points;
            Debug.Log("High score updated to: " + GameData.highScore);
        }
        else
        {
            Debug.Log("Current points: " + PointsManager.Instance.Points);
        }

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= onLevelFinishedLoading;
        GameEvents.onGameOver -= tryUpdateHighScore;
    }

    private void onLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        AudioManager.Instance.stopAllAudio();
        if (scene.name.Contains("GameScene"))
        {
            OnGameSceneLoaded?.Invoke();
            spawnPlayer();
        }
        if (scene.name.Equals("HowToPlay"))
        {
            AudioManager.Instance.playHowToPlayMusic();
        }
 
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name.Contains("GameScene"))
            {
                GameEvents.triggerGameSceneEnded();
                MultiplayerGameEvents.triggerGameSceneEnded();
            }
            loadScene("MainMenu");
            
        }
        else if (Input.GetKeyDown(KeyCode.F1))
        {
            loadScene("GameScene");

        }
        
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            currentPlayer = GameObject.FindGameObjectWithTag("Player");
            if (currentPlayer != null)
            {
                currentPlayer.TryGetComponent<Player>(out var playerComponent);
                playerComponent.takeDamage(100000);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }

    }

    private void spawnPlayer()
    {
        if (currentPlayer == null)
        {
            Vector3 spawnPosition = getSpawnPosition();
            currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            if (currentPlayer.TryGetComponent<Player>(out var playerComponent))
            {
                playerComponent.Movement.setMovementStats(defaultMovementStats);
            }

            OnPlayerSpawned?.Invoke();
            GameEvents.triggerPlayerSpawned(currentPlayer);

        }
    }

    private Vector3 getSpawnPosition()
    {
        // Find spawn point or use default position
        var spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        return spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
    }

    public void loadScene(string sceneName)
    {

        Debug.Log($"[GameManager] Intentando cargar escena: '{sceneName}'"); // �A�ade esta l�nea!
        //SceneManager.LoadScene(sceneName);
        StartCoroutine(playTransition(() => SceneManager.LoadScene(sceneName)));
        
    }
    IEnumerator playTransition(Action action)
    {
        GameObject transition = Instantiate(transitionAnimPrefab);
        yield return new WaitForSeconds(2f); // Wait for the transition to start
        action?.Invoke(); // Invoke the action after the transition delay
   
 



    }
 

     
}