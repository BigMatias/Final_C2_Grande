using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerSettingsSO _playerSettingsSo;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _animator;

    [Header("Cameras")] 
    [SerializeField] private GameObject _aimPivot;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float minPitch = -70f;
    [SerializeField] private float maxPitch = 70f;
    
    public PlayerSettingsSO PlayerSettings => _playerSettingsSo;
    public LayerMask GroundLayer => _groundLayer;
    public Animator Animator => _animator;
    public Camera MainCamera => _mainCamera;
    
    public Rigidbody Rb { get; private set; }
    public CapsuleCollider Col { get; private set; }
    public Vector3 StandCameraPos { get; private set; }
    public Vector3 CrouchCameraPos { get; private set; }
    
    private float _pitch;
    private PlayerShoot _playerShoot;
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
        _playerShoot = GetComponent<PlayerShoot>();
        
        _healthSystem.onDie += () => onDie?.Invoke();
        _healthSystem.onDamage += (c, t) => onDamage?.Invoke(c, t);

        LocomotionState = new PlayerLocomotionState(this);
        CrouchState = new PlayerCrouchState(this);
        AirborneState = new PlayerAirborneState(this);
    }

    private void Start()
    {
        StandCameraPos = _mainCamera.transform.localPosition; 
        CrouchCameraPos = new Vector3(StandCameraPos.x, StandCameraPos.y - 0.5f, StandCameraPos.z); 

        SwitchState(LocomotionState);
    }

    private void Update()
    {
        _currentState.UpdateState(); 
        CameraRotate(); 
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

    private void CameraRotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * _playerSettingsSo.mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _playerSettingsSo.mouseSens * Time.deltaTime;

        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        _mainCamera.transform.localRotation = Quaternion.Euler(_pitch, 0, 0);
        _aimPivot.transform.localRotation = Quaternion.Euler(_pitch, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
        
        _playerShoot.GetGrips();
    }

    public void TakeDamage(float amount) => _healthSystem.TakeDamage(amount);
}