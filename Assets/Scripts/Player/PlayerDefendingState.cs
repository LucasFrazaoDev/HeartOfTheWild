using UnityEngine;

public class PlayerDefendingState : PlayerBaseState
{
    private const float k_debuffSpeed = 0.75f;

    public override void Enter()
    {
        m_stateMachine.Velocity.y = Physics.gravity.y;

        m_stateMachine.PlayerAnimator.CrossFadeMoveAnimation();
        //m_stateMachine.PlayerAnimator.ToggleShieldDefense(1.0f);

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
        Move(k_debuffSpeed);

        float speed = m_stateMachine.CharController.velocity.magnitude;
        m_stateMachine.PlayerAnimator.UpdateMoveAnimation(speed);
    }

    public override void Exit()
    {
        base.Exit();
        UnsubscribeInputEvents();
        //m_stateMachine.PlayerAnimator.ToggleShieldDefense(0f);
    }

    private void SubscribeInputEvents()
    {
        m_stateMachine.GameInput.OnShieldDefenseCanceled += SwitchToMoveState;
    }

    private void UnsubscribeInputEvents()
    {
        m_stateMachine.GameInput.OnShieldDefenseCanceled -= SwitchToMoveState;
    }

    private void SwitchToMoveState()
    {
        m_stateMachine.SwitchToState<PlayerMoveState>();
    }
}