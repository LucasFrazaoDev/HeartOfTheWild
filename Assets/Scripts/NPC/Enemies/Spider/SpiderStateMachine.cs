using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class SpiderStateMachine : StateMachine
{
    [SerializeField] private float m_rotationSpeed = 20.0f;
    [SerializeField] private float m_walkSpeed = 2.0f;
    [SerializeField] private Transform _player;

    [SerializeField] private SpiderAnimator m_spiderAnimator;

    private NavMeshAgent m_navAgent;

    private StatePool _statePool;

    public float RotationSpeed { get => m_rotationSpeed; set => m_rotationSpeed = value; }
    public NavMeshAgent NavAgent { get => m_navAgent; set => m_navAgent = value; }
    public float WalkSpeed { get => m_walkSpeed; set => m_walkSpeed = value; }
    public SpiderAnimator SpiderAnimator { get => m_spiderAnimator; set => m_spiderAnimator = value; }
    public Transform Player { get => _player; set => _player = value; }

    private void Awake()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
        _statePool = new StatePool(this);
    }

    private void Start()
    {
        SwitchState(_statePool.GetState<SpiderIdleState>());
    }

    public void SwitchToState<T>() where T : SpiderBaseState, new()
    {
        SwitchState(_statePool.GetState<T>());
    }

    // TODO: mudar método pra SpiderBaseState
    public bool IsPlayerInDetectionRange(float customRange = -1)
    {
        if (_player == null) return false;
        float range = customRange > 0 ? customRange : 10f;
        return Vector3.Distance(transform.position, _player.position) <= range;
    }
}