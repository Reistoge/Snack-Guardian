using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour, IInteractor
{
    [SerializeField] Player player;
    [SerializeField] LayerMask EnemyLayerInteract;


    public void interactionWithSnack(ObjectEffect effect)
    {

        if (effect == null) return;
        if (!player.isDashing() && effect.type == EffectType.Nothing)
        {
            player.bump();
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
