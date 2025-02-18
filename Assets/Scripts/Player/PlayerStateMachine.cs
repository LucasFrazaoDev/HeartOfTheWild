using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : StateMachine
{
    private CharacterController m_charController;
    private Transform m_mainCamera;
    [HideInInspector] public Vector3 Velocity;

    [Header("Player status")]
    [SerializeField] private float m_movementSpeed = 3f;
    [SerializeField] private float m_jumpForce = 5f;
    [SerializeField] private float m_lookRotationDampFactor = 2f;
    [SerializeField] private float m_acceleration = 5.0f;
    [SerializeField] private float m_gravityMultiplier = 2f;
    [SerializeField] private float m_maxFallSpeed = -20f;

    [Header("Dependecies")]
    [SerializeField] private PlayerAnimator m_playerAnimator;
    [SerializeField] private InputReaderSO m_inputReader;
    [SerializeField] private AttackDataSO[] m_attackCombo;

    public float MovementSpeed { get => m_movementSpeed; set => m_movementSpeed = value; }
    public float JumpForce { get => m_jumpForce; set => m_jumpForce = value; }
    public float LookRotationDampFactor { get => m_lookRotationDampFactor; set => m_lookRotationDampFactor = value; }
    public Transform MainCamera { get => m_mainCamera; set => m_mainCamera = value; }

    public InputReaderSO GameInput { get => m_inputReader; set => m_inputReader = value; }
    public CharacterController CharController { get => m_charController; set => m_charController = value; }
    public PlayerAnimator PlayerAnimator { get => m_playerAnimator; set => m_playerAnimator = value; }
    public float Acceleration { get => m_acceleration; set => m_acceleration = value; }
    public AttackDataSO[] AttackCombo { get => m_attackCombo; set => m_attackCombo = value; }
    public float GravityMultiplier { get => m_gravityMultiplier; set => m_gravityMultiplier = value; }
    public float MaxFallSpeed { get => m_maxFallSpeed; set => m_maxFallSpeed = value; }

    private void Awake()
    {
        CharController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        MainCamera = Camera.main.transform;

        SwitchState(new PlayerMoveState(this));
    }
}