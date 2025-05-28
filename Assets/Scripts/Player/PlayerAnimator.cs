using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStateMachine m_stateMachine;
    [SerializeField] private InputReaderSO m_inputReader;

    [Header("Settings")]
    [SerializeField] private float m_moveAnimSmoothTime = 0.1f;
    [SerializeField] private float m_attackTransitionTime = 0.05f;

    // Referencias das animações
    private readonly int m_speedXHash = Animator.StringToHash("SpeedX");
    private readonly int m_speedYHash = Animator.StringToHash("SpeedY");
    private readonly int m_moveBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    private readonly int m_shieldDefenseHash = Animator.StringToHash("ShieldDefense");

    // Nome das animações
    private const string k_jumpAnim = "JumpLaunch";
    private const string k_fallAnim = "JumpMidAir";
    private const float k_airAnimTransition = 0.1f;

    private Animator m_animator;
    private float m_smoothVelocityX;
    private float m_smoothVelocityY;
    private Coroutine m_attackRoutine;

    private void Awake() => m_animator = GetComponent<Animator>();

    public void UpdateMoveAnimation(float speed)
    {
        Vector2 input = m_inputReader.GetMovementVectorNormalized();

        float currentSpeedX = Mathf.SmoothDamp(m_animator.GetFloat(m_speedXHash),
            speed * input.x, ref m_smoothVelocityX, m_moveAnimSmoothTime);

        float currentSpeedY = Mathf.SmoothDamp(m_animator.GetFloat(m_speedYHash),
            speed * input.y, ref m_smoothVelocityY, m_moveAnimSmoothTime);

        m_animator.SetFloat(m_speedXHash, currentSpeedX);
        m_animator.SetFloat(m_speedYHash, currentSpeedY);
    }

    // Inicia a animação de ataque com base no nome do trigger
    public void PlayAttackAnimation(string triggerName)
    {
        // Cancela qualquer ataque anterior
        if (m_attackRoutine != null)
        {
            StopCoroutine(m_attackRoutine);
            ResetAttackTriggers();
        }

        // Dispara o novo ataque
        m_animator.SetTrigger(triggerName);

        // Inicia o tracker da animação
        m_attackRoutine = StartCoroutine(TrackAttackAnimation(triggerName));
    }

    // Inicia a animação de ataque com base no índice do ataque
    private IEnumerator TrackAttackAnimation(string triggerName)
    {
        // Espera a animação começar
        yield return new WaitUntil(() =>
            m_animator.GetCurrentAnimatorStateInfo(0).IsName(triggerName));

        // Espera a animação terminar
        while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
        {
            yield return null;
        }

        // Volta para movimento
        CrossFadeMoveAnimation();
        m_attackRoutine = null;
    }

    // Reseta os triggers de ataque para evitar animações sobrepostas
    private void ResetAttackTriggers()
    {
        m_animator.ResetTrigger("Attack1");
        m_animator.ResetTrigger("Attack2");
        m_animator.ResetTrigger("Attack3");
    }

    // Inicia a animação de movimento com transição suave
    public void CrossFadeMoveAnimation() =>
        m_animator.CrossFade(m_moveBlendTreeHash, 0.1f);

    // Inicia a animação de pulo e queda
    public void PlayJumpAnimation() =>
        m_animator.CrossFade(k_jumpAnim, k_airAnimTransition);

    // Inicia a animação de queda
    public void PlayFallAnimation() =>
        m_animator.CrossFade(k_fallAnim, k_airAnimTransition);

    // Alterna a defesa com escudo, ativa ou desativa a animação
    public void ToggleShieldDefense(bool active) =>
        m_animator.SetBool(m_shieldDefenseHash, active);

    public void CancelAttackAnimation()
    {
        if (m_attackRoutine != null)
        {
            StopCoroutine(m_attackRoutine);
            ResetAttackTriggers();
            CrossFadeMoveAnimation();
            m_attackRoutine = null;
        }
    }
}