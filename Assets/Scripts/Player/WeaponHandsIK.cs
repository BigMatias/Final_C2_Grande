using UnityEngine;

public class WeaponHandsIK : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private Transform _weaponHolder;

    [Header("IK Targets")]
    [SerializeField] private Transform _rightHandIKTarget;
    [SerializeField] private Transform _leftHandIKTarget;

    [Header("Right Arm Bones")]
    [SerializeField] private Transform _rightUpperArm;
    [SerializeField] private Transform _rightForeArm;
    [SerializeField] private Transform _rightHand;

    [Header("Left Arm Bones")]
    [SerializeField] private Transform _leftUpperArm;
    [SerializeField] private Transform _leftForeArm;
    [SerializeField] private Transform _leftHand;

    [Header("Reach")]
    [SerializeField, Range(0.8f, 1f)] private float _reachFactor = 0.95f;

    [Header("View Follow")]
    [SerializeField] private Transform _viewCamera;
    [SerializeField, Range(0f, 1f)] private float _followCameraWeight = 0.8f;

    [Header("Smoothing")]
    [SerializeField] private float _positionSmoothing = 18f;

    [Header("Aim")]
    [SerializeField] private Transform _aimCamera;
    [SerializeField] private float _aimDistance = 0.2f;
    [SerializeField] private float _baseAimTime = 0.1f;
    [SerializeField] private float _aimTimePerWeight = 0.12f;

    public Transform WeaponHolder => _weaponHolder;
    public float AimBlend => _aimBlendEased;

    private WeaponBase _currentWeapon;
    private Vector3 _holderBaseLocalPosition;
    private Quaternion _holderBaseLocalRotation;
    private Vector3 _smoothedLocalPosition;
    private Vector3 _cameraRestLocalPosition;
    private float _rightArmLength;
    private float _leftArmLength;
    private bool _isAiming;
    private float _aimBlend;
    private float _aimBlendEased;
    private float _kickBack;
    private float _kickPitch;
    private float _kickReturnSpeed = 10f;

    private void Awake()
    {
        _holderBaseLocalPosition = _weaponHolder.localPosition;
        _holderBaseLocalRotation = _weaponHolder.localRotation;
        _smoothedLocalPosition = _holderBaseLocalPosition;
        _rightArmLength = GetArmLength(_rightUpperArm, _rightForeArm, _rightHand);
        _leftArmLength = GetArmLength(_leftUpperArm, _leftForeArm, _leftHand);
        _cameraRestLocalPosition = transform.InverseTransformPoint(_viewCamera.position);
    }

    public void AttachToWeapon(WeaponBase weapon)
    {
        _currentWeapon = weapon;

        if (_currentWeapon == null)
            return;

        _weaponHolder.localPosition = _holderBaseLocalPosition;
        _weaponHolder.localRotation = _holderBaseLocalRotation;
        _smoothedLocalPosition = _holderBaseLocalPosition;
        _aimBlend = 0f;
        _aimBlendEased = 0f;
        _kickBack = 0f;
        _kickPitch = 0f;

        PlaceTarget(_rightHandIKTarget, _currentWeapon.RightHandGrip);
        PlaceTarget(_leftHandIKTarget, _currentWeapon.LeftHandGrip);
    }

    public void SetAiming(bool aiming)
    {
        _isAiming = aiming;
    }

    public void AddKick(float back, float pitch, float returnSpeed)
    {
        _kickBack += back;
        _kickPitch += pitch;
        _kickReturnSpeed = returnSpeed;
    }

    public void UpdateReach()
    {
        if (_currentWeapon == null)
            return;

        UpdateAimBlend();

        _weaponHolder.localRotation = _holderBaseLocalRotation;
        _weaponHolder.localPosition = _holderBaseLocalPosition;

        ApplyCameraFollow();

        Vector3 hipLocalPosition = _weaponHolder.localPosition;
        Vector3 aimLocalPosition = GetAimLocalPosition(hipLocalPosition);
        _weaponHolder.localPosition = Vector3.Lerp(hipLocalPosition, aimLocalPosition, _aimBlendEased);

        KeepInReach(_rightHandIKTarget, _rightUpperArm, _rightArmLength);
        KeepInReach(_leftHandIKTarget, _leftUpperArm, _leftArmLength);

        Vector3 desiredLocalPosition = _weaponHolder.localPosition;
        float t = 1f - Mathf.Exp(-_positionSmoothing * Time.deltaTime);
        _smoothedLocalPosition = Vector3.Lerp(_smoothedLocalPosition, desiredLocalPosition, t);
        _weaponHolder.localPosition = _smoothedLocalPosition;

        ApplyKick();
    }

    private void UpdateAimBlend()
    {
        float aimTime = _baseAimTime + _currentWeapon.Data.weight * _aimTimePerWeight;
        float target = _isAiming ? 1f : 0f;

        _aimBlend = Mathf.MoveTowards(_aimBlend, target, Time.deltaTime / Mathf.Max(aimTime, 0.01f));
        _aimBlendEased = Mathf.SmoothStep(0f, 1f, _aimBlend);
    }

    private Vector3 GetAimLocalPosition(Vector3 fallback)
    {
        Transform sight = _currentWeapon.SightPoint;

        if (sight == null)
            return fallback;

        Transform pivot = _weaponHolder.parent;
        Vector3 sightTargetWorld = _aimCamera.position + _aimCamera.forward * _aimDistance;
        Vector3 sightTargetInPivot = pivot.InverseTransformPoint(sightTargetWorld);
        Vector3 sightInHolder = _weaponHolder.InverseTransformPoint(sight.position);

        return sightTargetInPivot - _holderBaseLocalRotation * sightInHolder;
    }

    private void ApplyCameraFollow()
    {
        Vector3 cameraLocal = transform.InverseTransformPoint(_viewCamera.position);
        Vector3 offset = cameraLocal - _cameraRestLocalPosition;
        _weaponHolder.position += transform.TransformVector(offset * _followCameraWeight);
    }

    private void ApplyKick()
    {
        float t = 1f - Mathf.Exp(-_kickReturnSpeed * Time.deltaTime);
        _kickBack = Mathf.Lerp(_kickBack, 0f, t);
        _kickPitch = Mathf.Lerp(_kickPitch, 0f, t);

        _weaponHolder.localPosition += Vector3.back * _kickBack;
        _weaponHolder.localRotation = Quaternion.Euler(-_kickPitch, 0f, 0f) * _holderBaseLocalRotation;
    }

    private void PlaceTarget(Transform target, Transform grip)
    {
        if (grip == null)
            return;

        target.SetPositionAndRotation(grip.position, grip.rotation);
    }

    private void KeepInReach(Transform target, Transform shoulder, float armLength)
    {
        Vector3 toTarget = target.position - shoulder.position;
        float maxDistance = armLength * _reachFactor;

        if (toTarget.magnitude <= maxDistance)
            return;

        _weaponHolder.position -= toTarget - toTarget.normalized * maxDistance;
    }

    private static float GetArmLength(Transform upperArm, Transform foreArm, Transform hand)
    {
        return Vector3.Distance(upperArm.position, foreArm.position) + Vector3.Distance(foreArm.position, hand.position);
    }
}