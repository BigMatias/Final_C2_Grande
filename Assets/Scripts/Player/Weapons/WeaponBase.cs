using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected WeaponDataSO _weaponDataSo;
    [SerializeField] private Transform _rightHandGrip;
    [SerializeField] private Transform _leftHandGrip;

    private float _nextFireTime;

    public Transform RightHandGrip => _rightHandGrip;
    public Transform LeftHandGrip => _leftHandGrip;

    public void TryShoot(Ray ray)
    {
        if (Time.time < _nextFireTime) return;
        _nextFireTime = Time.time + 1f / _weaponDataSo.fireRate;
        Shoot(ray);
    }

    protected abstract void Shoot(Ray ray);
}