using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField] private int m_priority = 1; // Prioridade do objeto
    [SerializeField] Transform m_targetPoint;

    private bool m_isTarget = true;

    public float Priority => m_priority;
    public Transform TargetPoint => m_targetPoint;

    public bool IsTarget { get => m_isTarget; set => m_isTarget = value; }
}