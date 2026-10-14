public abstract class PlayerBaseState
{
    protected PlayerMovement ctx; 

    public PlayerBaseState(PlayerMovement currentContext)
    {
        ctx = currentContext;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
}