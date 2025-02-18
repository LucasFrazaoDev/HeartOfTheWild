using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    private Vector3 currentVelocity;
    protected readonly PlayerStateMachine m_stateMachine;

    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        m_stateMachine = stateMachine;
    }

    protected void CalculateMoveDirection()
    {
        float directionX = m_stateMachine.GameInput.GetMovementVectorNormalized().x;
        float directionY = m_stateMachine.GameInput.GetMovementVectorNormalized().y;

        Vector3 cameraForward = new Vector3(m_stateMachine.MainCamera.forward.x, 0, m_stateMachine.MainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(m_stateMachine.MainCamera.right.x, 0, m_stateMachine.MainCamera.right.z).normalized;

        Vector3 targetVelocity = (cameraForward * directionY + cameraRight * directionX) * m_stateMachine.MovementSpeed;

        // Slowly adjust the speed
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * m_stateMachine.Acceleration);

        m_stateMachine.Velocity.x = currentVelocity.x;
        m_stateMachine.Velocity.z = currentVelocity.z;
    }

    protected void FaceMoveDirection()
    {
        float directionX = m_stateMachine.GameInput.GetMovementVectorNormalized().x;
        float directionY = m_stateMachine.GameInput.GetMovementVectorNormalized().y;

        Vector3 cameraForward = new Vector3(m_stateMachine.MainCamera.forward.x, 0, m_stateMachine.MainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(m_stateMachine.MainCamera.right.x, 0, m_stateMachine.MainCamera.right.z).normalized;

        Vector3 moveDirection = cameraForward * directionY + cameraRight * directionX;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            m_stateMachine.transform.rotation = Quaternion.Slerp(m_stateMachine.transform.rotation, targetRotation, m_stateMachine.LookRotationDampFactor * Time.deltaTime);
        }
    }



    protected void ApplyGravity()
    {
        float gravity = Physics.gravity.y * m_stateMachine.GravityMultiplier;
        m_stateMachine.Velocity.y += gravity * Time.deltaTime;

        // Limite for max speed falling
        if (m_stateMachine.Velocity.y < m_stateMachine.MaxFallSpeed)
            m_stateMachine.Velocity.y = m_stateMachine.MaxFallSpeed;
    }


    protected void Move(float multiplier = 1.0f)
    {
        // Multiplier value to change speed based on moving states
        m_stateMachine.CharController.Move(m_stateMachine.Velocity * multiplier * Time.deltaTime);
    }
}