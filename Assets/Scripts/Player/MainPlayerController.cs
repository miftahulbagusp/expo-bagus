using System;
using UnityEngine;

public abstract class MainPlayerController : BasePlayerController
{
    private bool _isReadyToFall;
    
    protected bool EnableGravity;
    protected float FallCooldown;
    private bool _isFalling;
    
    // Events & Actions
    public Action OnJump;
    public Action<int> OnEmojiSelected;


    // Public 
    public bool IsGrounded => Controller.isGrounded;
    public float GravityForce { get; protected set; }
    public float CurrentMovementSpeed { get; private set; }
    public Vector2 CurrentMovementDirection { get; private set; }
    
    protected override void Navigate() {}
    protected override void Look() {}

    public virtual void Jump()
    {
        if (!InputManager.Instance.enableAvatarMove) return;
        if (!InputManager.Instance.enableJump) return;

        if (Controller.isGrounded && FallCooldown <= 0)
        {
            GravityForce += Mathf.Sqrt((playerSettings.jumpForce * scaleModifier) * -3.0f * Physics.gravity.y);
            OnJump?.Invoke();
        }
    }
    
    protected void CheckFallState()
    {
        if (Controller.isGrounded)
        {
            if (_isFalling)
            {
                _isFalling = false;
                if (GravityForce < (playerSettings.minimumFallForce * scaleModifier))
                {
                    FallCooldown = playerSettings.delayAfterFall;
                }
            }
        }
        else
        {
            _isFalling = true;
        }

        if (FallCooldown >= 0)
        {
            FallCooldown -= Time.deltaTime;
        }
    }
    
    protected void ApplyGravity()
    {
        if (Controller.isGrounded && GravityForce < 0)
        {
            GravityForce = 0f;
        }

        GravityForce += Physics.gravity.y * Time.deltaTime;
    }

    protected override void SetAnimationProperties()
    {
        CurrentMovementDirection = Vector2.Lerp(
            CurrentMovementDirection,
            new Vector2(MovementInputValue.x, MovementInputValue.z),
            playerSettings.avatarTurningSpeed * Time.deltaTime
        );
        CurrentMovementSpeed = Mathf.Max(Mathf.Abs(MovementInputValue.x), Mathf.Abs(MovementInputValue.z));
    }

    protected void PreventFallOnStart()
    {
        if (_isReadyToFall)  return;

        if (Physics.Raycast(
            transform.position, 
            transform.TransformDirection(Vector3.down), 
            out _,
            Mathf.Infinity, 
            ~LayerMask.NameToLayer("Avatar")))
        {
            _isReadyToFall = true;
            EnableGravity = true;
        }
    }
}
