using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDeathHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _mainCollider;
    [SerializeField] private Behaviour[] _disableOnDeath;
    [SerializeField] private float _deathForceMultiplier = 1.5f;
    [SerializeField] private float _upwardBias = 0.25f;

    private readonly List<Rigidbody> _ragdollBodies = new List<Rigidbody>();
    private NavMeshAgent _agent;
    private EnemyHitReceiver _receiver;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _receiver = GetComponent<EnemyHitReceiver>();

        foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>(true))
        {
            if (body.gameObject != gameObject)
                _ragdollBodies.Add(body);
        }
    }

    private void OnEnable()
    {
        SetAlive(true);
        _receiver.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        _receiver.OnDied -= HandleDied;
    }

    private void HandleDied(DamageInfo lastHit)
    {
        SetAlive(false);

        if (lastHit.HitBody == null)
            return;

        Vector3 direction = (lastHit.Direction + Vector3.up * _upwardBias).normalized;
        lastHit.HitBody.AddForceAtPosition(direction * lastHit.Force * _deathForceMultiplier, lastHit.Point, ForceMode.Impulse);
    }

    private void SetAlive(bool alive)
    {
        foreach (Behaviour behaviour in _disableOnDeath)
        {
            if (behaviour != null)
                behaviour.enabled = alive;
        }

        if (_agent != null)
            _agent.enabled = alive;

        if (_animator != null)
            _animator.enabled = alive;

        if (_mainCollider != null)
            _mainCollider.enabled = alive;

        foreach (Rigidbody body in _ragdollBodies)
        {
            body.isKinematic = alive;
            body.interpolation = alive ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
        }
    }
}