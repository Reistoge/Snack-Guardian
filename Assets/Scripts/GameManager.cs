using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PlayerMovementStats defaultMovementStats;

    public static Action OnGameSceneLoaded;
    public static Action OnPlayerSpawned;

    private GameObject currentPlayer;

    void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += onLevelFinishedLoading;
        GameEvents.onPlayerDeath+= () =>
        {
            Destroy(currentPlayer);
            currentPlayer = null;
            loadScene("MainMenu");
        };
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= onLevelFinishedLoading;
    }

    private void onLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "GameScene":
                OnGameSceneLoaded?.Invoke();
                spawnPlayer();
                break;

            case "MainMenu":
                // Handle main menu initialization
                break;
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
        SceneManager.LoadScene(sceneName);
    }

    public HealthSystem getPlayerHealth()
    {
        return currentPlayer?.GetComponent<HealthSystem>();
    }


}