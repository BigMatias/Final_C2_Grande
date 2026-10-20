using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Chase,
        Attack
    }

    [SerializeField] private EnemyDataSO _data;

    public event Action OnAttackStarted;

    private NavMeshAgent _agent;
    private Transform _target;
    private IDamageable _targetDamageable;
    private State _state;
    private float _nextRepathTime;
    private float _nextAttackTime;
    private float _attackTimer;
    private bool _hitApplied;

    public EnemyDataSO Data => _data;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Transform target)
    {
        _target = target;
        _targetDamageable = target != null ? target.GetComponent<IDamageable>() : null;
        _nextAttackTime = 0f;

        ConfigureAgent();
        EnterChase();
    }

    private void ConfigureAgent()
    {
        float variation = UnityEngine.Random.Range(_data.speedVariation.x, _data.speedVariation.y);

        _agent.speed = _data.moveSpeed * variation;
        _agent.acceleration = _data.acceleration;
        _agent.angularSpeed = _data.angularSpeed;
        _agent.stoppingDistance = _data.attackRange * 0.8f;
    }

    private void Update()
    {
        if (_target == null || !_agent.enabled || !_agent.isOnNavMesh)
            return;

        if (_state == State.Chase)
            UpdateChase();
        else
            UpdateAttack();
    }

    private void EnterChase()
    {
        _state = State.Chase;
        _nextRepathTime = 0f;

        if (_agent.enabled && _agent.isOnNavMesh)
            _agent.isStopped = false;

        _agent.updateRotation = true;
    }

    private void UpdateChase()
    {
        if (Time.time >= _nextRepathTime)
        {
            _agent.SetDestination(_target.position);
            _nextRepathTime = Time.time + _data.repathInterval;
        }

        if (!IsTargetInRange(_data.attackRange))
            return;

        FaceTarget();

        if (Time.time >= _nextAttackTime)
            EnterAttack();
    }

    private void EnterAttack()
    {
        _state = State.Attack;
        _attackTimer = 0f;
        _hitApplied = false;

        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _agent.updateRotation = false;

        OnAttackStarted?.Invoke();
    }

    private void UpdateAttack()
    {
        _attackTimer += Time.deltaTime;
        FaceTarget();

        if (!_hitApplied && _attackTimer >= _data.hitDelay)
        {
            _hitApplied = true;
            TryHitTarget();
        }

        if (_attackTimer >= _data.attackDuration)
        {
            _nextAttackTime = Time.time + _data.attackCooldown;
            EnterChase();
        }
    }

    private void TryHitTarget()
    {
        if (_targetDamageable == null)
            return;

        if (IsTargetInRange(_data.attackRange + _data.hitRangeTolerance))
            _targetDamageable.TakeDamage(_data.attackDamage);
    }

    private void FaceTarget()
    {
        Vector3 direction = _target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _data.angularSpeed * Time.deltaTime);
    }

    private bool IsTargetInRange(float range)
    {
        Vector3 offset = _target.position - transform.position;
        offset.y = 0f;
        return offset.sqrMagnitude <= range * range;
    }
}