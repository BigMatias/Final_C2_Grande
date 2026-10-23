using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyDataSO _data;
    [SerializeField] private Transform _testTarget;

    public EnemyDataSO Data => _data;
    public NavMeshAgent Agent { get; private set; }
    public Transform Target { get; private set; }
    public IDamageable TargetDamageable { get; private set; }
    public int ChasePriority { get; private set; }
    public float NextAttackTime { get; set; }

    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }

    public EnemyStateId CurrentStateId => _currentState != null ? _currentState.Id : EnemyStateId.Chase;

    private EnemyBaseState _currentState;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        ChaseState = new EnemyChaseState(this);
        AttackState = new EnemyAttackState(this);
    }

    private void Start()
    {
        if (Target == null && _testTarget != null)
            Initialize(_testTarget);
    }

    public void Initialize(Transform target)
    {
        Target = target;
        TargetDamageable = target != null ? target.GetComponent<IDamageable>() : null;
        NextAttackTime = 0f;
        ChasePriority = Random.Range(_data.chaseAvoidancePriority.x, _data.chaseAvoidancePriority.y + 1);

        ConfigureAgent();

        _currentState = null;
        SwitchState(ChaseState);
    }

    private void Update()
    {
        if (_currentState == null || Target == null || !Agent.enabled || !Agent.isOnNavMesh)
            return;

        _currentState.Update();
    }

    public void SwitchState(EnemyBaseState nextState)
    {
        _currentState?.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }

    public bool IsTargetInRange(float range)
    {
        Vector3 offset = Target.position - transform.position;
        offset.y = 0f;
        return offset.sqrMagnitude <= range * range;
    }

    public void FaceTarget()
    {
        Vector3 direction = Target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _data.angularSpeed * Time.deltaTime);
    }

    private void ConfigureAgent()
    {
        float variation = Random.Range(_data.speedVariation.x, _data.speedVariation.y);

        Agent.speed = _data.moveSpeed * variation;
        Agent.acceleration = _data.acceleration;
        Agent.angularSpeed = _data.angularSpeed;
        Agent.stoppingDistance = _data.attackRange * 0.8f;
    }
}