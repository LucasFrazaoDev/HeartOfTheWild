using UnityEngine;
using UnityEngine.AI;

public class SpiderIdleState : SpiderBaseState
{
    private float _idleTime = 3f;
    private float _timer;
    private const float _detectionRange = 10f;

    public override void Enter()
    {
        m_stateMachine.SpiderAnimator.UpdateMovementSpeed(0f);
        m_stateMachine.NavAgent.isStopped = true;
        _timer = 0f;
    }

    public override void Tick()
    {
        _timer += Time.deltaTime;

        if (m_stateMachine.IsPlayerInDetectionRange(_detectionRange))
        {
            m_stateMachine.SwitchToState<SpiderChaseState>();
            return;
        }

        if (_timer >= _idleTime)
        {
            m_stateMachine.SwitchToState<SpiderWalkState>();
        }
    }

    public override void Exit()
    {
        m_stateMachine.NavAgent.isStopped = false;
    }
}