using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class MainMenuCigarette : MonoBehaviour
{
    // Start is called before the first frame update
    public static event Action onCigarette;
    [SerializeField] AudioConfig zippoLight;
    [SerializeField] AudioConfig zippoOpen;
    [SerializeField] Animator animator;
    [SerializeField] UnityEvent onCigaretteEvent;

    const string ZIPPO_LIGHT = "ZippoLight";
    const string ZIPPO_On = "ZippoOn";
    readonly int ZIPPO_LIGHT_HASH = Animator.StringToHash(ZIPPO_LIGHT); 
    readonly int ZIPPO_ON_HASH = Animator.StringToHash(ZIPPO_On);
    void Start()
    {

        StartCoroutine(startZippo());
    }
    IEnumerator startZippo()
    {

        AudioManager.Instance.playSFX(zippoOpen);
        yield return new WaitForSeconds(zippoOpen.clip.length);
        int times = Random.Range(1, 5);
        for (int i = 0; i < times; i++)
        {
            animator.Play(ZIPPO_LIGHT_HASH, -1, 0f);
            AudioManager.Instance.playSFX(zippoLight);
            if (i != times - 1)
            {
                yield return new WaitForSeconds(zippoLight.clip.length + Random.Range(0.5f, 1f));
            }



        }
        animator.Play(ZIPPO_ON_HASH, -1, 0f);
        AudioManager.Instance.playMainMenuMusic();
        onCigarette?.Invoke();
        onCigaretteEvent?.Invoke();
    }




}
