using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCAnimator : MonoBehaviour
{
    [SerializeField] private float m_transitionTime = 0.1f;

    private Animator m_animator;
    private int m_footmanMoveTreeHash = Animator.StringToHash("FootmanMoveTree");
    private int m_moveSpeed = Animator.StringToHash("MoveSpeed");

    private const float k_crossFadeDuration = 0.2f;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Update animation based on speed
    /// </summary>
    /// <param name="speed">Current NPC speed</param>
    public void UpdateMoveAnimation(float speed)
    {
        m_animator.CrossFadeInFixedTime(m_footmanMoveTreeHash, k_crossFadeDuration);
        m_animator.SetFloat(m_moveSpeed, speed);
    }

    public void PlayAttackAnimation()
    {
        m_animator.CrossFadeInFixedTime("Attack01", k_crossFadeDuration);
    }

}
