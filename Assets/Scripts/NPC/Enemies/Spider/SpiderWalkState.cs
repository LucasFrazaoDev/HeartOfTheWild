using UnityEngine;
using UnityEngine.AI;

public class SpiderWalkState : SpiderBaseState
{
    private Vector3 _destination;
    private float _walkSpeed;
    private const float _stoppingDistance = 1f;
    private const float _searchRadius = 10f;

    public override void Enter()
    {
        _walkSpeed = m_stateMachine.WalkSpeed;
        m_stateMachine.NavAgent.isStopped = false;
        m_stateMachine.NavAgent.speed = _walkSpeed;
        m_stateMachine.NavAgent.stoppingDistance = _stoppingDistance;

        FindNewDestination();
    }

    public override void Tick()
    {
        float normalizedSpeed = m_stateMachine.NavAgent.velocity.magnitude / _walkSpeed;
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(normalizedSpeed);

        if (m_stateMachine.IsPlayerInDetectionRange())
        {
            m_stateMachine.SwitchToState<SpiderChaseState>();
            return;
        }

        if (HasReachedDestination())
        {
            m_stateMachine.SwitchToState<SpiderIdleState>();
        }
    }

    public override void Exit()
    {
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(0f);
    }

    private void FindNewDestination()
    {
        for (int i = 0; i < 3; i++) // Tenta 3 vezes encontrar um ponto válido
        {
            Vector3 randomPoint = m_stateMachine.transform.position +
                                Random.insideUnitSphere * _searchRadius;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _searchRadius, NavMesh.AllAreas))
            {
                _destination = hit.position;
                m_stateMachine.NavAgent.SetDestination(_destination);
                return;
            }
        }

        // Fallback se não encontrar ponto válido
        _destination = m_stateMachine.transform.position;
    }

    private bool HasReachedDestination()
    {
        return !m_stateMachine.NavAgent.pathPending &&
               m_stateMachine.NavAgent.remainingDistance <= _stoppingDistance &&
               m_stateMachine.NavAgent.velocity.sqrMagnitude == 0f;
    }
}