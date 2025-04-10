using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpiderAnimator : MonoBehaviour
{
    [SerializeField] private float _transitionTime = 0.1f;

    private Animator _animator;
    private int _speedHash = Animator.StringToHash("Speed");
    private float _currentSpeed;
    private float _smoothVelocity;

    // Hashes para os estados de animação
    private readonly int _idleStateHash = Animator.StringToHash("Spider_Idle");
    private readonly int _walkStateHash = Animator.StringToHash("Spider_Walk");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateMovementAnimation(float speed)
    {
        // Suaviza a transição de velocidade
        _currentSpeed = Mathf.SmoothDamp(
            _currentSpeed,
            speed,
            ref _smoothVelocity,
            _transitionTime
        );

        _animator.SetFloat(_speedHash, _currentSpeed);
    }

    public void PlayIdleAnimation()
    {
        _animator.CrossFade(_idleStateHash, _transitionTime);
    }

    public void PlayWalkAnimation()
    {
        _animator.CrossFade(_walkStateHash, _transitionTime);
    }

    public void SetMovementSpeed(float normalizedSpeed)
    {
        // Para blend tree 1D simples (0 = idle, 1 = walk)
        _animator.SetFloat(_speedHash, Mathf.Clamp01(normalizedSpeed));
    }
}