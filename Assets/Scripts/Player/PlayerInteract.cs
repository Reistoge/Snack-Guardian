using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour, IInteractor
{
    [SerializeField] Player player;
    [SerializeField] LayerMask EnemyLayerInteract;
    
    private ParticleSystem playerBumpParticles;
    [SerializeField] ObjectEffect bumpEffect;

    void Start()
    {

        playerBumpParticles = Instantiate(bumpEffect.effectParticles).GetComponent<ParticleSystem>();
        
         
    }
    public void interactionWithSnack(ObjectEffect effect)
    {

        if (effect == null) return;
        if (!player.isDashing() && effect.type == EffectType.Nothing)
        {
            player.bump();

            playerBumpParticles.transform.position = player.transform.position;
            playerBumpParticles.Play();
             
            GameEvents.triggerSnackCaptured(false);
            StartCoroutine(stopTime(0.1f));

        }
        if (player.isDashing())
        {
            GameEvents.triggerSnackCaptured(true);
            StartCoroutine(stopTime(0.1f));
            print("Player destroys the snack while dashing .");
        }
        // if (player.isDashing())
        // {
        //     player.bump();
        // }


        // if (effect.type == EffectType.Heal)
        // { 
        //     player.bump();
        // }

    }
    IEnumerator stopTime(float time)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
         
    }

    public void applyEffect(ObjectEffect effect)
    {
        if (player == null) return;
        switch (effect.type)
        {
            case EffectType.Heal:
                player.heal(effect.amount);
                break;

            case EffectType.Damage:
                if (!player.isDashing())
                {
                    player.takeDamage(effect.amount);
                    player.applyEffect(effect);
                }
                break;
            case EffectType.Nothing:
                player.applyEffect(effect);
                break;

        }

    }

}
