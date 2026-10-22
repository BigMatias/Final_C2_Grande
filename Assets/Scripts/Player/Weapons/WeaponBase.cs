using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Grips")]
    [SerializeField] private Transform _rightHandGrip;
    [SerializeField] private Transform _leftHandGrip;

    [Header("Points")]
    [SerializeField] private Transform _sightPoint;
    [SerializeField] private Transform _shootPoint;

    [Header("Effects")]
    [SerializeField] private MuzzleFlash _muzzleFlash;

    public WeaponDataSO Data { get; private set; }
    public Transform RightHandGrip => _rightHandGrip;
    public Transform LeftHandGrip => _leftHandGrip;
    public Transform SightPoint => _sightPoint;
    public Transform ShootPoint => _shootPoint;

    public event Action OnFired;

    private float _nextFireTime;

    public void Initialize(WeaponDataSO data)
    {
        Data = data;
        _nextFireTime = 0f;
        OnInitialized();
    }

    protected virtual void OnInitialized()
    {
    }

    public bool TryFire(Ray aimRay)
    {
        if (Data == null || Time.time < _nextFireTime)
            return false;

        _nextFireTime = Time.time + Data.TimeBetweenShots;

        Fire(aimRay);

        if (_muzzleFlash != null)
            _muzzleFlash.Play();

        OnFired?.Invoke();
        return true;
    }

    protected abstract void Fire(Ray aimRay);
}