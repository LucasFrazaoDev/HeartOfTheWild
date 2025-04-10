using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class PlayerRunState : PlayerBaseState
{
    private const float k_runSpeedAddition = 4.0f;
    private const float k_maxRunTime = 5.0f;
    private const float k_speedTransitionTime = 0.3f;

    private float m_currentRunTime;
    private float m_originalSpeed;
    private float m_speedTransitionVelocity;
    private CancellationTokenSource m_speedTransitionCTS;

    public override void Enter()
    {
        m_originalSpeed = m_stateMachine.MovementSpeed;
        m_stateMachine.Velocity.y = Physics.gravity.y;

        // Smooth transition
        m_speedTransitionCTS = new CancellationTokenSource();
        SmoothSpeedTransitionAsync().Forget();

        m_stateMachine.PlayerAnimator.CrossFadeMoveAnimation();
        m_stateMachine.GameInput.OnJumpPerformed += SwitchToJumpState;
    }

    public override void Tick()
    {
        m_currentRunTime += Time.deltaTime;

        if (m_currentRunTime >= k_maxRunTime ||
            m_stateMachine.GameInput.GetMovementVectorNormalized() == Vector2.zero)
        {
            m_stateMachine.SwitchToState<PlayerMoveState>();
            return;
        }

        if (!m_stateMachine.CharController.isGrounded)
        {
            m_stateMachine.SwitchToState<PlayerFallState>();
            return;
        }

        CalculateMoveDirection();
        FaceMoveDirection();
        Move();

        m_stateMachine.PlayerAnimator.UpdateMoveAnimation(
            m_stateMachine.CharController.velocity.magnitude);
    }

    public override void Exit()
    {
        base.Exit();
        // Cancel transition if it is still running
        m_speedTransitionCTS?.Cancel();
        m_speedTransitionCTS?.Dispose();

        m_stateMachine.MovementSpeed = m_originalSpeed;
        m_stateMachine.GameInput.OnJumpPerformed -= SwitchToJumpState;
    }

    public override void ResetState()
    {
        m_currentRunTime = 0f;
    }

    private async UniTaskVoid SmoothSpeedTransitionAsync()
    {
        float targetSpeed = m_originalSpeed + k_runSpeedAddition;
        float currentSpeed = m_stateMachine.MovementSpeed;

        try
        {
            while (Mathf.Abs(currentSpeed - targetSpeed) > 0.1f)
            {
                currentSpeed = Mathf.SmoothDamp(
                    currentSpeed,
                    targetSpeed,
                    ref m_speedTransitionVelocity,
                    k_speedTransitionTime);

                m_stateMachine.MovementSpeed = currentSpeed;
                await UniTask.Yield(m_speedTransitionCTS.Token);
            }

            m_stateMachine.MovementSpeed = targetSpeed;
        }
        catch (OperationCanceledException) { }
    }

    private void SwitchToJumpState()
    {
        m_stateMachine.SwitchToState<PlayerJumpState>();
    }
}