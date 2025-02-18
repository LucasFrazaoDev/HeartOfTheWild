using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCChaseState : NPCBaseState
{
    public NPCChaseState(NPCStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        m_stateMachine.NavMeshAgent.speed = 4.0f;
        m_stateMachine.NpcAnimator.UpdateMoveAnimation(0.25f);

    }

    public override void Tick()
    {
        if (m_stateMachine.Target == null)
        {
            m_stateMachine.SwitchState(new NPCIdleState(m_stateMachine));
            return;
        }

        MoveTo(m_stateMachine.Target.position);

        if (IsPlayerInRange(m_stateMachine.AttackRange))
        {
            m_stateMachine.SwitchState(new NPCAttackState(m_stateMachine));
            return;
        }

        if (!IsPlayerInRange(m_stateMachine.DetectionRange))
        {
            m_stateMachine.SwitchState(new NPCIdleState(m_stateMachine));
        }
    }

    public override void Exit()
    {
        StopMovement();
    }
}
