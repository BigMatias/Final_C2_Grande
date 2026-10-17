using UnityEngine;

public class PlayerLocomotionState : PlayerBaseState
{
    private Vector3 _movement;
    private float _targetSpeed;

    public PlayerLocomotionState(PlayerMovement currentContext) : base(currentContext) { }

    public override void EnterState() { }

    public override void UpdateState()
    {
        bool isRunning = Input.GetKey(ctx.PlayerSettings.runkey);
        _targetSpeed = isRunning ? ctx.PlayerSettings.runSpeed : ctx.PlayerSettings.speed;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        _movement = (ctx.transform.forward * inputZ + ctx.transform.right * inputX).normalized;

        UpdateLocomotionAnimator();

        if (Input.GetKeyDown(ctx.PlayerSettings.crouchKey))
            ctx.SwitchState(ctx.CrouchState);
    }

    public override void FixedUpdateState()
    {
        MoveTowardsVelocity(_movement, _targetSpeed);
    }

    public override void ExitState() { }
}