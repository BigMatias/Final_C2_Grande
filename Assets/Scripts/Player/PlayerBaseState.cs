using UnityEngine;

public abstract class PlayerBaseState
{
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private static readonly int VerticalHash = Animator.StringToHash("Vertical");

    protected PlayerMovement ctx;

    public PlayerBaseState(PlayerMovement currentContext)
    {
        ctx = currentContext;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();

    protected void MoveTowardsVelocity(Vector3 direction, float targetSpeed)
    {
        Vector3 velocity = ctx.Rb.linearVelocity;
        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 target = direction * targetSpeed;

        float rate = direction.sqrMagnitude > 0.01f
            ? ctx.PlayerSettings.acceleration
            : ctx.PlayerSettings.deceleration;

        horizontal = Vector3.MoveTowards(horizontal, target, rate * Time.fixedDeltaTime);
        ctx.Rb.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
    }

    protected void UpdateLocomotionAnimator()
    {
        Vector3 velocity = ctx.Rb.linearVelocity;
        Vector3 local = ctx.transform.InverseTransformDirection(new Vector3(velocity.x, 0f, velocity.z));

        ctx.Animator.SetFloat(HorizontalHash, local.x, 0.1f, Time.deltaTime);
        ctx.Animator.SetFloat(VerticalHash, local.z, 0.1f, Time.deltaTime);
    }
}