using UnityEngine;

public class PlayerLocomotionState : PlayerBaseState
{
    private Vector3 _movement;
    private float _speedMultiplier;

    public PlayerLocomotionState(PlayerMovement currentContext) : base(currentContext) { }

    public override void EnterState() { }


    public override void UpdateState()
    {
        bool isRunning = Input.GetKey(ctx.PlayerSettings.runkey);
        _speedMultiplier = isRunning ? 2f : 1f;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        _movement = (ctx.transform.forward * inputZ + ctx.transform.right * inputX).normalized;

        float smoothX = Input.GetAxis("Horizontal") * _speedMultiplier;
        float smoothZ = Input.GetAxis("Vertical") * _speedMultiplier;

        ctx.Animator.SetFloat("Horizontal", smoothX);
        ctx.Animator.SetFloat("Vertical", smoothZ);

        if (Input.GetKeyDown(ctx.PlayerSettings.crouchKey))
            ctx.SwitchState(ctx.CrouchState);
    }

    public override void FixedUpdateState()
    {
        float currentY = ctx.Rb.linearVelocity.y;
        float targetSpeed = _speedMultiplier == 2f ? ctx.PlayerSettings.runSpeed : ctx.PlayerSettings.speed;
    
        ctx.Rb.linearVelocity = new Vector3(_movement.x * targetSpeed, currentY, _movement.z * targetSpeed);
    }

    public override void ExitState() { }

    private bool IsGrounded()
    {
        return Physics.Raycast(ctx.transform.position, Vector3.down, 1.1f, ctx.GroundLayer);
    }
}