using System;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Camera _camera;
    [SerializeField] private WeaponDataSO gun;
    [SerializeField] private Transform _rightHandIKTarget;
    [SerializeField] private Transform _leftHandIKTarget;

    private WeaponBase _currentWeapon;
    public event Action OnPlayerShot;

    private void Start()
    {
        EquipWeapon(gun);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryShoot();
    }

    public void GetGrips()
    {
        if (_currentWeapon == null) return;

        if (_currentWeapon.RightHandGrip != null)
        {
            _rightHandIKTarget.position = _currentWeapon.RightHandGrip.position;
            _rightHandIKTarget.rotation = _currentWeapon.RightHandGrip.rotation;
        }

        if (_currentWeapon.LeftHandGrip != null)
        {
            _leftHandIKTarget.position = _currentWeapon.LeftHandGrip.position;
            _leftHandIKTarget.rotation = _currentWeapon.LeftHandGrip.rotation;
        }
    }

    public void EquipWeapon(WeaponDataSO weaponData)
    {
        if (_currentWeapon != null)
            Destroy(_currentWeapon.gameObject);

        GameObject go = Instantiate(weaponData.weaponPrefab, _weaponHolder);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        _currentWeapon = go.GetComponent<WeaponBase>();
    }

    private void TryShoot()
    {
        if (_currentWeapon == null) return;
        OnPlayerShot?.Invoke();
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        _currentWeapon.TryShoot(ray);
    }
}