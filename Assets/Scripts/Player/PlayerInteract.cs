using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour, IInteractor
{
    [SerializeField] Player player;
    // Start is called before the first frame update
    // void Start()
    // {

    // }

    // Update is called once per frame
    // void Update()
    // {

    // }
    // void OnTriggerEnter2D(Collider2D collider){


    // }


    // template dont change this, 
    // this method pass the effect to the main player script
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
