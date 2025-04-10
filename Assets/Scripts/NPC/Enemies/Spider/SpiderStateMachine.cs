using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class SpiderStateMachine : StateMachine
{
    [SerializeField] private float m_rotationSpeed = 20.0f;
    [SerializeField] private float m_walkSpeed = 2.0f;

    [SerializeField] private Animator m_spiderAnimator;

    private NavMeshAgent m_navAgent;

    private StatePool _statePool;

    public float RotationSpeed { get => m_rotationSpeed; set => m_rotationSpeed = value; }
    public NavMeshAgent NavAgent { get => m_navAgent; set => m_navAgent = value; }
    public float WalkSpeed { get => m_walkSpeed; set => m_walkSpeed = value; }

    private void Awake()
    {
        _statePool = new StatePool(this);
        SwitchState(_statePool.GetState<SpiderIdleState>());
    }

    public void SwitchToState<T>() where T : SpiderBaseState, new()
    {
        SwitchState(_statePool.GetState<T>());
    }
}