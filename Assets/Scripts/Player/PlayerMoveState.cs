using System;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public override void Enter()
    {
        m_stateMachine.Velocity.y = Physics.gravity.y;
        m_stateMachine.PlayerAnimator.CrossFadeMoveAnimation();
        SubscribeInputEvents();
    }

    public override void Tick()
    {
        if (!m_stateMachine.CharController.isGrounded)
        {
            m_stateMachine.SwitchToState<PlayerFallState>();
            return;
        }

        CalculateMoveDirection();
        FaceMoveDirection();
        Move();

        float speed = m_stateMachine.CharController.velocity.magnitude;
        m_stateMachine.PlayerAnimator.UpdateMoveAnimation(speed);
    }

    public override void Exit()
    {
        base.Exit();
        UnsubscribeInputEvents();
    }

    private void SubscribeInputEvents()
    {
        m_stateMachine.GameInput.OnJumpPerformed += SwitchToJumpState;
        m_stateMachine.GameInput.OnRunPerformed += SwitchToRunState;
        m_stateMachine.GameInput.OnAttackPerformed += SwitchToAttackState;
        m_stateMachine.GameInput.OnShieldDefenseStarted += SwitchToDefendingState;
    }

    private void UnsubscribeInputEvents()
    {
        m_stateMachine.GameInput.OnJumpPerformed -= SwitchToJumpState;
        m_stateMachine.GameInput.OnRunPerformed -= SwitchToRunState;
        m_stateMachine.GameInput.OnAttackPerformed -= SwitchToAttackState;
        m_stateMachine.GameInput.OnShieldDefenseStarted -= SwitchToDefendingState;
    }

    private void SwitchToJumpState()
    {
        m_stateMachine.SwitchToState<PlayerJumpState>();
    }

    private void SwitchToRunState()
    {
        m_stateMachine.SwitchToState<PlayerRunState>();
    }

    private void SwitchToAttackState()
    {
        m_stateMachine.SwitchToState<PlayerAttackState>();
    }

    private void SwitchToDefendingState()
    {
        m_stateMachine.SwitchToState<PlayerDefendingState>();
    }
}
