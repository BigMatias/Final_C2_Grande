public abstract class EnemyBaseState
{
    protected readonly EnemyAI ctx;

    protected EnemyBaseState(EnemyAI context)
    {
        ctx = context;
    }

    public abstract EnemyStateId Id { get; }

    public virtual void Enter()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Exit()
    {
    }
}