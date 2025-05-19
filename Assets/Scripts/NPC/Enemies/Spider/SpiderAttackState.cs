using UnityEngine;

public class SpiderAttackState : SpiderBaseState
{
    [Header("Attack Settings")]
    private float m_attackCooldown = 1f;
    private float m_attackRange = 2f;
    private float m_attackDamage = 10f;
    private float m_attackAnimationDuration = 1.2f;

    private float m_attackTimer;
    private bool m_isAttacking;
    private float m_originalStoppingDistance;

    public override void Enter()
    {
        // Configuração inicial
        m_attackTimer = 0f;
        m_isAttacking = false;
        m_originalStoppingDistance = m_stateMachine.NavAgent.stoppingDistance;

        // Prepara para o ataque
        m_stateMachine.NavAgent.isStopped = true;
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(0f);
    }

    public override void Tick()
    {
        m_attackTimer += Time.deltaTime;

        // Verifica se o jogador ainda está no alcance
        if (m_stateMachine.Player == null ||
            !m_stateMachine.IsPlayerInDetectionRange(m_attackRange * 1.2f))
        {
            m_stateMachine.SwitchToState<SpiderChaseState>();
            return;
        }

        FaceTarget(m_stateMachine.Player.position);

        if (!m_isAttacking && m_attackTimer >= m_attackCooldown)
        {
            StartAttack();
        }
        else if (m_isAttacking && m_attackTimer >= m_attackAnimationDuration)
        {
            CompleteAttack();
        }
    }

    public override void Exit()
    {
        m_stateMachine.NavAgent.stoppingDistance = m_originalStoppingDistance;
        m_stateMachine.NavAgent.isStopped = false;
    }

    private void StartAttack()
    {
        m_isAttacking = true;
        m_stateMachine.NavAgent.speed = 0f;
        m_attackTimer = 0f;

        m_stateMachine.SpiderAnimator.PlayAttackAnimation();

        //ApplyDamage();
    }

    private void CompleteAttack()
    {
        m_isAttacking = false;
        m_stateMachine.NavAgent.speed = m_stateMachine.WalkSpeed;
        m_attackTimer = 0f;
    }

    private void ApplyDamage()
    {
        if (Vector3.Distance(m_stateMachine.transform.position,
                           m_stateMachine.Player.position) <= m_attackRange)
        {
            // Aqui você implementaria a lógica de causar dano ao jogador
            // Exemplo: m_stateMachine.Player.GetComponent<PlayerHealth>().TakeDamage(m_attackDamage);
            Debug.Log("Ataque causou " + m_attackDamage + " de dano ao jogador");
        }
    }
}