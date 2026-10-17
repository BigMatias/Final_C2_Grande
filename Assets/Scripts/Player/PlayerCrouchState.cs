using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    private Vector3 _movement;

    public PlayerCrouchState(PlayerMovement currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        ctx.Col.height = 1f;
        ctx.Look.SetCrouched(true);
    }

    public override void UpdateState()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        _movement = (ctx.transform.forward * inputZ + ctx.transform.right * inputX).normalized;

        if (Input.GetKeyUp(ctx.PlayerSettings.crouchKey))
        {
            ctx.SwitchState(ctx.LocomotionState);
        }
    }

    public override void FixedUpdateState()
    {
        MoveTowardsVelocity(_movement, ctx.PlayerSettings.crouchSpeed);
    }
    
    public override void ExitState()
    {
        ctx.Col.height = 2f;
        ctx.Look.SetCrouched(false);
    }
}