using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int m_currentComboStep = 0;
    private float m_comboTimer = 0f;
    private const float k_comboResetTime = 1.0f;
    private bool m_isAttacking = false;

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        m_currentComboStep = 0;
        m_comboTimer = 0f;
        m_isAttacking = false;
        PerformAttack();
        EventBus.Instance.Subscribe<InputReaderEvents.AttackEvent>(OnAttackPressed);
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
        EventBus.Instance.Subscribe<InputReaderEvents.AttackEvent>(OnAttackPressed);
    }

    private void OnAttackPressed(InputReaderEvents.AttackEvent attackEvent)
    {
        if (!m_isAttacking)
        {
            var attackCombo = m_stateMachine.AttackCombo;

            // Avança para o próximo passo do combo, se houver
            if (m_currentComboStep < attackCombo.Length - 1)
            {
                m_currentComboStep++;
                PerformAttack().Forget(); // Inicia o próximo ataque
            }
        }
    }

    private async UniTaskVoid PerformAttack()
    {
        m_isAttacking = true;
        m_comboTimer = 0f;
        var attackCombo = m_stateMachine.AttackCombo;

        if (m_currentComboStep < attackCombo.Length)
        {
            AttackDataSO attack = attackCombo[m_currentComboStep];
            m_stateMachine.PlayerAnimator.PlayAttackAnimation(m_stateMachine, m_currentComboStep, OnAttackCompleted);
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
        m_currentComboStep = 0;
        m_comboTimer = 0f;
        m_stateMachine.SwitchState(new PlayerMoveState(m_stateMachine));
    }
}
