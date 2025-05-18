using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpiderAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float m_speedSmoothTime = 0.1f;

    private Animator m_animator;
    private float m_currentSpeed;
    private float m_speedSmoothVelocity;

    private const string k_attackParam = "SpiderAttack";

    public Animator Animator => m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void UpdateMovementSpeed(float speed)
    {
        // Blend tree movement speed
        float normalizedSpeed = Mathf.Clamp01(speed);
        m_currentSpeed = Mathf.SmoothDamp(
            m_currentSpeed,
            normalizedSpeed,
            ref m_speedSmoothVelocity,
            m_speedSmoothTime
        );
        m_animator.SetFloat("Speed", m_currentSpeed);
    }

    public void PlayAttackAnimation()
    {
        m_animator.SetTrigger(k_attackParam);
    }
}