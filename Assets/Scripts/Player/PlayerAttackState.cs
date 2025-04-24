using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private AttackDataSO m_currentWeaponData;
    private AttackCombo m_currentAttack;
    private bool m_comboInputReceived = false;
    private float m_stateEnterTime;

    public override void Enter()
    {
        m_stateEnterTime = Time.time;

        // Obter dados do ataque atual
        m_currentWeaponData = GetWeaponData(m_stateMachine.CurrentWeapon);
        m_currentAttack = m_currentWeaponData.GetAttackData(m_stateMachine.CurrentComboIndex);

        if (m_currentAttack == null)
        {
            m_stateMachine.SwitchToState<PlayerMoveState>();
            return;
        }

        // Configurar ataque
        //m_stateMachine.PlayerAnimator.PlayAttackAnimation(m_currentAttack.animationTrigger);
        m_comboInputReceived = false;

        // Registrar input para próximo ataque
        m_stateMachine.GameInput.OnAttackPerformed += RegisterComboInput;
    }

    public override void Tick()
    {
        // Verificar se o tempo do ataque acabou
        if (GetStateTime() >= m_currentAttack.attackDuration)
        {
            if (m_comboInputReceived && !m_currentAttack.isFinalAttack)
            {
                // Avançar para próximo ataque do combo
                m_stateMachine.SetCurrentComboIndex(m_stateMachine.CurrentComboIndex + 1);
                m_stateMachine.SwitchToState<PlayerAttackState>();
            }
            else
            {
                // Voltar para movimento
                ResetCombo();
                m_stateMachine.SwitchToState<PlayerMoveState>();
            }
        }
    }

    public override void Exit()
    {
        m_stateMachine.GameInput.OnAttackPerformed -= RegisterComboInput;
        m_stateMachine.UpdateLastAttackTime();
    }

    private void RegisterComboInput()
    {
        // Só registra input se estiver no tempo da janela de combo
        if (GetStateTime() >= m_currentAttack.attackDuration - m_currentAttack.comboWindow)
        {
            m_comboInputReceived = true;
        }
    }

    private AttackDataSO GetWeaponData(WeaponType weapon)
    {
        foreach (var data in m_stateMachine.AttackCombo)
        {
            if (data.weaponType == weapon)
                return data;
        }
        return null;
    }

    private void ResetCombo()
    {
        m_stateMachine.SetCurrentComboIndex(0);
    }

    private float GetStateTime()
    {
        return Time.time - m_stateEnterTime;
    }
}