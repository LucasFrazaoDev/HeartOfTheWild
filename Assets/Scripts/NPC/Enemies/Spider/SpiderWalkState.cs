using UnityEngine.AI;
using UnityEngine;

public class SpiderWalkState : SpiderBaseState
{
    private Vector3 _targetPosition;

    public override void Enter()
    {
        _targetPosition = GetRandomNavMeshPosition(5f);
        m_stateMachine.NavAgent.SetDestination(_targetPosition);
        m_stateMachine.NavAgent.speed = m_stateMachine.WalkSpeed;
    }

    public override void Tick()
    {
        if (HasReachedDestination())
            m_stateMachine.SwitchToState<SpiderIdleState>();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    private Vector3 GetRandomNavMeshPosition(float radius)
    {
        Vector3 randomPoint = Random.insideUnitSphere * radius + m_stateMachine.transform.position;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, NavMesh.AllAreas);
        return hit.position;
    }

    private bool HasReachedDestination()
        => !m_stateMachine.NavAgent.pathPending &&
           m_stateMachine.NavAgent.remainingDistance <= m_stateMachine.NavAgent.stoppingDistance;
}