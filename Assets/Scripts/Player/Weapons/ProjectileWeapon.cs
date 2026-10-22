using UnityEngine;

public class ProjectileWeapon : WeaponBase
{
    [SerializeField] private PoolServiceSO _pool;
    [SerializeField] private float _minConvergenceDistance = 1.5f;

    protected override void OnInitialized()
    {
        if (!IsConfigured())
            return;

        _pool.Prewarm(Data.projectilePrefab, Data.projectilePoolSize);
    }

    protected override void Fire(Ray aimRay)
    {
        if (!IsConfigured())
            return;

        Vector3 origin = ShootPoint.position;
        Vector3 direction = GetFireDirection(aimRay, origin);

        Projectile projectile = _pool.Spawn(Data.projectilePrefab, origin, Quaternion.LookRotation(direction));
        projectile.Launch(origin, direction, Data.projectileSpeed, Data.damage, Data.impactForce, Data.projectileGravity, Data.ProjectileLifetime, Data.hitLayers);
    }

    private bool IsConfigured()
    {
        if (_pool == null)
        {
            Debug.LogWarning($"{name}: falta asignar el Pool en ProjectileWeapon.", this);
            return false;
        }

        if (Data.projectilePrefab == null)
        {
            Debug.LogWarning($"{name}: el WeaponData no tiene Projectile Prefab asignado.", this);
            return false;
        }

        if (ShootPoint == null)
        {
            Debug.LogWarning($"{name}: falta asignar el Shoot Point.", this);
            return false;
        }

        return true;
    }

    private Vector3 GetFireDirection(Ray aimRay, Vector3 origin)
    {
        Vector3 aimPoint = aimRay.origin + aimRay.direction * Data.range;

        if (Physics.Raycast(aimRay, out RaycastHit hit, Data.range, Data.hitLayers, QueryTriggerInteraction.Ignore))
            aimPoint = hit.point;

        Vector3 toAimPoint = aimPoint - origin;

        if (toAimPoint.magnitude < _minConvergenceDistance)
            return aimRay.direction;

        return toAimPoint.normalized;
    }
}