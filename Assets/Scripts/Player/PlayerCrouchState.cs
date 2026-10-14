using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    private Vector3 _movement;

    public PlayerCrouchState(PlayerMovement currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        ctx.Col.height = 1f; 
        ctx.MainCamera.transform.localPosition = ctx.CrouchCameraPos;
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
        float currentY = ctx.Rb.linearVelocity.y;
        ctx.Rb.linearVelocity = new Vector3(_movement.x * ctx.PlayerSettings.crouchSpeed, currentY, _movement.z * ctx.PlayerSettings.crouchSpeed);
    }

    public override void ExitState()
    {
        ctx.Col.height = 2f; 
        ctx.MainCamera.transform.localPosition = ctx.StandCameraPos;
    }
}