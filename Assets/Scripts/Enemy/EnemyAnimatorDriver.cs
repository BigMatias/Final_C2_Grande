using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorDriver : MonoBehaviour, IPoolable
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    [SerializeField] private Animator _animator;
    [SerializeField] private float _dampTime = 0.1f;

    private NavMeshAgent _agent;
    private EnemyAI _ai;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _ai = GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        _ai.OnAttackStarted += HandleAttackStarted;
    }

    private void OnDisable()
    {
        _ai.OnAttackStarted -= HandleAttackStarted;
    }

    private void Update()
    {
        if (_agent == null || !_agent.enabled || !_animator.enabled)
            return;

        _animator.SetFloat(SpeedHash, _agent.velocity.magnitude, _dampTime, Time.deltaTime);
    }

    private void HandleAttackStarted()
    {
        _animator.SetTrigger(AttackHash);
    }

    public void OnSpawnedFromPool()
    {
        _animator.Rebind();
        _animator.Update(0f);
    }

    public void OnReturnedToPool()
    {
    }
}