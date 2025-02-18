using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    private const float k_runSpeedAddition = 4.0f;

    private const float k_maxRunTime = 5.0f;
    private float m_currentRunTime = 0.0f;

    public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        m_stateMachine.Velocity.y = Physics.gravity.y;

        m_stateMachine.MovementSpeed += k_runSpeedAddition;

        m_stateMachine.PlayerAnimator.CrossFadeMoveAnimation();
        EventBus.Instance.Subscribe<InputReaderEvents.JumpEvent>(SwitchToJumpState);
    }

    public override void Tick()
    {
        m_currentRunTime += Time.deltaTime;

        if (m_currentRunTime >= k_maxRunTime || m_stateMachine.GameInput.GetMovementVectorNormalized() == Vector2.zero)
        {
            m_currentRunTime = 0.0f;
            m_stateMachine.SwitchState(new PlayerMoveState(m_stateMachine));
        }

        if (!m_stateMachine.CharController.isGrounded)
        {
            m_stateMachine.SwitchState(new PlayerFallState(m_stateMachine));
        }

        CalculateMoveDirection();
        FaceMoveDirection();
        Move();

        m_stateMachine.PlayerAnimator.UpdateMoveAnimation(m_stateMachine.CharController.velocity.magnitude);
    }

    public override void Exit()
    {
        m_stateMachine.MovementSpeed -= k_runSpeedAddition;
        EventBus.Instance.Unsubscribe<InputReaderEvents.JumpEvent>(SwitchToJumpState);
    }

    private void SwitchToJumpState(InputReaderEvents.JumpEvent jump)
    {
        m_stateMachine.SwitchState(new PlayerJumpState(m_stateMachine));
    }
}

