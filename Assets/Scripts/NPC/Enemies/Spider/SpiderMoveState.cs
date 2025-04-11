using UnityEngine;
using UnityEngine.AI;

public class SpiderMoveState : SpiderBaseState
{
    // Spider settings
    private float m_idleDuration = 0f;
    private float m_minIdleDuration = 1f;
    private float m_maxIdleDuration = 6f;

    private float m_walkSpeed = 3.5f;
    private float m_stoppingDistance = 1f;
    private float m_searchRadius = 10f;
    private float m_detectionRange = 10f;

    // State variables
    private float m_timer;
    private Vector3 m_destination;
    private bool m_isMoving;

    public override void Enter()
    {
        m_timer = 0f;
        m_idleDuration = Random.Range(m_minIdleDuration, m_maxIdleDuration);

        m_isMoving = false;
        m_stateMachine.NavAgent.isStopped = true;
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(0f);
    }

    public override void Tick()
    {
        m_timer += Time.deltaTime;

        if (m_stateMachine.IsPlayerInDetectionRange(m_detectionRange))
        {
            m_stateMachine.SwitchToState<SpiderChaseState>();
            return;
        }

        // Idle/Walk logic
        if (!m_isMoving)
        {
            // IdleState
            if (m_timer >= m_idleDuration)
            {
                StartMovement();
            }
        }
        else
        {
            // Walk State
            UpdateMovementAnimation();

            if (HasReachedDestination())
            {
                StopMovement();
            }
        }
    }

    public override void Exit()
    {
        m_stateMachine.NavAgent.isStopped = true;
    }

    private void StartMovement()
    {
        m_isMoving = true;
        m_timer = 0f;
        m_stateMachine.NavAgent.isStopped = false;
        m_stateMachine.NavAgent.speed = m_walkSpeed;
        m_stateMachine.NavAgent.stoppingDistance = m_stoppingDistance;
        FindNewDestination();
    }

    private void StopMovement()
    {
        m_isMoving = false;
        m_timer = 0f;
        m_stateMachine.NavAgent.isStopped = true;
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(0f);
    }

    private void UpdateMovementAnimation()
    {
        float normalizedSpeed = m_stateMachine.NavAgent.velocity.magnitude / m_walkSpeed;
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(normalizedSpeed);
    }

    private void FindNewDestination()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 randomPoint = m_stateMachine.transform.position +
                                  Random.insideUnitSphere * m_searchRadius;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, m_searchRadius, NavMesh.AllAreas))
            {
                m_destination = hit.position;
                m_stateMachine.NavAgent.SetDestination(m_destination);
                return;
            }
        }
        m_destination = m_stateMachine.transform.position;
    }

    private bool HasReachedDestination()
    {
        return !m_stateMachine.NavAgent.pathPending &&
               m_stateMachine.NavAgent.remainingDistance <= m_stoppingDistance &&
               m_stateMachine.NavAgent.velocity.sqrMagnitude == 0f;
    }
}