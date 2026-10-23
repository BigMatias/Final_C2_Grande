using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorDriver : MonoBehaviour, IPoolable
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int StateHash = Animator.StringToHash("State");

    [SerializeField] private Animator _animator;
    [SerializeField] private float _dampTime = 0.1f;

    private NavMeshAgent _agent;
    private EnemyAI _ai;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _ai = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        if (_agent == null || !_agent.enabled || !_animator.enabled)
            return;

        _animator.SetFloat(SpeedHash, _agent.velocity.magnitude, _dampTime, Time.deltaTime);
        _animator.SetInteger(StateHash, (int)_ai.CurrentStateId);
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