using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cysharp.Threading.Tasks;

public class LookAtManager : MonoBehaviour
{
    [Header("Look At Settings")]
    [SerializeField] private float m_lookAtRange = 5f;
    [SerializeField] private float m_viewAngle = 120f;
    [SerializeField] private float m_lookAtSmoothTime = 0.2f;
    [SerializeField] private float m_targetSwitchSmoothTime = 0.3f;

    [Header("Animation Rigging")]
    [SerializeField] private RigBuilder m_rigBuilder;
    [SerializeField] private Rig m_lookAtRig;
    [SerializeField] private MultiAimConstraint m_multiAimConstraint;

    private List<LookAtTarget> m_lookAtTargets = new List<LookAtTarget>();
    private Transform m_currentTarget;
    private Transform m_previousTarget;
    private Transform m_smoothedTarget;
    private float m_currentWeightVelocity;
    private Vector3 m_targetSwitchVelocity;

    private Collider[] m_hitColliders = new Collider[16];
    private const int k_millisecondsDelay = 300;

    private void Start()
    {
        m_lookAtRig.weight = 0f;
        CreateSmoothTransformObject();
        SetMultiAimConstraint();

        StartLookAtUpdateLoop().Forget();
    }

    private void Update()
    {
        SmoothRigWeight();
        SmoothTransformPosition();
    }

    private void SetMultiAimConstraint()
    {
        // Use a temp transform to set multiAim
        var sourceObjects = new WeightedTransformArray();
        WeightedTransform weightedTransform = new WeightedTransform { transform = m_smoothedTarget, weight = 1f };
        sourceObjects.Add(weightedTransform);
        m_multiAimConstraint.data.sourceObjects = sourceObjects;
        m_rigBuilder.Build();
    }

    private void CreateSmoothTransformObject()
    {
        m_smoothedTarget = new GameObject("SmoothedLookAtTarget").transform;
        m_smoothedTarget.SetParent(transform);
    }

    private async UniTaskVoid StartLookAtUpdateLoop()
    {
        while (true)
        {
            UpdateLookAtTargets();
            UpdateLookAtTarget();
            await UniTask.Delay(k_millisecondsDelay);
        }
    }

    private void UpdateLookAtTargets()
    {
        m_lookAtTargets.Clear();

        System.Array.Clear(m_hitColliders, 0, m_hitColliders.Length);

        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, m_lookAtRange, m_hitColliders);

        for (int i = 0; i < numColliders; i++)
        {
            Collider collider = m_hitColliders[i];
            if (collider.TryGetComponent<LookAtTarget>(out LookAtTarget lookAtTarget))
            {
                if (lookAtTarget.IsTarget && CanSeeObject(lookAtTarget.transform))
                {
                    m_lookAtTargets.Add(lookAtTarget);
                    Debug.Log(lookAtTarget.name);
                }
            }
        }
    }

    private void UpdateLookAtTarget()
    {
        Transform bestTarget = GetBestLookAtTarget();

        if (bestTarget != m_currentTarget)
        {
            m_previousTarget = m_currentTarget;
            m_currentTarget = bestTarget;
        }
    }

    private bool CanSeeObject(Transform target)
    {
        if (target == null)
            return false;

        // Check if target is in angle vision
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (angleToTarget > m_viewAngle / 2f)
            return false;

        // Check for obstacle between target and observer
        RaycastHit hit;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget))
        {
            if (hit.transform != target)
                return false;
        }

        return true;
    }

    private Transform GetBestLookAtTarget()
    {
        Transform bestTarget = null;
        float highestPriority = float.MinValue;
        float closestDistance = Mathf.Infinity;

        foreach (LookAtTarget target in m_lookAtTargets)
        {
            // Use reference point, if available
            Vector3 targetPosition = target.TargetPoint != null ? target.TargetPoint.position : target.transform.position;

            float distance = Vector3.Distance(transform.position, targetPosition);

            if (target.Priority > highestPriority ||
               (target.Priority == highestPriority && distance < closestDistance))
            {
                highestPriority = target.Priority;
                closestDistance = distance;
                bestTarget = target.transform;
            }
        }

        return bestTarget;
    }

    private void SmoothTransformPosition()
    {
        if (m_currentTarget != null)
        {
            // Use reference point, if available
            LookAtTarget lookAtTarget = m_currentTarget.GetComponent<LookAtTarget>();
            Vector3 targetPosition = lookAtTarget != null && lookAtTarget.TargetPoint != null
                ? lookAtTarget.TargetPoint.position : m_currentTarget.position;

            m_smoothedTarget.position = Vector3.SmoothDamp(
                m_smoothedTarget.position,
                targetPosition,
                ref m_targetSwitchVelocity,
                m_targetSwitchSmoothTime
            );
        }
        else if (m_previousTarget != null)
        {
            // If dont have a current target, but have a previous, keep smooth transition
            m_smoothedTarget.position = Vector3.SmoothDamp(
                                                            m_smoothedTarget.position,
                                                            m_previousTarget.position,
                                                            ref m_targetSwitchVelocity,
                                                            m_targetSwitchSmoothTime
            );
        }
    }

    private void SmoothRigWeight()
    {
        float targetWeight = m_currentTarget != null ? 1f : 0f;
        m_lookAtRig.weight = Mathf.SmoothDamp(
                                              m_lookAtRig.weight,
                                              targetWeight,
                                              ref m_currentWeightVelocity,
                                              m_lookAtSmoothTime);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 arcOrigin = transform.position;
        Vector3 forward = transform.forward;

        // angle vision
        Vector3 leftDirection = Quaternion.AngleAxis(-m_viewAngle / 2, transform.up) * forward;
        Vector3 rightDirection = Quaternion.AngleAxis(m_viewAngle / 2, transform.up) * forward;

        Gizmos.color = Color.black;

        // Main line FOV
        Gizmos.DrawLine(arcOrigin, arcOrigin + leftDirection * m_lookAtRange);
        Gizmos.DrawLine(arcOrigin, arcOrigin + rightDirection * m_lookAtRange);

        // Draw arc
        int segments = 20; // More segments, better arc
        float angleStep = m_viewAngle / segments;
        Vector3 previousPoint = arcOrigin + leftDirection * m_lookAtRange;

        for (int i = 1; i <= segments; i++)
        {
            Vector3 nextDirection = Quaternion.AngleAxis(-m_viewAngle / 2 + i * angleStep, transform.up) * forward;
            Vector3 nextPoint = arcOrigin + nextDirection * m_lookAtRange;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        // NPC line of sight
        if (m_currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, m_currentTarget.position);
        }
    }
}