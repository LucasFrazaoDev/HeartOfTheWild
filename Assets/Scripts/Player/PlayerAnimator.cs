using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerStateMachine m_stateMachine;
    [SerializeField] private InputReaderSO m_inputReader;
    [SerializeField] private float m_transitionTime = 0.1f;

    private float m_smoothSpeedX;
    private float m_smoothSpeedY;

    private Animator m_animator;
    private int m_speedHashX = Animator.StringToHash("SpeedX");
    private int m_speedHashY = Animator.StringToHash("SpeedY");

    private const string k_jumpAnimName = "JumpLaunch";
    private const string k_fallAnimName = "JumpMidAir";

    // Substitua as constantes de ataque existentes por:
    private const string k_bowAttackPrefix = "Bow_Attack";
    private const string k_swordAttackPrefix = "SwordAndShield_Attack";
    private const string k_spearAttackPrefix = "SpearAndShield_Attack";

    // Adicione estes hashes para os layers de armas
    private readonly int m_bowLayer = Animator.StringToHash("BowLayer");
    private readonly int m_swordLayer = Animator.StringToHash("SwordLayer");
    private readonly int m_spearLayer = Animator.StringToHash("SpearLayer");

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

    public void PlayAttackAnimation(string animationTrigger)
    {
        // Para todas as animações de ataque em andamento
        if (m_attackCoroutine != null)
        {
            StopCoroutine(m_attackCoroutine);
            m_attackCoroutine = null;
        }

        // Configura o layer de acordo com a arma atual
        SetWeaponLayerWeight(m_stateMachine.CurrentWeapon, 1f);

        // Executa a animação
        Animator.SetTrigger(animationTrigger);

        // Rastreia a animação se necessário
        m_attackCoroutine = StartCoroutine(TrackAttackAnimation());
    }

    private IEnumerator TrackAttackAnimation()
    {
        // Espera a animação terminar (implementação básica)
        yield return new WaitForSeconds(0.1f); // Pequeno delay para o trigger ser capturado

        while (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        // Volta ao peso normal dos layers
        ResetWeaponLayers();
        m_attackCoroutine = null;
    }

    private void SetWeaponLayerWeight(WeaponType weapon, float weight)
    {
        switch (weapon)
        {
            case WeaponType.Bow:
                Animator.SetLayerWeight(m_bowLayer, weight);
                break;
            case WeaponType.SwordAndShield:
                Animator.SetLayerWeight(m_swordLayer, weight);
                break;
            case WeaponType.SpearAndShield:
                Animator.SetLayerWeight(m_spearLayer, weight);
                break;
        }
    }

    private void ResetWeaponLayers()
    {
        Animator.SetLayerWeight(m_bowLayer, 0f);
        Animator.SetLayerWeight(m_swordLayer, 0f);
        Animator.SetLayerWeight(m_spearLayer, 0f);
    }

    public void CancelAttackAnimation()
    {
        if (m_attackCoroutine != null)
        {
            StopCoroutine(m_attackCoroutine);
            m_attackCoroutine = null;
        }

        ResetWeaponLayers();
        Animator.CrossFade(m_moveBlendTreeHash, 0.1f);
    }

    public void SwitchWeapon(WeaponType newWeapon)
    {
        // Reseta todas as layers primeiro
        ResetWeaponLayers();

        // Configura a layer da nova arma
        SetWeaponLayerWeight(newWeapon, 1f);

        // Atualiza a animação de movimento para refletir a mudança
        CrossFadeMoveAnimation();
    }
}