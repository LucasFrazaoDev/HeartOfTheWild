using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public override void Enter()
    {
        m_stateMachine.Velocity = new Vector3(m_stateMachine.Velocity.x, m_stateMachine.JumpForce, m_stateMachine.Velocity.z);

        m_stateMachine.PlayerAnimator.PlayJumpAnimation();
    }

    public override void Tick()
    {
        ApplyGravity();

        if (m_stateMachine.Velocity.y <= 0f)
        {
            m_stateMachine.SwitchToState<PlayerFallState>();
        }

        FaceMoveDirection();
        Move();
    }

    public override void Exit(){ }
}