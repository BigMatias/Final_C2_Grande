using UnityEngine;

public class PlayerAirborneState : PlayerBaseState
{
    public PlayerAirborneState(PlayerMovement currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        ctx.Rb.AddForce(Vector3.up * ctx.PlayerSettings.jumpForce, ForceMode.Impulse);
    }

    public override void UpdateState()
    {
        if (ctx.Rb.linearVelocity.y <= 0.1f && IsGrounded())
        {
            ctx.SwitchState(ctx.LocomotionState);
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState() { }

    private bool IsGrounded()
    {
        return Physics.Raycast(ctx.transform.position, Vector3.down, 1.1f, ctx.GroundLayer);
    }
}