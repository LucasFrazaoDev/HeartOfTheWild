using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachine : StateMachine
{
    [Header("NPC Properties")]
    [SerializeField] private Transform m_target;
    [SerializeField] private Transform m_npcEyes;
    [SerializeField] private float m_originalSpeed = 2.0f;
    
    [Header("Attack Settings")]
    [SerializeField] private float m_detectionRange = 10f;
    [SerializeField] private float m_viewAngle = 120f;
    [SerializeField] private float m_attackRange = 2.0f;
    [SerializeField] private float m_viewRange = 10f;
    [SerializeField] private float m_rotationSpeed = 5f;
    [SerializeField] private float m_alignmentThreshold = 10f;

    [Header("Patrol Waypoints")]
    [SerializeField] private Transform[] m_waypoints;

    [Header("Dependecies")]
    [SerializeField] private NPCAnimator m_npcAnimator;

    private NavMeshAgent m_navMeshAgent;


    public NavMeshAgent NavMeshAgent { get => m_navMeshAgent; set => m_navMeshAgent = value; }
    public NPCAnimator NpcAnimator { get => m_npcAnimator; set => m_npcAnimator = value; }
    public float DetectionRange { get => m_detectionRange; set => m_detectionRange = value; }
    public float AttackRange { get => m_attackRange; set => m_attackRange = value; }
    public Transform Target { get => m_target; set => m_target = value; }
    public float ViewAngle { get => m_viewAngle; set => m_viewAngle = value; }
    public float ViewRange { get => m_viewRange; set => m_viewRange = value; }
    public Transform[] Waypoints { get => m_waypoints; set => m_waypoints = value; }
    public float RotationSpeed { get => m_rotationSpeed; set => m_rotationSpeed = value; }
    public float AlignmentThreshold { get => m_alignmentThreshold; set => m_alignmentThreshold = value; }
    public float OriginalSpeed { get => m_originalSpeed; set => m_originalSpeed = value; }
    public Transform HeadTransform { get => m_npcEyes; set => m_npcEyes = value; }

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        CheckForPlayer();
        SwitchState(new NPCIdleState(this));
    }

    private void CheckForPlayer()
    {
        if (m_target != null)
            return;

        m_target = GameObject.FindWithTag("Player").transform;
    }

    private void OnDrawGizmosSelected()
    {
        if (HeadTransform == null)
        {
            Debug.LogWarning("HeadTransform não está atribuído ao NPCStateMachine!");
            return;
        }

        Vector3 arcOrigin = HeadTransform.position;
        Vector3 forward = HeadTransform.forward;

        // Calculate angle view
        Vector3 leftDirection = Quaternion.AngleAxis(-ViewAngle / 2, HeadTransform.up) * forward;
        Vector3 rightDirection = Quaternion.AngleAxis(ViewAngle / 2, HeadTransform.up) * forward;

        Gizmos.color = Color.blue;

        // Central line of FOV
        Gizmos.DrawLine(arcOrigin, arcOrigin + leftDirection * ViewRange);
        Gizmos.DrawLine(arcOrigin, arcOrigin + rightDirection * ViewRange);

        // Draw arc
        int segments = 20; // More segments, better arc
        float angleStep = ViewAngle / segments;
        Vector3 previousPoint = arcOrigin + leftDirection * ViewRange;

        for (int i = 1; i <= segments; i++)
        {
            Vector3 nextDirection = Quaternion.AngleAxis(-ViewAngle / 2 + i * angleStep, HeadTransform.up) * forward;
            Vector3 nextPoint = arcOrigin + nextDirection * ViewRange;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}
