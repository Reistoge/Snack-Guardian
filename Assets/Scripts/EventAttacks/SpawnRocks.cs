using System;
using System.Collections;
using System.Collections.Generic;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnRocks : MonoBehaviour, GameDebuff
{
    // Start is called before the first frame update

    [SerializeField] List<Snack> rocks = new List<Snack>();
    [SerializeField] int spawnIteration = 3;
    [SerializeField] float notificationDelay = 2f;
    [SerializeField] float spawnDelay = 4f;

    public bool debuffIsActive;
    public bool DebuffIsActive { get => debuffIsActive; set => debuffIsActive = value; }

    private IEnumerator spawnRocksCoroutine()
    {
        // for now works fine with this solution, if you want a more precise timing
        // just yield until the rock is landed on the last spawns rocks or in
        //  the entire spawn iterations -- waitForRocksToLandCoroutine()
        debuffIsActive = true;

        Debug.Log("Spawning rocks...");
        VendingLogic.instance.stopOrderProcessing();

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < spawnIteration; i++)
        {
            UIGameNotifications.instance.playRocksComingNotification();
            yield return new WaitForSeconds(notificationDelay);
            addRocksOnRandomRow();
            yield return new WaitForSeconds(spawnDelay);

        }
        debuffIsActive = false;

        VendingLogic.instance.restartOrderProcessing();


    }

    public void addRocksOnRandomRow()
    {

        List<string> list = new List<string> { "A", "B", "C", "D", "E", "F" };
        rocks = TrayManager.instance.addRocksOnTrays(list[Random.Range(0, list.Count)].ToCharArray()[0]);


    }
    // IEnumerator waitForRocksToLandCoroutine()
    // {
    //     Debug.Log("waiting for rocks to land.");
    //     //Debug.Log("rocks count: " + rocks.Count);
    //     yield return new WaitUntil(() => rocks.TrueForAll(rock => rock.Landed == true));

    //     Debug.Log("finished waiting for rocks to land.");



    // }

    public GameDebuff applyDebuff()
    {
        StartCoroutine(spawnRocksCoroutine());
        return this;
    }

    public string getDebuffName()
    {
        return "spawnRocks";
    }
}
