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
        EventBus.Instance.Subscribe<InputReaderEvents.JumpEvent>(SwitchToJumpState);
        EventBus.Instance.Subscribe<InputReaderEvents.AttackEvent>(SwitchToAttackState);
        EventBus.Instance.Subscribe<InputReaderEvents.ShieldDefenseEvent>(SwitchToMoveWithShieldState);
        EventBus.Instance.Subscribe<InputReaderEvents.RunEvent>(SwitchToRunState);
    }

    private void UnsubscribeInputEvents()
    {
        EventBus.Instance.Unsubscribe<InputReaderEvents.JumpEvent>(SwitchToJumpState);
        EventBus.Instance.Unsubscribe<InputReaderEvents.AttackEvent>(SwitchToAttackState);
        EventBus.Instance.Unsubscribe<InputReaderEvents.ShieldDefenseEvent>(SwitchToMoveWithShieldState);
        EventBus.Instance.Unsubscribe<InputReaderEvents.RunEvent>(SwitchToRunState);
    }

    private void SwitchToJumpState(InputReaderEvents.JumpEvent jump)
    {
        m_stateMachine.SwitchState(new PlayerJumpState(m_stateMachine));
    }

    private void SwitchToRunState(InputReaderEvents.RunEvent run)
    {
        m_stateMachine.SwitchState(new PlayerRunState(m_stateMachine));
    }

    private void SwitchToAttackState(InputReaderEvents.AttackEvent attack)
    {
        m_stateMachine.SwitchState(new PlayerAttackState(m_stateMachine));
    }

    private void SwitchToMoveWithShieldState(InputReaderEvents.ShieldDefenseEvent shieldUp)
    {
        m_stateMachine.SwitchState(new PlayerDefendingState(m_stateMachine));
    }
}
