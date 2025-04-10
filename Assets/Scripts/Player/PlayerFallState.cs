using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public override void Enter()
    {
        m_stateMachine.Velocity.y = 0f;
        m_stateMachine.PlayerAnimator.PlayFallAnimation();
    }

    public override void Tick()
    {
        ApplyGravity();
        Move();
        FaceMoveDirection();

        if (m_stateMachine.CharController.isGrounded)
        {
            m_stateMachine.SwitchToState<PlayerMoveState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}