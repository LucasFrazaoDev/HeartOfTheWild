using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int m_currentComboStep;
    private float m_comboTimer;
    private const float k_comboResetTime = 0.5f; // Tempo menor para sair do combo
    private bool m_isAttacking;
    private bool m_waitingForNextInput;

    public override void Enter()
    {
        ResetState();
        PerformAttack().Forget();
        m_stateMachine.GameInput.OnAttackPerformed += OnAttackPressed;
    }

    public override void Tick()
    {
        FaceMoveDirection();

        // Só conta o tempo se não estiver atacando e estiver esperando próxima entrada
        if (!m_isAttacking && m_waitingForNextInput)
        {
            m_comboTimer += Time.deltaTime;
            if (m_comboTimer > k_comboResetTime)
            {
                ResetCombo();
            }
        }
    }

    public override void Exit()
    {
        m_stateMachine.GameInput.OnAttackPerformed -= OnAttackPressed;
    }

    private void OnAttackPressed()
    {
        if (!m_isAttacking && m_waitingForNextInput)
        {
            if (m_currentComboStep < m_stateMachine.AttackCombo.Length - 1)
            {
                m_currentComboStep++;
                PerformAttack().Forget();
            }
        }
    }

    private async UniTaskVoid PerformAttack()
    {
        m_isAttacking = true;
        m_waitingForNextInput = false;
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

        // Pequena janela de bloqueio de input
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f), ignoreTimeScale: false);

        // Libera para receber próximo input ou sair do combo
        m_waitingForNextInput = true;
        m_isAttacking = false;
    }

    private void OnAttackCompleted()
    {
        // Não faz nada aqui, o controle é feito no Tick()
    }

    private void ResetCombo()
    {
        m_stateMachine.SwitchToState<PlayerMoveState>();
    }
}