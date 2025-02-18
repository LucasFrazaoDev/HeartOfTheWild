using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) { }

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
            m_stateMachine.SwitchState(new PlayerMoveState(m_stateMachine));
        }
    }

    public override void Exit()
    {

    }
}