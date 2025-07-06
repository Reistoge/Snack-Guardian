using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventsAttacksHandler : MonoBehaviour
{
    [SerializeField] GameObject[] gameDebuffPrefab;
    List<GameDebuff> gameDebuffs = new List<GameDebuff>();
    [SerializeField] Queue<GameDebuff> debuffExecutionQueue = new Queue<GameDebuff>();

    GameDebuff currentDebuff;
    Coroutine executionCoroutine;

    public static EventsAttacksHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        // Do NOT call DontDestroyOnLoad, so it won't persist between scenes
    }
    [SerializeField] int maxDebuffs = 5; // Maximum number of debuffs that can be active at once
    void Start()
    {
        for (int i = 0; i < gameDebuffPrefab.Length; i++)
        {
            GameObject go = Instantiate(gameDebuffPrefab[i], transform);
            GameDebuff gameDebuff = go.GetComponent<GameDebuff>();
            if (gameDebuff != null)
            {
                gameDebuffs.Add(gameDebuff);
            }
            else
            {
                Debug.LogWarning($"GameDebuff component not found on {go.name}");
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {

            // applyRandomDebuff();
        }
    }
    // public void applyRandomDebuff()
    // {
    //     if(currentDebuff != null )
    //     {

    //         Debug.LogWarning("A debuff is already active. Please wait for it to finish before applying a new one.");
    //         return;
    //     }
    //     currentDebuff = gameDebuffs[Random.Range(0, gameDebuffs.Count)].applyDebuff();
    //     StartCoroutine(waitToDebuffToEndCoroutine(currentDebuff));
    // }
    public void applyRandomDebuff()
    {

        // currentDebuff = gameDebuffs[Random.Range(0, gameDebuffs.Count)].applyDebuff();
        //debuffExecutionQueue.Enqueue();
        applyDebuff(gameDebuffs[Random.Range(0, gameDebuffs.Count)]);
    }

    private void applyDebuff(GameDebuff gameDebuff)
    {

        if (debuffExecutionQueue.Count == 0 && executionCoroutine == null)
        {
            Debug.Log("Starting debuff execution coroutine for: " + gameDebuff);
            debuffExecutionQueue.Enqueue(gameDebuff);

            executionCoroutine = StartCoroutine(handleDebuffExecutionQueueCoroutine());
        }
        else if (debuffExecutionQueue.Count > 0 && debuffExecutionQueue.Count < maxDebuffs && executionCoroutine != null)
        {
            // If the queue is not full, add the debuff to the queue
            Debug.Log("Adding debuff to queue: " + gameDebuff.getDebuffName());
            debuffExecutionQueue.Enqueue(gameDebuff);
        }
        else
        {
            Debug.LogWarning("Debuff queue is full. Cannot apply new debuff: " + gameDebuff.getDebuffName());
        }



    }
    private void applyDebuff(debuffType type)
    {
        // Find the GameDebuff instance that matches the given debuffType
        GameDebuff gameDebuff = gameDebuffs.Find(d => d.getDebuffName() == type.ToString());

        if (gameDebuff == null)
        {
            Debug.LogWarning("No GameDebuff found for type: " + type);
            return;
        }

        if (debuffExecutionQueue.Count == 0 && executionCoroutine == null)
        {
            Debug.Log("Starting debuff execution coroutine for: " + type);
            debuffExecutionQueue.Enqueue(gameDebuff);
            executionCoroutine = StartCoroutine(handleDebuffExecutionQueueCoroutine());
        }
        else if (debuffExecutionQueue.Count > 0 && debuffExecutionQueue.Count < maxDebuffs && executionCoroutine != null)
        {
            Debug.Log("Adding debuff to queue: " + gameDebuff.getDebuffName());
            debuffExecutionQueue.Enqueue(gameDebuff);
        }
        else
        {
            Debug.LogWarning("Debuff queue is full. Cannot apply new debuff: " + gameDebuff.getDebuffName());
        }
    }
    public void applyWeakDebuff()
    {
        applyDebuff(debuffType.flipCamera);

    }
    public void applyMediumDebuff()
    {
        applyDebuff(debuffType.turnOffLights);
    
    }
    public void applyStrongDebuff()
    {
        applyDebuff(debuffType.spawnRocks);
    }



    IEnumerator handleDebuffExecutionQueueCoroutine()
    {
        yield return new WaitForEndOfFrame(); // Ensure the coroutine starts after the frame update
        while (debuffExecutionQueue.Count > 0)
        {

            GameDebuff debuffToExecute = debuffExecutionQueue.Peek();
            currentDebuff = debuffToExecute.applyDebuff();
            yield return waitToDebuffToEndCoroutine(currentDebuff);
            yield return new WaitForEndOfFrame(); // Wait for the end of the frame to ensure the debuff is applied before continuing
            Debug.Log("Debuff executed: " + debuffToExecute.getDebuffName());
            yield return new WaitForSeconds(1f);
            debuffExecutionQueue.Dequeue(); // Remove the debuff from the queue after execution
            // Optional delay between debuffs
            // if (debuffExecutionQueue.Count == 0)
            // {
            //     yield return new WaitUntil(() => debuffExecutionQueue.Count != 0);
            //     yield return new WaitForEndOfFrame();
            // }

        }
        executionCoroutine = null; // Reset the coroutine reference when done
    }


    IEnumerator waitToDebuffToEndCoroutine(GameDebuff debuff)
    {
        if (debuff != null)
        {

            yield return new WaitUntil(() => debuff.DebuffIsActive == true);
            yield return new WaitUntil(() => debuff.DebuffIsActive == false);

        }
    }
    public enum debuffType
    {
        flipCamera,
        spawnRocks,
        turnOffLights,

    }
}
