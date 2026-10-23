using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{
    private float _nextRepathTime;

    public EnemyChaseState(EnemyAI context) : base(context) { }

    public override EnemyStateId Id => EnemyStateId.Chase;

    public override void Enter()
    {
        _nextRepathTime = 0f;

        NavMeshAgent agent = ctx.Agent;
        agent.avoidancePriority = ctx.ChasePriority;
        agent.updateRotation = true;

        if (!agent.isOnNavMesh)
            return;

        agent.nextPosition = ctx.transform.position;
        agent.updatePosition = true;
        agent.isStopped = false;
    }

    public override void Update()
    {
        if (Time.time >= _nextRepathTime)
        {
            ctx.Agent.SetDestination(ctx.Target.position);
            _nextRepathTime = Time.time + ctx.Data.repathInterval;
        }

        if (!ctx.IsTargetInRange(ctx.Data.attackRange))
            return;

        ctx.FaceTarget();

        if (Time.time >= ctx.NextAttackTime)
            ctx.SwitchState(ctx.AttackState);
    }
}