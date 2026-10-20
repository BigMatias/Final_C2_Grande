using UnityEngine;
using UnityEngine.AI;

public class EnemyStagger : MonoBehaviour
{
    [SerializeField] private float _slowMultiplier = 0.4f;
    [SerializeField] private float _heavyHitDamage = 50f;
    [SerializeField] private float _pushPerForce = 0.03f;
    [SerializeField] private float _maxPush = 0.25f;

    private NavMeshAgent _agent;
    private EnemyHitReceiver _receiver;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _receiver = GetComponent<EnemyHitReceiver>();
    }

    private void OnEnable()
    {
        _receiver.OnHit += HandleHit;
    }

    private void OnDisable()
    {
        _receiver.OnHit -= HandleHit;
    }

    private void HandleHit(DamageInfo info)
    {
        if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh)
            return;

        bool heavyHit = info.Amount >= _heavyHitDamage;
        _agent.velocity *= heavyHit ? 0f : _slowMultiplier;

        Vector3 push = info.Direction;
        push.y = 0f;

        if (push.sqrMagnitude < 0.0001f)
            return;

        float distance = Mathf.Min(info.Force * _pushPerForce, _maxPush);
        _agent.Move(push.normalized * distance);
    }
}