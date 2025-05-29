using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class SpiderStateMachine : StateMachine
{
    // Referências do SpiderStateMachine
    [SerializeField] private float m_rotationSpeed = 20.0f;
    [SerializeField] private float m_walkSpeed = 2.0f;
    [SerializeField] private Transform m_player;

    [SerializeField] private SpiderAnimator m_spiderAnimator;

    private NavMeshAgent m_navAgent;

    private StatePool m_statePool;

    public float RotationSpeed { get => m_rotationSpeed; set => m_rotationSpeed = value; }
    public NavMeshAgent NavAgent { get => m_navAgent; set => m_navAgent = value; }
    public float WalkSpeed { get => m_walkSpeed; set => m_walkSpeed = value; }
    public SpiderAnimator SpiderAnimator { get => m_spiderAnimator; set => m_spiderAnimator = value; }
    public Transform Player { get => m_player; set => m_player = value; }

    // Inicializa o NavMeshAgent e o StatePool
    private void Awake()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
        m_statePool = new StatePool(this);
    }

    // Inicia o estado inicial do SpiderStateMachine
    private void Start()
    {
        SwitchState(m_statePool.GetState<SpiderMoveState>());

        if(m_player == null)
            m_player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Alterna para um novo estado, retornando o estado atual ao pool
    public void SwitchToState<T>() where T : SpiderBaseState, new()
    {
        SwitchState(m_statePool.GetState<T>());
    }

    // TODO: mudar método pra SpiderBaseState
    // Método para verificar se o jogador está dentro do alcance de detecção
    public bool IsPlayerInDetectionRange(float customRange = -1)
    {
        if (m_player == null) return false;
        float range = customRange > 0 ? customRange : 10f;
        return Vector3.Distance(transform.position, m_player.position) <= range;
    }
}