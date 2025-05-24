using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraySpawner : MonoBehaviour
{
    // [SerializeField] GameObject trayPrefab;
    // [SerializeField] float traySeparation = 0.1f;
    // [SerializeField] int trayCount = 5;
    [SerializeField] TraySpawnerConfig traySpawnerConfig;
    void Start()
    {
        spawnTray();
    }
    public void spawnTray()
    {

        for (int i = 0; i < traySpawnerConfig.trayCount; i++)
        {
            //GameObject tray = Instantiate(trayConfig.trayPrefab, transform);
            GameObject tray = Instantiate(traySpawnerConfig.trayConfig.trayPrefab, transform);
            tray.GetComponent<Tray>().setTrayConfig(traySpawnerConfig.trayConfig);
            tray.transform.localPosition = new Vector3(i * traySpawnerConfig.traySeparation, 0, 0);
            tray.transform.localScale = new Vector3(1, 1, 1);
            // needed for set the tray id
            string id = transform.name.Split(' ')[1] + (i+1);
            tray.transform.name = "Tray " + id;
            TrayManager.instance.registerTray(tray.GetComponent<ITray>(), id);
            	
        }
    }

}
