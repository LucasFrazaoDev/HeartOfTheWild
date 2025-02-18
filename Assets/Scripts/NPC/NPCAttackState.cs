using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackState : NPCBaseState
{
    private float m_attackCooldown = 2.0f; // Tempo entre ataques
    private float m_lastAttackTime;

    public NPCAttackState(NPCStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        m_lastAttackTime = Time.time;
        m_stateMachine.NavMeshAgent.speed = 0f;
        PerformAttack();
    }

    public override void Tick()
    {
        // Verifica se o jogador saiu do alcance de ataque
        if (!IsPlayerInRange(m_stateMachine.AttackRange))
        {
            m_stateMachine.SwitchState(new NPCChaseState(m_stateMachine));
            return;
        }

        // Rotaciona para alinhar com o jogador
        AlignToPlayer();

        // Checa se est� no cooldown de ataque
        if (Time.time >= m_lastAttackTime + m_attackCooldown && IsAlignedWithPlayer())
        {
            PerformAttack();
            m_lastAttackTime = Time.time;
        }
    }

    public override void Exit()
    {
        m_stateMachine.NavMeshAgent.speed = m_stateMachine.OriginalSpeed; // Restaura a velocidade original
    }

    private void PerformAttack()
    {
        if (m_stateMachine.Target == null) return;

        m_stateMachine.NpcAnimator.PlayAttackAnimation();
        Debug.Log("NPC Attack Performed!");
    }

    private void AlignToPlayer()
    {
        if (m_stateMachine.Target == null) return;

        // Calcula a dire��o para o jogador
        Vector3 directionToPlayer = (m_stateMachine.Target.position - m_stateMachine.transform.position).normalized;
        directionToPlayer.y = 0f; // Ignora a altura

        // Rota��o desejada
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Rota��o suave em dire��o ao jogador
        m_stateMachine.transform.rotation = Quaternion.Slerp(
            m_stateMachine.transform.rotation,
            targetRotation,
            Time.deltaTime * m_stateMachine.RotationSpeed // Use uma velocidade configur�vel
        );
    }

    private bool IsAlignedWithPlayer()
    {
        if (m_stateMachine.Target == null) return false;

        // Dire��o para o jogador
        Vector3 directionToPlayer = (m_stateMachine.Target.position - m_stateMachine.transform.position).normalized;
        directionToPlayer.y = 0f;

        // �ngulo entre a dire��o atual e o jogador
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float angleDifference = Quaternion.Angle(m_stateMachine.transform.rotation, targetRotation);

        // Considera alinhado se o �ngulo for menor ou igual ao threshold
        return angleDifference <= m_stateMachine.AlignmentThreshold; // Configure o threshold no state machine
    }
}
