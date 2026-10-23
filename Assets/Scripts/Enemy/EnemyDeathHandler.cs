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
    [SerializeField] private string _corpseLayerName = "Corpse";

    private readonly List<Rigidbody> _ragdollBodies = new List<Rigidbody>();
    private readonly List<GameObject> _ragdollObjects = new List<GameObject>();
    private readonly List<int> _aliveLayers = new List<int>();
    private NavMeshAgent _agent;
    private EnemyHitReceiver _receiver;
    private int _corpseLayer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _receiver = GetComponent<EnemyHitReceiver>();
        _corpseLayer = LayerMask.NameToLayer(_corpseLayerName);

        foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>(true))
        {
            if (body.gameObject == gameObject)
                continue;

            _ragdollBodies.Add(body);
            _ragdollObjects.Add(body.gameObject);
            _aliveLayers.Add(body.gameObject.layer);
        }

        if (_corpseLayer < 0)
            Debug.LogWarning($"{name}: no existe la capa '{_corpseLayerName}'.", this);
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

        for (int i = 0; i < _ragdollBodies.Count; i++)
        {
            Rigidbody body = _ragdollBodies[i];
            body.isKinematic = alive;
            body.interpolation = alive ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;

            if (_corpseLayer >= 0)
                _ragdollObjects[i].layer = alive ? _aliveLayers[i] : _corpseLayer;
        }
    }
}