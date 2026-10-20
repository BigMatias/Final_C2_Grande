using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] private TrailRenderer _trail;

    private Vector3 _velocity;
    private float _damage;
    private float _impactForce;
    private float _gravityMultiplier;
    private float _remainingLifetime;
    private LayerMask _hitLayers;
    private bool _isFlying;

    public void Launch(Vector3 position, Vector3 direction, float speed, float damage, float impactForce, float gravityMultiplier, float lifetime, LayerMask hitLayers)
    {
        transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));

        _velocity = direction * speed;
        _damage = damage;
        _impactForce = impactForce;
        _gravityMultiplier = gravityMultiplier;
        _remainingLifetime = lifetime;
        _hitLayers = hitLayers;
        _isFlying = true;

        if (_trail != null)
            _trail.Clear();
    }

    public void OnSpawnedFromPool()
    {
        if (_trail != null)
            _trail.Clear();
    }

    public void OnReturnedToPool()
    {
        _isFlying = false;
    }

    private void Update()
    {
        if (!_isFlying)
            return;

        float deltaTime = Time.deltaTime;
        _remainingLifetime -= deltaTime;

        if (_remainingLifetime <= 0f)
        {
            Despawn();
            return;
        }

        _velocity += Physics.gravity * (_gravityMultiplier * deltaTime);

        Vector3 start = transform.position;
        Vector3 displacement = _velocity * deltaTime;
        float distance = displacement.magnitude;

        if (distance <= 0f)
            return;

        if (Physics.Raycast(start, displacement / distance, out RaycastHit hit, distance, _hitLayers, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
            HandleImpact(hit);
            Despawn();
            return;
        }

        transform.position = start + displacement;
        transform.rotation = Quaternion.LookRotation(_velocity);
    }

    private void HandleImpact(RaycastHit hit)
    {
        DamageInfo info = new DamageInfo(_damage, hit.point, _velocity.normalized, _impactForce);

        if (hit.collider.TryGetComponent(out IHittable hittable))
        {
            hittable.ReceiveHit(info);
            return;
        }

        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(_damage);

        if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
            hit.rigidbody.AddForceAtPosition(info.Direction * _impactForce, hit.point, ForceMode.Impulse);
    }

    private void Despawn()
    {
        if (!_isFlying)
            return;

        _isFlying = false;
        PooledObject.Despawn(gameObject);
    }
}