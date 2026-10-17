using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [FormerlySerializedAs("gun")]
    [SerializeField] private WeaponDataSO _startingWeapon;

    private WeaponHandsIK _handsIK;
    private PlayerLook _look;
    private ProjectilePool _projectilePool;
    private WeaponBase _currentWeapon;

    public WeaponBase CurrentWeapon => _currentWeapon;
    public bool IsAiming { get; private set; }

    public event Action OnPlayerShot;

    private void Awake()
    {
        _handsIK = GetComponent<WeaponHandsIK>();
        _look = GetComponent<PlayerLook>();
        _projectilePool = GetComponent<ProjectilePool>();
    }

    private void Start()
    {
        if (_startingWeapon != null)
            EquipWeapon(_startingWeapon);
    }

    private void Update()
    {
        if (_currentWeapon == null)
            return;

        IsAiming = Input.GetMouseButton(1);
        _handsIK.SetAiming(IsAiming);
        _look.SetAimFov(_handsIK.AimBlend, _currentWeapon.Data.aimFov);

        bool wantsToFire = _currentWeapon.Data.isAutomatic
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (wantsToFire)
            TryShoot();
    }

    public void EquipWeapon(WeaponDataSO weaponData)
    {
        if (_currentWeapon != null)
            Destroy(_currentWeapon.gameObject);

        GameObject go = Instantiate(weaponData.weaponPrefab, _handsIK.WeaponHolder);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        _currentWeapon = go.GetComponent<WeaponBase>();
        _currentWeapon.Initialize(weaponData, _projectilePool);

        _handsIK.AttachToWeapon(_currentWeapon);
    }

    private void TryShoot()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (!_currentWeapon.TryFire(ray))
            return;

        ApplyRecoil();
        OnPlayerShot?.Invoke();
    }

    private void ApplyRecoil()
    {
        WeaponDataSO data = _currentWeapon.Data;
        float multiplier = Mathf.Lerp(1f, data.aimRecoilMultiplier, _handsIK.AimBlend);

        float pitch = UnityEngine.Random.Range(data.recoilPitchRange.x, data.recoilPitchRange.y) * multiplier;
        float yaw = UnityEngine.Random.Range(data.recoilYawRange.x, data.recoilYawRange.y) * multiplier;

        _look.AddRecoil(pitch, yaw, data.recoilRecoverySpeed);
        _handsIK.AddKick(data.kickBack * multiplier, data.kickRotation * multiplier, data.kickReturnSpeed);
    }
}