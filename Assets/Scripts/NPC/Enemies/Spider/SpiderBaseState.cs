using UnityEngine;

public abstract class SpiderBaseState : State
{
    protected SpiderStateMachine m_stateMachine;

    public void Initialize(SpiderStateMachine machine)
    {
        m_stateMachine = machine;
    }
    protected void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - m_stateMachine.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            m_stateMachine.transform.rotation = Quaternion.Slerp(
                m_stateMachine.transform.rotation,
                targetRotation,
                m_stateMachine.RotationSpeed * Time.deltaTime
            );
        }
    }
}