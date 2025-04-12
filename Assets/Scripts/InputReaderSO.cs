using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReaderSO : ScriptableObject, PlayerInputController.IPlayerActions,
                                               PlayerInputController.IUIActions,
                                               PlayerInputController.IDevConsoleActions
{
    // Gameplay Events
    public event Action OnJumpPerformed;
    public event Action OnRunPerformed;
    public event Action OnAttackPerformed;
    public event Action OnShieldDefenseStarted;
    public event Action OnShieldDefenseCanceled;
    public event Action OnPausePerformed;

    // Dev Console Events
    public event Action OnToggleConsolePerformed;

    // UI Events
    public event Action<Vector2> OnNavigatePerformed;
    public event Action OnUISubmitPerformed;
    public event Action OnUICancelPerformed;

    private PlayerInputController m_playerInput;
    private bool m_isInUIMode = false;

    public bool IsInUIMode => m_isInUIMode;

    private void OnEnable()
    {
        InitializePlayerInput();
    }

    private void OnDisable()
    {
        CleanupPlayerInput();
    }

    private void InitializePlayerInput()
    {
        if (m_playerInput == null)
        {
            m_playerInput = new PlayerInputController();
            m_playerInput.Player.SetCallbacks(this);
            m_playerInput.UI.SetCallbacks(this);
            m_playerInput.DevConsole.SetCallbacks(this);

            m_playerInput.DevConsole.Enable();

            EnableUIMode();
        }
    }

    private void CleanupPlayerInput()
    {
        if (m_playerInput != null)
        {
            m_playerInput.Player.Jump.performed -= OnJump;
            m_playerInput.Player.Run.performed -= OnRun;
            m_playerInput.Player.ShieldDefense.performed -= OnShieldDefense;

            m_playerInput.UI.Navigate.performed -= OnNavigate;
            m_playerInput.UI.Submit.performed -= OnSubmit;
            m_playerInput.UI.Cancel.performed -= OnCancel;


            m_playerInput.Disable();
        }
    }

    public void EnableUIMode()
    {
        m_playerInput.Player.Disable();
        m_playerInput.UI.Enable();
        m_isInUIMode = true;
    }

    public void EnableGameplayMode()
    {
        m_playerInput.UI.Disable();
        m_playerInput.Player.Enable();
        m_isInUIMode = false;
    }

    #region GameplayActions
    public Vector2 GetMovementVectorNormalized()
    {
        return m_isInUIMode ? Vector2.zero : m_playerInput.Player.Move.ReadValue<Vector2>().normalized;
    }

    public Vector2 GetLookVector()
    {
        return m_isInUIMode ? Vector2.zero : m_playerInput.Player.Look.ReadValue<Vector2>().normalized;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!m_isInUIMode && context.performed)
            OnJumpPerformed?.Invoke();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (!m_isInUIMode && context.performed)
            OnRunPerformed?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!m_isInUIMode && context.performed)
            OnAttackPerformed?.Invoke();
    }

    public void OnShieldDefense(InputAction.CallbackContext context)
    {
        if (m_isInUIMode) return;

        if (context.started)
            OnShieldDefenseStarted?.Invoke();
        else if (context.canceled)
            OnShieldDefenseCanceled?.Invoke();
    }
    #endregion

    // UI Actions
    #region UIActions
    public void OnNavigate(InputAction.CallbackContext context)
    {
        //if (m_isInUIMode && context.performed)
        //    OnNavigatePerformed?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        //if (m_isInUIMode && context.performed)
        //    OnUISubmitPerformed?.Invoke();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (m_isInUIMode && context.performed)
            OnUICancelPerformed?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnPausePerformed?.Invoke();
    }

    // DevConsole
    public void OnToggleConsole(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnToggleConsolePerformed?.Invoke();
    }
    #endregion

    // Métodos da interface (não utilizados)
    public void OnMove(InputAction.CallbackContext context) { }
    public void OnLook(InputAction.CallbackContext context) { }
}