using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
[DefaultExecutionOrder(-99)] // IMPORTANT: initialization After TrayManager (-100)
public class TraySpawner : MonoBehaviour
{
    [SerializeField] TraySpawnerConfig traySpawnerConfig;
    private List<Tray> spawnedTrays = new List<Tray>();

    void Start()
    {
        StartCoroutine(spawnTrayRoutine());
    }

    private IEnumerator spawnTrayRoutine()
    {
        for (int i = 0; i < traySpawnerConfig.trayCount; i++)
        {
            GameObject tray = Instantiate(traySpawnerConfig.trayConfig.trayPrefab, transform);
            Tray trayComponent = tray.GetComponent<Tray>();
            
            // Set config before initialization
            trayComponent.setTrayConfig(traySpawnerConfig.trayConfig);
            
            // Position setup
            tray.transform.localPosition = new Vector3(i * traySpawnerConfig.traySeparation, 0, 0);
            tray.transform.localScale = Vector3.one;
            
            string id = transform.name.Split(' ')[1] + (i + 1);
            TrayManager.instance.registerTray(trayComponent, id);
            
            spawnedTrays.Add(trayComponent);
            
            // Wait a frame between spawns
            yield return null;
        }

        // Verify initialization
        yield return new WaitForSeconds(0.1f); // Small delay to ensure components are ready
        
        bool allTraysInitialized = spawnedTrays.All(t => 
            t.getTrayConfig() != null && 
            t.hasSnacksAvailable()
        );

        if (allTraysInitialized)
        {
            TrayManager.instance.triggerOnTraysRegistered();
        }
        else
        {
            Debug.LogError("Some trays failed to initialize properly!");
        }
    }
}

// public class TraySpawner : MonoBehaviour
// {
//     // [SerializeField] GameObject trayPrefab;
//     // [SerializeField] float traySeparation = 0.1f;
//     // [SerializeField] int trayCount = 5;
//     [SerializeField] TraySpawnerConfig traySpawnerConfig;
//     void Start()
//     {
//         spawnTray();
//     }
//     public void spawnTray()
//     {

//         for (int i = 0; i < traySpawnerConfig.trayCount; i++)
//         {
//             //GameObject tray = Instantiate(trayConfig.trayPrefab, transform);
//             GameObject tray = Instantiate(traySpawnerConfig.trayConfig.trayPrefab, transform);
//             tray.GetComponent<Tray>().setTrayConfig(traySpawnerConfig.trayConfig);
//             tray.transform.localPosition = new Vector3(i * traySpawnerConfig.traySeparation, 0, 0);
//             tray.transform.localScale = new Vector3(1, 1, 1);
//             // needed for set the tray id
//             string id = transform.name.Split(' ')[1] + (i + 1);
//             tray.transform.name = "Tray " + id;
//             TrayManager.instance.registerTray(tray.GetComponent<ITray>(), id);


//         }
//         TrayManager.instance.triggerOnTraysRegistered();// Notify that trays have been registered
//     }

// }
