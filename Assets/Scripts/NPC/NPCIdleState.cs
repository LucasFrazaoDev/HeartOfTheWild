using UnityEngine;

public class NPCIdleState : NPCBaseState
{
    private float m_idleTime;
    private readonly float m_maxIdleTime = 2.0f;

    public NPCIdleState(NPCStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        StopMovement();
        m_idleTime = 0f;
        m_stateMachine.NpcAnimator.UpdateMoveAnimation(0f);
    }

    public override void Tick()
    {
        m_idleTime += Time.deltaTime;

        if (IsPlayerInRange(m_stateMachine.DetectionRange))
        {
            m_stateMachine.SwitchState(new NPCChaseState(m_stateMachine));
            return;
        }

        if (m_idleTime >= m_maxIdleTime)
        {
            m_stateMachine.SwitchState(new NPCPatrolState(m_stateMachine));
        }
    }

    public override void Exit()
    {
        // Opcional: Lógica ao sair do estado idle.
    }
}
