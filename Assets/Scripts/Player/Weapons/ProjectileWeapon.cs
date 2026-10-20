using UnityEngine;

public class ProjectileWeapon : WeaponBase
{
    [SerializeField] private PoolServiceSO _pool;
    [SerializeField] private float _minConvergenceDistance = 1.5f;

    protected override void Fire(Ray aimRay)
    {
        if (_pool == null || Data.projectilePrefab == null || ShootPoint == null)
            return;

        Vector3 origin = ShootPoint.position;
        Vector3 direction = GetFireDirection(aimRay, origin);

        Projectile projectile = _pool.Spawn(Data.projectilePrefab, origin, Quaternion.LookRotation(direction));
        projectile.Launch(origin, direction, Data.projectileSpeed, Data.damage, Data.impactForce, Data.projectileGravity, Data.ProjectileLifetime, Data.hitLayers);
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