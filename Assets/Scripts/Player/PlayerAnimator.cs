using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private InputReaderSO m_inputReader;
    [SerializeField] private float m_transitionTime = 0.1f;

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
    private AnimationClip m_currentAttackClip;
    private Coroutine m_attackCoroutine;

    public Animator Animator { get => m_animator; set => m_animator = value; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        m_shieldDefenseLayer = Animator.GetLayerIndex("ShieldDefense");
    }

    public void UpdateMoveAnimation(float speed)
    {
        Vector2 inputVector = m_inputReader.GetMovementVectorNormalized();

        // Remove o RoundInputValues e mantém os valores originais do input
        float targetSpeedX = speed * inputVector.x;
        float targetSpeedY = speed * inputVector.y;

        // Aumenta o tempo de transição para suavização mais perceptível
        //float transitionTime = 0.15f; // Ajuste este valor conforme necessário

        float currentSpeedX = Mathf.SmoothDamp(Animator.GetFloat(m_speedHashX),
                                             targetSpeedX,
                                             ref m_smoothSpeedX,
                                             m_transitionTime);

        float currentSpeedY = Mathf.SmoothDamp(Animator.GetFloat(m_speedHashY),
                                             targetSpeedY,
                                             ref m_smoothSpeedY,
                                             m_transitionTime);

        Animator.SetFloat(m_speedHashX, currentSpeedX);
        Animator.SetFloat(m_speedHashY, currentSpeedY);
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

    public void PlayAttackAnimation(AnimationClip attackClip, Action onComplete)
    {
        if (m_attackCoroutine != null)
            StopCoroutine(m_attackCoroutine);

        m_currentAttackClip = attackClip;

        // Usa o nome do clip diretamente
        Animator.CrossFadeInFixedTime(attackClip.name, 0.05f);

        m_attackCoroutine = StartCoroutine(TrackAttackAnimation(onComplete));
    }

    private IEnumerator TrackAttackAnimation(Action onComplete)
    {
        // Espera animação começar
        yield return new WaitUntil(() =>
            Animator.GetCurrentAnimatorStateInfo(0).IsName(m_currentAttackClip.name));

        float timeout = m_currentAttackClip.length * 2f; // Segurança
        float elapsed = 0f;

        while (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f &&
               elapsed < timeout)
        {
            if (!Animator.GetCurrentAnimatorStateInfo(0).IsName(m_currentAttackClip.name))
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke();
    }

    public void CancelAttackAnimation()
    {
        if (m_attackCoroutine != null)
        {
            StopCoroutine(m_attackCoroutine);
            m_attackCoroutine = null;
        }
        Animator.CrossFade(m_moveBlendTreeHash, 0.1f);
    }
}