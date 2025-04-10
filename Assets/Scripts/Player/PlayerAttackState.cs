using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int m_currentComboStep;
    private float m_comboTimer;
    private const float k_comboResetTime = 1.0f;
    private bool m_isAttacking;

    public override void Enter()
    {
        ResetState(); // Reutiliza o mesmo método de reset
        PerformAttack().Forget();
        m_stateMachine.GameInput.OnAttackPerformed += OnAttackPressed;
    }

    public override void Tick()
    {
        FaceMoveDirection();
        m_comboTimer += Time.deltaTime;

        if (m_comboTimer > k_comboResetTime)
        {
            ResetCombo();
        }
    }

    public override void Exit()
    {
        base.Exit(); // Importante para retornar ao pool
        m_stateMachine.GameInput.OnAttackPerformed -= OnAttackPressed;
    }

    public override void ResetState()
    {
        m_currentComboStep = 0;
        m_comboTimer = 0f;
        m_isAttacking = false;
    }

    private void OnAttackPressed()
    {
        if (!m_isAttacking && m_currentComboStep < m_stateMachine.AttackCombo.Length - 1)
        {
            m_currentComboStep++;
            PerformAttack().Forget();
        }
    }

    private async UniTaskVoid PerformAttack()
    {
        m_isAttacking = true;
        m_comboTimer = 0f;

        if (m_currentComboStep < m_stateMachine.AttackCombo.Length)
        {
            AttackDataSO attack = m_stateMachine.AttackCombo[m_currentComboStep];
            m_stateMachine.PlayerAnimator.PlayAttackAnimation(
                m_stateMachine,
                m_currentComboStep,
                OnAttackCompleted);

            Debug.Log($"Attack: {attack.attackName} Damage: {attack.damage}");
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.25f), ignoreTimeScale: false);
    }

    private void OnAttackCompleted()
    {
        m_isAttacking = false;
    }

    private void ResetCombo()
    {
        m_stateMachine.SwitchToState<PlayerMoveState>();
    }
}