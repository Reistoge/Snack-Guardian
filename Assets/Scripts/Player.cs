using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerAnimator playerAnimation;

    public PlayerMovement Movement { get => movement; set => movement = value; }
    public PlayerAnimator PlayerAnimation { get => playerAnimation; set => playerAnimation = value; }
    public PlayerStateMachine StateMachine { get => stateMachine; set => stateMachine = value; }

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        stateMachine.Initialize(new IdleState(this));
    }
    private void Update()
    {
        stateMachine.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
     
    }


}
