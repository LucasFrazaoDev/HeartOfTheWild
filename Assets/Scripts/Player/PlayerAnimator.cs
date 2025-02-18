using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private InputReaderSO m_inputReader;
    [SerializeField] private float m_transitionTime = 0.07f;

    private float m_smoothSpeedX;
    private float m_smoothSpeedY;

    private Animator m_animator;
    private int m_speedHashX = Animator.StringToHash("SpeedX");
    private int m_speedHashY = Animator.StringToHash("SpeedY");

    private const string k_jumpAnimName = "JumpLaunch";
    private const string k_fallAnimName = "JumpMidAir";
    private const string k_attackAnimationPrefix = "SwordAndShield_Attack0";

    private const float k_crossFadeMoveDuration = 0.2f;
    private const float k_dampAirTime = 0.1f;

    private readonly int m_moveBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    private int m_shieldDefenseLayer = 0;

    private CancellationTokenSource m_shieldWeightCancellationTokenSource;

    public Animator Animator { get => m_animator; set => m_animator = value; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        m_shieldDefenseLayer = Animator.GetLayerIndex("ShieldDefense");
    }

    public void UpdateMoveAnimation(float speed)
    {
        Vector2 inputVector = m_inputReader.GetMovementVectorNormalized();
        RoundInputValues(ref inputVector);

        float targetSpeedX = speed * inputVector.x;
        float targetSpeedY = speed * inputVector.y;

        // Smooth transitions
        float currentSpeedX = Mathf.SmoothDamp(Animator.GetFloat(m_speedHashX), targetSpeedX, ref m_smoothSpeedX, m_transitionTime);
        float currentSpeedY = Mathf.SmoothDamp(Animator.GetFloat(m_speedHashY), targetSpeedY, ref m_smoothSpeedY, m_transitionTime);

        // Set animations
        Animator.SetFloat(m_speedHashX, currentSpeedX);
        Animator.SetFloat(m_speedHashY, currentSpeedY);
    }

    private void RoundInputValues(ref Vector2 inputVector)
    {
        inputVector.x = Mathf.Round(inputVector.x);
        inputVector.y = Mathf.Round(inputVector.y);
    }

    public void CrossFadeMoveAnimation()
    {
        Animator.CrossFadeInFixedTime(m_moveBlendTreeHash, k_crossFadeMoveDuration);
    }

    public void ToggleShieldDefense(float targetWeight, float transitionTime = 0.2f)
    {
        if (m_shieldDefenseLayer == -1) return;

        // Cancel the previous task, if in action
        m_shieldWeightCancellationTokenSource?.Cancel();
        m_shieldWeightCancellationTokenSource = new CancellationTokenSource();

        SetShieldWeightOverTimeAsync(targetWeight, transitionTime, m_shieldWeightCancellationTokenSource.Token).Forget();
    }

    private async UniTask SetShieldWeightOverTimeAsync(float targetWeight, float transitionTime, CancellationToken cancellationToken = default)
    {
        float startWeight = Animator.GetLayerWeight(m_shieldDefenseLayer);
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            // Cancel the task if token requested
            cancellationToken.ThrowIfCancellationRequested();

            // Smooth weight transition
            float newWeight = Mathf.Lerp(startWeight, targetWeight, elapsedTime / transitionTime);
            Animator.SetLayerWeight(m_shieldDefenseLayer, newWeight);

            elapsedTime += Time.deltaTime;
            await UniTask.Yield(); // Wait for next frame
        }

        Animator.SetLayerWeight(m_shieldDefenseLayer, targetWeight);
    }

    public void PlayJumpAnimation()
    {
        Animator.CrossFadeInFixedTime(k_jumpAnimName, k_dampAirTime);
    }

    public void PlayFallAnimation()
    {
        Animator.CrossFadeInFixedTime(k_fallAnimName, k_dampAirTime);
    }

    public void PlayAttackAnimation(PlayerStateMachine stateMachine, int comboStep, Action onComplete)
    {
        if (comboStep < 0 || comboStep >= stateMachine.AttackCombo.Length)
        {
            Debug.LogWarning("Combo step inválido. Ajuste para um valor dentro do intervalo correto.");
            return;
        }

        string attackAnimationName = $"{k_attackAnimationPrefix}{comboStep}";
        Animator.CrossFadeInFixedTime(attackAnimationName, 0.1f);

        WaitForAnimationCompletion(attackAnimationName, onComplete).Forget();
    }

    private async UniTask WaitForAnimationCompletion(string animationName, Action onComplete)
    {
        float animationLength = Animator.GetCurrentAnimatorStateInfo(0).length;
        await UniTask.Delay(TimeSpan.FromSeconds(animationLength));
        onComplete?.Invoke();
    }
}