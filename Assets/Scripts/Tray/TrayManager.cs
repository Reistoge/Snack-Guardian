
using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)] // initialization before other scripts.
public class TrayManager : MonoBehaviour
{
    public static TrayManager instance;
    private List<ITray> trays = new List<ITray>();
    public static event Action onTraysRegistered;
    const string trayIdPath = "ScriptableObjects/TrayIds";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            releaseRandomSnack();
        }

    }
    public void triggerOnTraysRegistered()
    {
        onTraysRegistered?.Invoke();
        Debug.Log("onTraysRegistered triggered");
    }
    private void releaseRandomSnack()
    {
        if (trays.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, trays.Count);
            Debug.Log("Random index: " + randomIndex);
            ITray tray = trays[randomIndex];
            if (tray != null)
            {
                tray.releaseSnack();

            }
            Debug.Log("Snack released: " + tray.getTrayId());
        }
        else
        {
            Debug.Log("No trays to release");
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void registerTray(ITray tray, string id)
    {
        if (!trays.Contains(tray))
        {
            trays.Add(tray);
            tray.setTrayId(id);
        }
        Debug.Log("Tray registered: " + tray.getTrayId() + "tray count: " + trays.Count);
    }
    public void unRegisterTray(ITray tray)
    {
        if (trays.Contains(tray))
        {
            trays.Remove(tray);
            Tray t = tray as Tray;
            t?.gameObject.SetActive(false);
            Debug.Log("Tray unregistered: " + tray.getTrayId());
            // tray.gameObject.SetActive(false);
        }
    }
    public void unRegisterTray(string id)
    {
        foreach (ITray tray in trays)
        {
            if (tray.getTrayId() == id)
            {
                trays.Remove(tray);
                Tray t = tray as Tray;
                t?.gameObject.SetActive(false);
                Debug.Log("Tray unregistered: " + tray.getTrayId());
                break;
            }
        }
    }
    public List<ITray> getTrays()
    {
        return trays;
    }
    public TrayId getTrayIdSO(string id)
    {
        TrayId[] trayIdsSrc = Resources.LoadAll<TrayId>(trayIdPath);
        foreach (TrayId trayId in trayIdsSrc)
        {

            if (trayId.id == id)
            {
                return trayId;
            }
        }

        return trayIdsSrc[0];
    }
    public string getTrayId(ITray tray)
    {
        try
        {
            foreach (ITray t in trays)
            {
                if (t == tray)
                {
                    return t.getTrayId();
                }
            }
            return "not loaded";
        }
        catch
        {
            Debug.LogError("TrayId not found: " + tray.getTrayId());
            return "not loaded";
        }




    }
    public ITray getRandomTray()
    {
        return trays[UnityEngine.Random.Range(0, trays.Count)];
    }




}
