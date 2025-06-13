using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Tray : MonoBehaviour, ITray
{
    // [SerializeField] SnackSpawner spawnerPrefabTemplate;
    // [SerializeField] Platform platformPrefabTemplate;
    // [SerializeField] SpawnerConfig[] spawnerConfigs;
    [SerializeField] SnackSpawner snackSpawnerLoaded;
    [SerializeField] Platform platformLoaded;
    Coroutine waitForSnack;
    [SerializeField] string id;
    bool platformDestroyedOnStart = false;
    TrayConfig trayConfig;

    void OnEnable()
    {
        GameEvents.onTraysCleared += fillTray; 
    }

    private void Start()
    {
        if (trayConfig == null)
        {
            Debug.LogError($"TrayConfig not set for tray {gameObject.name}");
            return;
        }

        initializePlatform();
        initializeSnackSpawner();
    }
    public int getEmptyCount()
    {
        if (snackSpawnerLoaded == null)
        {
            Debug.LogError($"SnackSpawner not initialized for tray {id}");
            return 0;
        }
        return snackSpawnerLoaded.getEmptyCount();
    }
 

    // void Start()
    // {

    //     //id = TrayManager.instance.getTrayId(this as ITray);

    //     // parseTrayIdFromTraySpawner();
    //     initializePlatform();
    //     initializeSnackSpawner();
    //     // TrayManager.registerTray(this);
    //     // isPlatformBroken = Random.Range(0, 2) ? 0 : 1; // 50% chance to be broken.
    // }
    public Snack addRockSnack()
    {
        if (snackSpawnerLoaded == null)
        {
            Debug.LogError($"SnackSpawner not initialized for tray {id}");
            return null;
        }

        return snackSpawnerLoaded.addRockSnack(trayConfig.rockConfig, true);
    }
    public void fillTray()
    {
        if (snackSpawnerLoaded == null)
        {
            Debug.LogError($"SnackSpawner not initialized for tray {id}");
            return;
        }

        snackSpawnerLoaded.fillSnacks();
    }

    private void initializeSnackSpawner()
    {
        if (snackSpawnerLoaded != null)
        {
            Debug.LogWarning($"SnackSpawner already initialized for tray {id}");
            return;
        }

        if (trayConfig.spawnerPrefabTemplate == null)
        {
            Debug.LogError($"Spawner prefab not set in TrayConfig for tray {id}");
            return;
        }

        if (platformLoaded == null)
        {
            Debug.LogError($"Platform not initialized for tray {id}");
            return;
        }

        snackSpawnerLoaded = Instantiate(trayConfig.spawnerPrefabTemplate, transform);
        snackSpawnerLoaded.initialize(trayConfig.spawnerConfigs[Random.Range(0, trayConfig.spawnerConfigs.Length)]);
        snackSpawnerLoaded.transform.SetParent(platformLoaded.getPlatformAnimHandler().transform);

        //Debug.Log($"SnackSpawner initialized for tray {id}");
    }

    // private void initializeSnackSpawner()
    // {
    //     snackSpawnerLoaded = Instantiate(trayConfig.spawnerPrefabTemplate, transform);
    //     snackSpawnerLoaded.initialize(trayConfig.spawnerConfigs[Random.Range(0, trayConfig.spawnerConfigs.Length)]);
    //     snackSpawnerLoaded.transform.SetParent(platformLoaded.getPlatformAnimHandler().transform); // this will sync the platform shake or things like that.

    // }


    private void initializePlatform()
    {
        platformLoaded = Instantiate(trayConfig.platformPrefabTemplate, transform);
        determinePlatformBreakOnStartProbability();

        if (platformDestroyedOnStart)
        {

            platformLoaded.destroyPlatform();
        }
        platformLoaded.setTrayIdSO(TrayManager.instance.getTrayIdSO(id));
        platformLoaded.loadTrayIdSprite();
        platformLoaded.onPlatformLanded += deterMineReleaseRandomSnackProbability;

    }
    public void deterMineReleaseRandomSnackProbability()
    {
        if (snackSpawnerLoaded == null)
        {
            Debug.LogError($"SnackSpawner not initialized for tray {id}");
            return;
        }
        bool release = Random.value < trayConfig.chanceToReleaseRandomSnack;
        if (release)
        {
            snackSpawnerLoaded.releaseSnack();
        }


    }
    public void determinePlatformBreakOnStartProbability()
    {
        platformDestroyedOnStart = Random.value < trayConfig.platformShouldBreakOnStartChance;
    }

    public void waitForSnackToLeave(Snack snack)
    {

        waitForSnack = StartCoroutine(waitForSnackToLeaveCoroutine(snack));
    }
    IEnumerator waitForSnackToLeaveCoroutine(Snack snack)
    {
        //yield return new WaitForSeconds(snack.getLeaveDuration());
        yield return new WaitUntil(() => snack.isReadyToFall());
        waitForSnack = null;


    }

    public void destroyPlatform()
    {
        platformLoaded.destroyPlatform();
    }

    public void repairPlatform()
    {
        platformLoaded.repairPlatform();
    }
    public void releaseSnack()
    {
        if (waitForSnack == null)
        {

            Snack snack = snackSpawnerLoaded.releaseSnack();
            if (snack != null)
            {
                //snack.startLeavingSpiralTray();
                waitForSnackToLeave(snack);
                //Debug.Log("Snack releasing from tray: " + id);
            }

        }
        else
        {
            // Debug.Log("There is a snack leaving the tray, please wait.");
        }

    }
    public void setTrayId(string id)
    {
        this.id = id;
        transform.name = "Tray " + id; // this will be the id of the tray.
        //Debug.Log("Tray id set to: " + id);
    }

    public string getTrayId()
    {
        return id;
    }
    public void setTrayConfig(TrayConfig trayConfig)
    {
        this.trayConfig = trayConfig;
    }

    public TrayConfig getTrayConfig()
    {
        return trayConfig;
    }

    public bool hasSnacksAvailable()
    {
        if (snackSpawnerLoaded == null)
        {
            Debug.LogError($"SnackSpawner not initialized for tray {id}");
            return false;
        }
        return snackSpawnerLoaded.hasSnacksAvailable();
    }



    public SnackSpawner getSnackSpawnerLoaded()
    {
        return snackSpawnerLoaded;
    }
}


