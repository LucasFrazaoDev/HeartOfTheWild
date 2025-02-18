using System;
using UnityEngine;

public class PlayerDefendingState : PlayerBaseState
{
    private const float k_debuffSpeed = 0.75f;

    public PlayerDefendingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        m_stateMachine.Velocity.y = Physics.gravity.y;

        m_stateMachine.PlayerAnimator.CrossFadeMoveAnimation();
        m_stateMachine.PlayerAnimator.ToggleShieldDefense(1.0f);

        SubscribeInputEvents();
    }

    public override void Tick()
    {
        if (!m_stateMachine.CharController.isGrounded)
        {
            m_stateMachine.SwitchState(new PlayerFallState(m_stateMachine));
        }

        CalculateMoveDirection();
        FaceMoveDirection();
        Move(k_debuffSpeed);

        float speed = m_stateMachine.CharController.velocity.magnitude;
        m_stateMachine.PlayerAnimator.UpdateMoveAnimation(speed);
    }

    public override void Exit()
    {
        UnsubscribeInputEvents();
        m_stateMachine.PlayerAnimator.ToggleShieldDefense(0f);
    }

    private void SubscribeInputEvents()
    {
        EventBus.Instance.Subscribe<InputReaderEvents.ShieldDefenseEvent>(SwitchToMoveState);
    }

    private void UnsubscribeInputEvents()
    {
        EventBus.Instance.Unsubscribe<InputReaderEvents.ShieldDefenseEvent>(SwitchToMoveState);
    }

    private void SwitchToMoveState(InputReaderEvents.ShieldDefenseEvent onShieldUp)
    {
        m_stateMachine.SwitchState(new PlayerMoveState(m_stateMachine));
    }
}
