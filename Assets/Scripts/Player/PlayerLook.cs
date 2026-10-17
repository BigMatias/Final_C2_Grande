using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private PlayerSettingsSO _playerSettingsSo;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _aimPivot;
    [SerializeField] private float _minPitch = -70f;
    [SerializeField] private float _maxPitch = 70f;
    [SerializeField] private float _crouchCameraOffset = 0.5f;
    [SerializeField] private float _lookDownForwardOffset = 0.25f;

    [Header("Recoil")]
    [SerializeField] private float _recoilSnappiness = 18f;

    [Header("Aim")]
    [SerializeField] private float _fovSmoothing = 10f;

    public Camera MainCamera => _mainCamera;

    private WeaponHandsIK _handsIK;
    private float _pitch;
    private Vector3 _standCameraPos;
    private Vector3 _crouchCameraPos;
    private Vector3 _currentBaseCameraPos;
    private Vector2 _recoilTarget;
    private Vector2 _recoilCurrent;
    private float _recoilRecoverySpeed = 6f;
    private float _defaultFov;
    private float _targetFov;

    private void Awake()
    {
        _handsIK = GetComponent<WeaponHandsIK>();
        _defaultFov = _mainCamera.fieldOfView;
        _targetFov = _defaultFov;
    }

    private void Start()
    {
        _standCameraPos = _mainCamera.transform.localPosition;
        _crouchCameraPos = _standCameraPos + Vector3.down * _crouchCameraOffset;
        _currentBaseCameraPos = _standCameraPos;
    }

    private void Update()
    {
        Rotate();
        UpdateFov();
    }

    public void SetCrouched(bool crouched)
    {
        _currentBaseCameraPos = crouched ? _crouchCameraPos : _standCameraPos;
    }

    public void AddRecoil(float pitch, float yaw, float recoverySpeed)
    {
        _recoilTarget += new Vector2(pitch, yaw);
        _recoilRecoverySpeed = recoverySpeed;
    }

    public void SetAimFov(float aimBlend, float aimFov)
    {
        _targetFov = Mathf.Lerp(_defaultFov, aimFov, aimBlend);
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * _playerSettingsSo.mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _playerSettingsSo.mouseSens * Time.deltaTime;

        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        UpdateRecoil();

        float finalPitch = Mathf.Clamp(_pitch - _recoilCurrent.x, _minPitch, _maxPitch);
        Quaternion lookRotation = Quaternion.Euler(finalPitch, _recoilCurrent.y, 0f);

        _mainCamera.transform.localRotation = lookRotation;
        _aimPivot.localRotation = lookRotation;
        transform.Rotate(Vector3.up * mouseX);

        float lookDown = Mathf.Clamp01(finalPitch / _maxPitch);
        _mainCamera.transform.localPosition = _currentBaseCameraPos + Vector3.forward * (_lookDownForwardOffset * lookDown);

        if (_handsIK != null)
            _handsIK.UpdateReach();
    }

    private void UpdateRecoil()
    {
        _recoilTarget = Vector2.Lerp(_recoilTarget, Vector2.zero, 1f - Mathf.Exp(-_recoilRecoverySpeed * Time.deltaTime));
        _recoilCurrent = Vector2.Lerp(_recoilCurrent, _recoilTarget, 1f - Mathf.Exp(-_recoilSnappiness * Time.deltaTime));
    }

    private void UpdateFov()
    {
        float t = 1f - Mathf.Exp(-_fovSmoothing * Time.deltaTime);
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _targetFov, t);
    }
}