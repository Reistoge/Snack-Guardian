using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject playerPrefab;
    [SerializeField] PlayerMovementStats playerMovementStats;
    [SerializeField] Transform spawnPoint;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SpawnPlayer();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    void OnEnable()
    {
        GameManager.OnGameSceneLoaded += loadPlayer;

    }
    void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= loadPlayer;
    }
    void SpawnPlayer()
    {
        // Check if the player is already spawned
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
             
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            Player playerScript = player.GetComponent<Player>();
            playerScript.Movement.setMovementStats(playerMovementStats);
            GameManager.Instance.setPlayer(player);
            onPlayerSpawned?.Invoke();
        }
        else
        {
            Debug.Log("Player already spawned");
        }
    }
    void loadPlayer()
    {
        // Check if the player is already spawned
   
        GameManager.Instance.setPlayer(playerPrefab);
            
        
 
    }   

 



}
