using System;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

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
            m_stateMachine.SwitchState(new PlayerFallState(m_stateMachine));
        }

        CalculateMoveDirection();
        FaceMoveDirection();
        Move();

        float speed = m_stateMachine.CharController.velocity.magnitude;
        m_stateMachine.PlayerAnimator.UpdateMoveAnimation(speed);
    }

    public override void Exit()
    {
        UnsubscribeInputEvents();
    }

    private void SubscribeInputEvents()
    {
        m_stateMachine.GameInput.OnJumpPerformed += SwitchToJumpState;
        m_stateMachine.GameInput.OnRunPerformed += SwitchToRunState;
        m_stateMachine.GameInput.OnAttackPerformed += SwitchToAttackState;
        m_stateMachine.GameInput.OnShieldDefenseStarted += SwitchToMoveWithShieldState;
    }

    private void UnsubscribeInputEvents()
    {
        m_stateMachine.GameInput.OnJumpPerformed -= SwitchToJumpState;
        m_stateMachine.GameInput.OnRunPerformed -= SwitchToRunState;
        m_stateMachine.GameInput.OnAttackPerformed -= SwitchToAttackState;
        m_stateMachine.GameInput.OnShieldDefenseStarted -= SwitchToMoveWithShieldState;
    }

    private void SwitchToJumpState()
    {
        m_stateMachine.SwitchState(new PlayerJumpState(m_stateMachine));
    }

    private void SwitchToRunState()
    {
        m_stateMachine.SwitchState(new PlayerRunState(m_stateMachine));
    }

    private void SwitchToAttackState()
    {
        m_stateMachine.SwitchState(new PlayerAttackState(m_stateMachine));
    }

    private void SwitchToMoveWithShieldState()
    {
        m_stateMachine.SwitchState(new PlayerDefendingState(m_stateMachine));
    }
}
