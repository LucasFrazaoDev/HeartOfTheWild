using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine m_stateMachine;
    private Vector3 m_currentVelocity;
    private Vector3 m_lastMoveDirection;
    private const float k_directionChangeThreshold = 0.7f;
    private bool m_shouldResetVelocity;

    public void Initialize(PlayerStateMachine stateMachine)
    {
        m_stateMachine = stateMachine;
    }

    public virtual void ResetState()
    {
        m_currentVelocity = Vector3.zero;
        m_lastMoveDirection = Vector3.zero;
        m_shouldResetVelocity = true;
    }

    public override void Exit()
    {
        m_stateMachine.ReturnStateToPool(this);
    }

    protected void CalculateMoveDirection()
    {
        Vector2 input = m_stateMachine.GameInput.GetMovementVectorNormalized();

        Vector3 cameraForward = new Vector3(m_stateMachine.MainCamera.forward.x, 0,
                                            m_stateMachine.MainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(m_stateMachine.MainCamera.right.x, 0,
                                          m_stateMachine.MainCamera.right.z).normalized;

        Vector3 moveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

        // Detecta mudança significativa de direção
        if (moveDirection != Vector3.zero &&
            Vector3.Dot(moveDirection, m_lastMoveDirection) < k_directionChangeThreshold)
        {
            m_shouldResetVelocity = true;
        }

        m_lastMoveDirection = moveDirection;

        Vector3 targetVelocity = moveDirection * m_stateMachine.MovementSpeed;

        if (m_shouldResetVelocity)
        {
            m_currentVelocity = targetVelocity;
            m_shouldResetVelocity = false;
        }
        else
        {
            // Aplica aceleração gradual
            m_currentVelocity = Vector3.Lerp(m_currentVelocity, targetVelocity,
                                             Time.deltaTime * m_stateMachine.Acceleration);
        }

        m_stateMachine.Velocity.x = m_currentVelocity.x;
        m_stateMachine.Velocity.z = m_currentVelocity.z;
    }

    protected void FaceMoveDirection()
    {
        if (m_lastMoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(m_lastMoveDirection, Vector3.up);
            m_stateMachine.transform.rotation = Quaternion.Slerp(
                m_stateMachine.transform.rotation,
                targetRotation,
                m_stateMachine.LookRotationDampFactor * Time.deltaTime);
        }
    }

    protected void ApplyGravity()
    {
        float gravity = Physics.gravity.y * m_stateMachine.GravityMultiplier;
        m_stateMachine.Velocity.y += gravity * Time.deltaTime;

        if (m_stateMachine.Velocity.y < m_stateMachine.MaxFallSpeed)
            m_stateMachine.Velocity.y = m_stateMachine.MaxFallSpeed;
    }

    protected void Move(float multiplier = 1.0f)
    {
        m_stateMachine.CharController.Move(m_stateMachine.Velocity * multiplier * Time.deltaTime);
    }
}