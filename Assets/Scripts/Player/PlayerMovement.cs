using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerSettingsSO _playerSettingsSo;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _animator;

    public PlayerSettingsSO PlayerSettings => _playerSettingsSo;
    public LayerMask GroundLayer => _groundLayer;
    public Animator Animator => _animator;

    public Rigidbody Rb { get; private set; }
    public CapsuleCollider Col { get; private set; }
    public PlayerLook Look { get; private set; }

    private HealthSystem _healthSystem;
    private PlayerBaseState _currentState;

    public PlayerLocomotionState LocomotionState { get; private set; }
    public PlayerCrouchState CrouchState { get; private set; }
    public PlayerAirborneState AirborneState { get; private set; }

    public event Action onDie;
    public event Action<float, float> onDamage;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        Rb = GetComponent<Rigidbody>();
        Col = GetComponent<CapsuleCollider>();
        Look = GetComponent<PlayerLook>();

        _healthSystem.onDie += () => onDie?.Invoke();
        _healthSystem.onDamage += (c, t) => onDamage?.Invoke(c, t);

        LocomotionState = new PlayerLocomotionState(this);
        CrouchState = new PlayerCrouchState(this);
        AirborneState = new PlayerAirborneState(this);
    }

    private void Start()
    {
        SwitchState(LocomotionState);
    }

    private void Update()
    {
        _currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        _currentState.FixedUpdateState();
    }

    public void SwitchState(PlayerBaseState newState)
    {
        _currentState?.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }

    public void TakeDamage(float amount) => _healthSystem.TakeDamage(amount);
}