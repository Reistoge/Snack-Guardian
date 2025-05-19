using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour, IInteractor
{
    [SerializeField] Player player;
    [SerializeField] LayerMask EnemyLayerInteract;

    public void applyEffect(ObjectEffect effect)
    {
        if (player != null)
        {
            switch (effect.type)
            {
                case EffectType.Heal:
                    player.heal(effect.amount);
                    print("Heal the player");
                    break;
                case EffectType.Damage:
                    
                    player.takeDamage(effect.amount);
                    print("Damage the player");
                    break;
            }
            // pass the things to the main object.
            player.applyEffect(effect);

        }


    }
    

}
