using UnityEngine;

public class SpiderChaseState : SpiderBaseState
{
    [Header("Chase Settings")]
    [SerializeField] private float _chaseSpeedMultiplier = 1.5f;
    [SerializeField] private float _chaseRange = 15f;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _maxChaseDuration = 10f;

    private float _chaseTimer;
    private float _originalStoppingDistance;

    public override void Enter()
    {
        // Configurações iniciais
        _chaseTimer = 0f;
        _originalStoppingDistance = m_stateMachine.NavAgent.stoppingDistance;
        m_stateMachine.NavAgent.stoppingDistance = _attackRange * 0.8f;

        // Ajusta velocidade
        m_stateMachine.NavAgent.speed = m_stateMachine.WalkSpeed * _chaseSpeedMultiplier;
        m_stateMachine.NavAgent.isStopped = false;
    }

    public override void Tick()
    {
        _chaseTimer += Time.deltaTime;

        if (m_stateMachine.Player == null || _chaseTimer >= _maxChaseDuration)
        {
            m_stateMachine.SwitchToState<SpiderIdleState>();
            return;
        }

        UpdateAnimation();

        ChasePlayer();
    }

    public override void Exit()
    {
        m_stateMachine.NavAgent.stoppingDistance = _originalStoppingDistance;
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(0f);
    }

    private void UpdateAnimation()
    {
        float currentSpeed = m_stateMachine.NavAgent.velocity.magnitude;
        float normalizedSpeed = currentSpeed / (m_stateMachine.WalkSpeed * _chaseSpeedMultiplier);
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

        if (distanceToPlayer > _chaseRange)
        {
            m_stateMachine.SwitchToState<SpiderIdleState>();
        }
        else if (distanceToPlayer <= _attackRange)
        {
            // m_stateMachine.SwitchToState<SpiderAttackState>();
        }
    }
}