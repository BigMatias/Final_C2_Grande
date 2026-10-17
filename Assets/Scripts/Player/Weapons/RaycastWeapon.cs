using System.Collections;
using UnityEngine;

public class RaycastWeapon : WeaponBase
{
    [Header("Tracer")]
    [SerializeField] private LineRenderer _bulletTracer;
    [SerializeField] private float _tracerDuration = 0.03f;

    private Coroutine _tracerRoutine;

    private void Awake()
    {
        if (_bulletTracer != null)
            _bulletTracer.enabled = false;
    }

    protected override void Fire(Ray aimRay)
    {
        Vector3 endPoint = aimRay.origin + aimRay.direction * Data.range;

        if (Physics.Raycast(aimRay, out RaycastHit hit, Data.range, Data.hitLayers, QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(Data.damage);
        }

        ShowTracer(endPoint);
    }

    private void ShowTracer(Vector3 endPoint)
    {
        if (_bulletTracer == null || ShootPoint == null)
            return;

        if (_tracerRoutine != null)
            StopCoroutine(_tracerRoutine);

        _tracerRoutine = StartCoroutine(TracerRoutine(endPoint));
    }

    private IEnumerator TracerRoutine(Vector3 endPoint)
    {
        _bulletTracer.useWorldSpace = true;
        _bulletTracer.SetPosition(0, ShootPoint.position);
        _bulletTracer.SetPosition(1, endPoint);
        _bulletTracer.enabled = true;

        yield return new WaitForSeconds(_tracerDuration);

        _bulletTracer.enabled = false;
        _tracerRoutine = null;
    }
}