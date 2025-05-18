using UnityEngine;

public class SpiderChaseState : SpiderBaseState
{
    private float m_chaseSpeedMultiplier = 1.5f;
    private float m_chaseRange = 15f;
    private float m_attackRange = 2f;
    private float m_maxChaseDuration = 10f;

    private float m_chaseTimer;
    private float m_originalStoppingDistance;

    public override void Enter()
    {
        // Initial setup
        m_chaseTimer = 0f;
        m_originalStoppingDistance = m_stateMachine.NavAgent.stoppingDistance;
        m_stateMachine.NavAgent.stoppingDistance = m_attackRange * 0.8f;

        // Adjust speed and state
        m_stateMachine.NavAgent.speed = m_stateMachine.WalkSpeed * m_chaseSpeedMultiplier;
        m_stateMachine.NavAgent.isStopped = false;
    }

    public override void Tick()
    {
        m_chaseTimer += Time.deltaTime;

        if (m_stateMachine.Player == null || m_chaseTimer >= m_maxChaseDuration)
        {
            m_stateMachine.SwitchToState<SpiderMoveState>();
            return;
        }

        UpdateAnimation();

        ChasePlayer();
    }

    public override void Exit()
    {
        m_stateMachine.NavAgent.stoppingDistance = m_originalStoppingDistance;
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(0f);
    }

    private void UpdateAnimation()
    {
        float currentSpeed = m_stateMachine.NavAgent.velocity.magnitude;
        float normalizedSpeed = currentSpeed / (m_stateMachine.WalkSpeed * m_chaseSpeedMultiplier);
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(normalizedSpeed);
    }

    private void ChasePlayer()
    {
        Vector3 playerPosition = m_stateMachine.Player.position;
        m_stateMachine.NavAgent.SetDestination(playerPosition);
        FaceTarget(playerPosition);

        float distanceToPlayer = Vector3.Distance(
            m_stateMachine.transform.position,
            playerPosition
        );

        if (distanceToPlayer > m_chaseRange)
        {
            m_stateMachine.SwitchToState<SpiderMoveState>();
        }
        else if (distanceToPlayer <= m_attackRange)
        {
            m_stateMachine.SwitchToState<SpiderAttackState>();
        }
    }
}