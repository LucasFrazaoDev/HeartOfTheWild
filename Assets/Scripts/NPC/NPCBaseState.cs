using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCBaseState : State
{
    private const float k_heightOffset = 0.7f;
    protected readonly NPCStateMachine m_stateMachine;

    protected NPCBaseState(NPCStateMachine stateMachine)
    {
        m_stateMachine = stateMachine;
    }

    protected void MoveTo(Vector3 destination)
    {
        if (m_stateMachine.NavMeshAgent.enabled)
        {
            m_stateMachine.NavMeshAgent.SetDestination(destination);
        }
    }

    protected void StopMovement()
    {
        if (m_stateMachine.NavMeshAgent.enabled)
        {
            m_stateMachine.NavMeshAgent.ResetPath();
        }
    }

    protected bool IsPlayerInRange(float range)
    {
        if (m_stateMachine.Target == null) return false;

        float distance = Vector3.Distance(m_stateMachine.HeadTransform.position, m_stateMachine.Target.position);
        return distance <= range;
    }

    public bool HasLineOfSight()
    {
        if (m_stateMachine.Target == null || m_stateMachine.HeadTransform == null) return false;

        Vector3 eyePosition = m_stateMachine.HeadTransform.position;
        Vector3 playerHeadPosition = m_stateMachine.Target.position + Vector3.up * k_heightOffset;
        Vector3 directionToPlayer = (playerHeadPosition - eyePosition).normalized;

        Debug.DrawRay(eyePosition, directionToPlayer * m_stateMachine.ViewRange, Color.yellow, 0.1f);

        // Check line of sight and distance
        Ray ray = new Ray(eyePosition, directionToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hit, m_stateMachine.ViewRange))
            return hit.transform == m_stateMachine.Target;

        return false;
    }

    public bool IsPlayerInFieldOfView()
    {
        if (m_stateMachine.Target == null || m_stateMachine.HeadTransform == null) return false;

        Vector3 directionToPlayer = (m_stateMachine.Target.position - m_stateMachine.HeadTransform.position).normalized;

        float angleToPlayer = Vector3.Angle(m_stateMachine.HeadTransform.forward, directionToPlayer);

        return angleToPlayer <= m_stateMachine.ViewAngle / 2f;
    }

    public bool CanSeePlayer()
    {
        return IsPlayerInRange(m_stateMachine.DetectionRange) &&
               IsPlayerInFieldOfView() &&
               HasLineOfSight();
    }
}
