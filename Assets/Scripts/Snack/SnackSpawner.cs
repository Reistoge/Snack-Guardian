using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class SnackSpawner : MonoBehaviour
{
    [SerializeField] GameObject snackPrefabTemplate;
    [SerializeField] SnackConfig[] snackConfigs;
    [SerializeField] Transform spawnPoint;
    [SerializeField] int maxSnacks = 10;
    [SerializeField] float maxScale = 1f;
    [SerializeField] float leaveDurationSpeedFactor = 1f;
    [SerializeField] float spawnDelay = 3f;
    [SerializeField] float spawnChance = 0.5f;
    float scaleFactor = 0.1f;
    SpawnerConfig spawnerConfig;

    Stack<Snack> snackStack = new Stack<Snack>();



    public void initialize()
    {
        if (spawnPoint == null)
        {

            spawnPoint = transform;
            scaleFactor = maxScale / maxSnacks;

            checkErrors();
            fillSnacks();
            // spawnSnacks(maxSnacks, 0);
            //releaseSnackRoutine = StartCoroutine(randomLeaveSnackRoutine());

        }
    }
    public int getEmptyCount()
    {
        return maxSnacks - snackStack.Count;
    }
    void loadConfig(SpawnerConfig spawnerConfig)
    {
        this.spawnerConfig = spawnerConfig;
        snackConfigs = spawnerConfig.snackConfigs;
        maxSnacks = spawnerConfig.maxSnacks;
        maxScale = spawnerConfig.maxScale;
        leaveDurationSpeedFactor = spawnerConfig.leaveDurationSpeedFactor;
        spawnDelay = spawnerConfig.spawnDelay;
        spawnChance = spawnerConfig.spawnChance;
        scaleFactor = maxScale / maxSnacks;


    }
    public void initialize(SpawnerConfig spawnerConfig)
    {
        if (spawnPoint == null)
        {
            spawnPoint = transform;
            loadConfig(spawnerConfig);
            fillSnacks();
            // spawnSnacks(maxSnacks, 0);
            //releaseSnackRoutine = StartCoroutine(randomLeaveSnackRoutine());

        }
    }

    private void checkErrors()
    {
        if (snackPrefabTemplate == null)
        {
            Debug.LogError("Snack prefab template is not assigned.");
        }
        if (snackConfigs == null || snackConfigs.Length == 0)
        {
            Debug.LogError("Snack configs are not assigned or empty.");
        }
        if (maxSnacks <= 0)
        {
            Debug.LogError("Max snacks must be greater than 0.");
        }

        if (maxScale <= 0)
        {
            Debug.LogError("Max scale must be greater than 0.");
        }


    }
    public void addSnack(SnackConfig config)
    {
        if (snackStack.Count >= maxSnacks)
        {
            Debug.LogWarning("Max snacks reached, cannot spawn more.");
            return;
        }

        // Instantiate the snack prefab and set its properties
        Snack snack = snackPrefabTemplate.GetComponent<Snack>();

        // snack setup
        snack = setupSnack(config, snack);
        // instantiate the snack and add to the stack
        GameObject newSnack = Instantiate(snack.gameObject, spawnPoint.position, Quaternion.identity, transform);
        snackStack.Push(newSnack.GetComponent<Snack>());
    }
    public void addSnack(SnackConfig config, bool releaseImmediately)
    {
        addSnack(config);
        if (releaseImmediately)
        {
            releaseSnack();
        }
    }
    public Snack addRockSnack(SnackConfig config, bool releaseImmediately)
    {
        // Instantiate the snack prefab and set its properties
        Snack rock = snackPrefabTemplate.GetComponent<Snack>();

        // snack setup
        rock = setupRock(config, rock);
        // instantiate the snack and add to the stack
        GameObject newSnack = Instantiate(rock.gameObject, spawnPoint.position, Quaternion.identity, transform);
        snackStack.Push(newSnack.GetComponent<Snack>());
        if (releaseImmediately)
        {
            return releaseSnack();
        }
        return rock;
    }
    void addSnack(int indexConfig)
    {
        if (snackStack.Count >= maxSnacks)
        {
            Debug.LogWarning("Max snacks reached, cannot spawn more.");
            return;
        }
        if (indexConfig < 0 || indexConfig >= snackConfigs.Length)
        {
            Debug.LogError("Index out of range");
            return;
        }

        SnackConfig config = snackConfigs[indexConfig]; // can be randomized later
        // Instantiate the snack prefab and set its properties
        Snack snack = snackPrefabTemplate.GetComponent<Snack>();

        // snack setup
        snack = setupSnack(config, snack);
        // instantiate the snack and add to the stack
        GameObject newSnack = Instantiate(snack.gameObject, transform);
        snackStack.Push(newSnack.GetComponent<Snack>());
    }

    private Snack setupSnack(SnackConfig config, Snack snack)
    {
        snack.setConfig(config);
        snack.setInitScale((snackStack.Count + 1) * scaleFactor);
        snack.setLeaveDuration((maxSnacks - snackStack.Count) * leaveDurationSpeedFactor);
        snack.setOrderInLayer(snackStack.Count);
        return snack;
    }
    private Snack setupRock(SnackConfig config, Snack snack)
    {
        snack.setConfig(config);
        snack.setInitScale((snackStack.Count + 1) * scaleFactor);
        snack.setLeaveDuration(0.5f); // rock snacks have a fixed leave duration
        snack.setOrderInLayer(snackStack.Count);
        return snack;
    }

    public void multiplyLeaveDurationSpeedFactor(float newSpeedFactor)
    {
        leaveDurationSpeedFactor *= newSpeedFactor;
        // if the factor changes, we just need to multiply the current value by the new factor because it was already set 
        foreach (Snack snack in snackStack)
        {
            snack.setLeaveDuration(snack.getLeaveDuration() * leaveDurationSpeedFactor);
        }
    }
    private void addSnacks(int amount, int index)
    {
        if (index < 0 || index >= snackConfigs.Length)
        {
            Debug.LogError("Index out of range");
            return;
        }

        SnackConfig config = snackConfigs[index];
        for (int i = 0; i < amount; i++)
        {
            addSnack(index);
        }
    }
    public void fillSnacks()
    {
        addSnacks(maxSnacks - snackStack.Count, Random.Range(0, snackConfigs.Length));
    }
    public Snack releaseSnack()
    {

        if (snackStack.Count > 0)
        {

            Snack snack = snackStack.Pop();
            snack.transform.SetParent(null);
            snack.startLeavingSpiralTray();
            return snack;
        }
        return null;

    }

    public bool hasSnacksAvailable()
    {
        return snackStack.Count > 0;
    }

    internal Sprite getCurrentSnackSprite()
    {
        snackStack.TryPeek(out Snack snack);
        if (snack != null)
        {
            return snack.getConfig().icon;
        }
        return null;
    }
}
