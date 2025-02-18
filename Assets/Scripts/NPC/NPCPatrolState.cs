using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class NPCPatrolState : NPCBaseState
{
    private const float k_timeToCheckForPlayer = 0.2f;
    private int m_currentWaypointIndex = 0;
    private CancellationTokenSource _cancellationTokenSource;

    public NPCPatrolState(NPCStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        SetInitialState();

        StartDetectionCheckAsync().Forget();
    }

    public override void Tick()
    {
        // Check if NPC can reach next patrol point
        if (m_stateMachine.Waypoints == null || m_stateMachine.Waypoints.Length == 0)
            return;

        if (!m_stateMachine.NavMeshAgent.pathPending &&
            m_stateMachine.NavMeshAgent.remainingDistance <= m_stateMachine.NavMeshAgent.stoppingDistance)
        {
            MoveToNextWaypoint();
        }
    }

    public override void Exit()
    {
        StopMovement();
        _cancellationTokenSource?.Cancel();
    }

    private void SetInitialState()
    {
        m_stateMachine.NavMeshAgent.speed = 2.0f;
        m_stateMachine.NpcAnimator.UpdateMoveAnimation(0.125f);

        if (m_stateMachine.Waypoints != null && m_stateMachine.Waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
        else
        {
            Debug.LogWarning("No waypoints found for patrol state.");
        }
    }

    private void MoveToNextWaypoint()
    {
        if (m_stateMachine.Waypoints == null || m_stateMachine.Waypoints.Length == 0)
            return;

        m_currentWaypointIndex = (m_currentWaypointIndex + 1) % m_stateMachine.Waypoints.Length;
        MoveTo(m_stateMachine.Waypoints[m_currentWaypointIndex].position);
    }

    private async UniTaskVoid StartDetectionCheckAsync()
    {
        // Cancellation Token creation
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (CanSeePlayer())
                {
                    m_stateMachine.SwitchState(new NPCChaseState(m_stateMachine));
                    return;
                }

                // Delay for next verification
                await UniTask.Delay(TimeSpan.FromSeconds(k_timeToCheckForPlayer), cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }
}
