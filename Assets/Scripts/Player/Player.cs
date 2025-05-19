using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class Player : MonoBehaviour, IInteractor, IInteractable
{
    private PlayerStateMachine stateMachine;
    [SerializeField] PlayerMovement movement;

    [SerializeField] PlayerAnimHandler anim;
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] ObjectEffect playerEffect;



    public PlayerMovement Movement { get => movement; set => movement = value; }
    public PlayerAnimHandler PlayerAnimation { get => anim; set => anim = value; }
    public PlayerStateMachine StateMachine { get => stateMachine; set => stateMachine = value; }

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        stateMachine.Initialize(new IdleState(this));
    }
    void OnEnable()
    {
        // healthSystem.onDamageTaken += OnDamageTaken;
        healthSystem.onDie += die;
        
        healthSystem.setIsTakingDamageTime(anim.getDamagedAnimationDuration());
        
         

         
        
    }
    public void die(){
        GameEvents.triggerPlayerDeath();

    }
    void OnDisable()
    {
        // healthSystem.onDamageTaken -= OnDamageTaken;
        // healthSystem.onDie -= ()=> {Destroy(gameObject);};
         GameEvents.onPlayerDeath -= die;
         
        
    }
    private void Update()
    {
        stateMachine.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();

    }
    void Start()
    {
        
    }
    public void onInteract(IInteractor interactor)
    {
        // what happens to the player when it interacts with the object ?
        interactor.applyEffect(playerEffect);


        // throw new System.NotImplementedException();
    }
    public void applyEffect(ObjectEffect effect)
    {
        // // here we apply the effect to the player when interacts with the object
        // switch (effect.type)
        // {
        //     case EffectType.Heal:
        //         print("Heal the player");
        //         healthSystem.heal(effect.amount);

        //         // currentHealth = Mathf.Min(maxHealth, currentHealth + effect.amount);
        //         break;
        //     case EffectType.Damage:
        //         print("Damage the player");
        //         healthSystem.takeDamage(effect.amount);
        //         // currentHealth = Mathf.Max(0, currentHealth - effect.amount);
        //         break;
        // }

        // OnHealthChanged?.Invoke(currentHealth);

        if (effect.effectParticles != null)
        {
            Instantiate(effect.effectParticles, transform.position, Quaternion.identity);
        }
    }
    public void heal(float amount)
    {
        // here we heal the player when interacts with the object
        healthSystem.handleHeal(amount);
    }
    public void takeDamage(float amount)
    {
        // here we damage the player when interacts with the object
        if (healthSystem.canTakeDamage())
        {
            healthSystem.handleDamage(amount);
            
            
        }
        
        // here we check if the player is damaged

    }
    public void setIsInvincible(bool isInvincible)
    {
        healthSystem.setIsInvincible( isInvincible);
    }
    
    

        
    
    public void activateDamagedMovement()
    {
        // here we change the player state to damaged
        movement.changeToDamagedMovement();

    }
    public void resetMovementStats()
    {
        // reset to the default movements stats
        movement.resetMovementStats();
    }
    public bool isTakingDamage()
    {
        // here we check if the player is damaged by using the health system
        return healthSystem.getIsTakingDamage();
    }
    public void playDamagedAnimation()
    {
        anim.playDamagedAnimation();
    }
    public void playerBump()
    {
        // here we play the bump animation
        

    }

    public void stopMovement()
    {
        movement.stopMovement();
    }
 

}
