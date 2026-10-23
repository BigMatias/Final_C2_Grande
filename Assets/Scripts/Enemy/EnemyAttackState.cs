using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    private float _timer;
    private bool _hitApplied;

    public EnemyAttackState(EnemyAI context) : base(context) { }

    public override EnemyStateId Id => EnemyStateId.Attack;

    public override void Enter()
    {
        _timer = 0f;
        _hitApplied = false;

        NavMeshAgent agent = ctx.Agent;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.avoidancePriority = ctx.Data.attackAvoidancePriority;
    }

    public override void Update()
    {
        ctx.Agent.nextPosition = ctx.transform.position;

        _timer += Time.deltaTime;
        ctx.FaceTarget();

        if (!_hitApplied && _timer >= ctx.Data.hitDelay)
        {
            _hitApplied = true;
            TryHitTarget();
        }

        if (_timer >= ctx.Data.attackDuration)
            ctx.SwitchState(ctx.ChaseState);
    }

    public override void Exit()
    {
        ctx.NextAttackTime = Time.time + ctx.Data.attackCooldown;
    }

    private void TryHitTarget()
    {
        if (ctx.TargetDamageable == null)
            return;

        if (ctx.IsTargetInRange(ctx.Data.attackRange + ctx.Data.hitRangeTolerance))
            ctx.TargetDamageable.TakeDamage(ctx.Data.attackDamage);
    }
}