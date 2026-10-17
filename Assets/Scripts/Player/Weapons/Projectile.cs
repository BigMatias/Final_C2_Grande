using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trail;

    private Vector3 _velocity;
    private float _damage;
    private float _gravityMultiplier;
    private float _remainingLifetime;
    private LayerMask _hitLayers;
    private Action<Projectile> _release;
    private bool _isFlying;

    public void SetRelease(Action<Projectile> release)
    {
        _release = release;
    }

    public void Launch(Vector3 position, Vector3 direction, float speed, float damage, float gravityMultiplier, float lifetime, LayerMask hitLayers)
    {
        transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));

        _velocity = direction * speed;
        _damage = damage;
        _gravityMultiplier = gravityMultiplier;
        _remainingLifetime = lifetime;
        _hitLayers = hitLayers;
        _isFlying = true;

        if (_trail != null)
            _trail.Clear();
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

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(_damage);

            Despawn();
            return;
        }

        transform.position = start + displacement;
        transform.rotation = Quaternion.LookRotation(_velocity);
    }

    private void Despawn()
    {
        if (!_isFlying)
            return;

        _isFlying = false;

        if (_release != null)
            _release(this);
        else
            Destroy(gameObject);
    }
}