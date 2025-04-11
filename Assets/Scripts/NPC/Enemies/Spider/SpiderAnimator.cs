using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpiderAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _speedSmoothTime = 0.1f;

    private Animator _animator;
    private float _currentSpeed;
    private float _speedSmoothVelocity;

    private readonly int m_speedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateMovementSpeed(float rawSpeed)
    {
        // Converte a velocidade real para valor normalizado (0-1)
        float normalizedSpeed = Mathf.Clamp01(rawSpeed);

        // Suaviza a transição
        _currentSpeed = Mathf.SmoothDamp(
            _currentSpeed,
            normalizedSpeed,
            ref _speedSmoothVelocity,
            _speedSmoothTime
        );

        _animator.SetFloat(m_speedHash, normalizedSpeed);
    }
}