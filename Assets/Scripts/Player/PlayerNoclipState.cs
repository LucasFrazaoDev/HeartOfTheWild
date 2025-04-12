using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNoclipState : PlayerBaseState
{
    private float m_flySpeed = 10f;
    private float m_lookSpeed = 2f;
    private float m_boostMultiplier = 3f;
    private Vector3 m_moveInput;

    public override void Enter()
    {
        // Reset speed
        m_stateMachine.Velocity = Vector3.zero;
    }

    public override void Tick()
    {
        HandleMovementInput();
        HandleRotation();
        MovePlayer();
    }

    private void HandleMovementInput()
    {
        Vector2 input = m_stateMachine.GameInput.GetMovementVectorNormalized();
        float verticalInput = 0f;

        // Flying controls (Space/Ctrl)
        if (Keyboard.current.spaceKey.isPressed) verticalInput = 1f;
        if (Keyboard.current.ctrlKey.isPressed) verticalInput = -1f;

        m_moveInput = new Vector3(input.x, verticalInput, input.y);
        m_boostMultiplier = Keyboard.current.shiftKey.isPressed ? 3f : 1f;
    }

    private void HandleRotation()
    {
        Vector2 lookInput = m_stateMachine.GameInput.GetLookVector();
        if (lookInput != Vector2.zero)
        {
            Vector3 rotation = new Vector3(-lookInput.y, lookInput.x, 0) * m_lookSpeed;
            m_stateMachine.transform.Rotate(rotation);
        }
    }

    private void MovePlayer()
    {
        m_stateMachine.transform.Translate(
            m_moveInput * m_flySpeed * m_boostMultiplier * Time.deltaTime
        );
    }

    public override void Exit()
    {
        
    }

    public void SetFlySpeed(float speed)
    {
        m_flySpeed = Mathf.Clamp(speed, 1f, 100f);
    }
}