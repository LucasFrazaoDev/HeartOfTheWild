using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int m_currentComboStep;
    private float m_comboTimer;
    private const float k_comboResetTime = 0.3f;
    private bool m_isAttacking;
    private bool m_canQueueNextAttack;
    private bool m_animationCompleted;

    public override void Enter()
    {
        m_currentComboStep = 0;
        m_isAttacking = true;
        m_canQueueNextAttack = false;

        m_stateMachine.GameInput.OnAttackPerformed += OnAttackInput;
        PerformAttack().Forget();
    }

    public override void Tick()
    {
        FaceMoveDirection();
        CalculateMoveDirection();

        if (!m_isAttacking)
        {
            m_comboTimer += Time.deltaTime;

            if (m_comboTimer > k_comboResetTime)
            {
                m_stateMachine.SwitchToState<PlayerMoveState>();
            }
        }
    }

    private async UniTaskVoid PerformAttack()
    {
        m_animationCompleted = false;
        m_comboTimer = 0f;

        if (m_currentComboStep < m_stateMachine.AttackCombo.Length)
        {
            AttackDataSO attack = m_stateMachine.AttackCombo[m_currentComboStep];
            m_stateMachine.PlayerAnimator.PlayAttackAnimation(
                attack.attackAnimation, // Passa o AnimationClip diretamente
                () => m_animationCompleted = true);

            // Calcula o tempo baseado no clip de animação
            float inputDelay = Mathf.Min(attack.attackAnimation.length * 0.3f, 0.2f);
            await UniTask.Delay((int)(inputDelay * 1000), ignoreTimeScale: false);

            m_canQueueNextAttack = true;
            await UniTask.WaitUntil(() => m_animationCompleted);
        }

        m_isAttacking = false;
    }

    private void OnAttackInput()
    {
        if (!m_canQueueNextAttack || m_stateMachine.GameInput.IsInUIMode) return;

        if (m_currentComboStep < m_stateMachine.AttackCombo.Length - 1)
        {
            m_currentComboStep++;
            m_isAttacking = true;
            m_canQueueNextAttack = false;
            PerformAttack().Forget();
        }
    }

    public override void Exit()
    {
        m_stateMachine.GameInput.OnAttackPerformed -= OnAttackInput;
        m_stateMachine.PlayerAnimator.CancelAttackAnimation();
    }
}