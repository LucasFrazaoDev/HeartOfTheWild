using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpiderAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float m_speedSmoothTime = 0.1f;

    private Animator m_animator;
    private float m_currentSpeed;
    private float m_speedSmoothVelocity;

    private readonly int m_speedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void UpdateMovementSpeed(float rawSpeed)
    {
        // Convert raw speed to a value between 0 and 1
        float normalizedSpeed = Mathf.Clamp01(rawSpeed);

        // Smooth transition
        m_currentSpeed = Mathf.SmoothDamp(
            m_currentSpeed,
            normalizedSpeed,
            ref m_speedSmoothVelocity,
            m_speedSmoothTime
        );

        m_animator.SetFloat(m_speedHash, m_currentSpeed);
    }
}