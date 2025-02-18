using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReaderSO : ScriptableObject, PlayerInputController.IPlayerActions
{
    private PlayerInputController m_playerInput;

    // Inicialização
    private void OnEnable()
    {
        if (m_playerInput == null)
        {
            m_playerInput = new PlayerInputController();
            m_playerInput.Player.SetCallbacks(this);

            // Registra os eventos
            m_playerInput.Player.Jump.performed += OnJump;
            m_playerInput.Player.Run.performed += OnRun;
            m_playerInput.Player.ShieldDefense.performed += OnShieldDefense;

            m_playerInput.Player.Enable();
        }
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = m_playerInput.Player.Move.ReadValue<Vector2>();
        return inputVector.normalized;
    }

    public Vector2 GetLookVector()
    {
        Vector2 inputVector = m_playerInput.Player.Look.ReadValue<Vector2>();
        return inputVector.normalized;
    }

    private void OnDisable()
    {
        if (m_playerInput != null)
        {
            // Remove os eventos
            m_playerInput.Player.Jump.performed -= OnJump;
            m_playerInput.Player.Run.performed -= OnRun;
            m_playerInput.Player.ShieldDefense.performed -= OnShieldDefense;

            m_playerInput.Disable();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Publica o evento de pulo
        EventBus.Instance.Publish(new InputReaderEvents.JumpEvent());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        // Publica o evento de corrida
        EventBus.Instance.Publish(new InputReaderEvents.RunEvent());
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Publica o evento de ataque
            EventBus.Instance.Publish(new InputReaderEvents.AttackEvent());
        }
    }

    public void OnShieldDefense(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Publica o evento de defesa com escudo
            EventBus.Instance.Publish(new InputReaderEvents.ShieldDefenseEvent());
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Method exists because interface demands
        // Move read values in GetMovementVectorNormalized()
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Method exists because interface demands
        // Look read values in CameraController script
    }
}