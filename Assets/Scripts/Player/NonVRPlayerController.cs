using System;
using System.Collections;
using UnityEngine;

public class NonVRPlayerController : MainPlayerController
{
    public static NonVRPlayerController Instance;

    // Camera Setting
    [Header("Perspective Setting")] 
    public Transform cameraPivot;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform idleWalkCameraPivot;
    [SerializeField] private Transform runCameraPivot;
    public Transform avatarPivot;

    // Inputs
    [Header("Mobile Input")] 
    [SerializeField] private Joystick moveInput;
    [SerializeField] private TouchArea lookInput;

    // Events & Actions
    public Action<int> OnEmoteSelected;

    // Prevent update movement from user input when player teleported 
    private bool _isTeleported;
    
    // Interaction
    // [SerializeField] private Camera camera;
    // private IInteractable interactable;
    // public Action<IInteractable> OnInteractableChanged;
    // public Action<IInteractable> OnInteractableInvoked;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;

        Controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Input = InputManager.Instance.input;

        Input.Universal.Jump.performed += context => Jump();
        // Input.Universal.Interact.performed += context => InvokeInteractable();
        // Input.Menu.ToggleMicrophone.performed += context => ToggleMicrophone();

        LookRotation = cameraPivot.localEulerAngles;

        // if (AudioManager.Instance.isOnMic)
        //     ToggleMicrophone();
    }

    private void Update()
    {
        PreventFallOnStart();
        
        if (EnableGravity)
        {
            CheckFallState();
            ApplyGravity();
        }

        if (!_isTeleported)
        {
            Navigate();
        }
        Look();

        AdjustPerspective();

        SetAnimationProperties();

        // DetectInteractable();
    }

    private void FixedUpdate()
    {
        AvatarFaceDirection();
    }
    
    protected override void Navigate()
    {
        if (Controller.isGrounded)
        {
            if (FallCooldown > 0 || !InputManager.Instance.enableAvatarMove)
            {
                MovementInputValue = Vector3.zero;
            }
            else
            {
                // Read movement input value

                // Joystick input
                MovementInputValue = new Vector3(moveInput.Axis.x, 0, moveInput.Axis.y);

                // Keyboard input            
                Vector2 axis = Input.Universal.Navigate.ReadValue<Vector2>();
                if (axis != Vector2.zero)
                {
                    MovementInputValue = Input.Universal.Sprint.IsPressed()
                        ? new Vector3(axis.x, 0, axis.y)
                        : new Vector3(axis.x / 2, 0, axis.y / 2);
                }

                if (!InputManager.Instance.enableSprint)
                {
                    MovementInputValue = new Vector3(
                        Mathf.Clamp(MovementInputValue.x, -0.5f, 0.5f),
                        0,
                        Mathf.Clamp(MovementInputValue.z, -0.5f, 0.5f)
                    );
                }
            }

        }

        // Apply movement
        Vector3 newPosition;
        if (MovementInputValue.z < 0)
        {
            newPosition = avatarPivot.transform.TransformDirection(MovementInputValue) * ((playerSettings.moveSpeed * scaleModifier)/ 2);
        }
        else
        {
            newPosition = avatarPivot.transform.TransformDirection(MovementInputValue) * (playerSettings.moveSpeed * scaleModifier);
        }

        newPosition.y += GravityForce;
        Controller.Move(newPosition * Time.deltaTime);
    }

    protected override void Look()
    {
        Vector2 axis = Vector2.zero;

        if (InputManager.Instance.enableAvatarMove)
        {
            // if (MultiPlatformHelper.Instance.IsPlayingOnAndroid())
            // {
                axis = lookInput.Axis * playerSettings.touchSensitivity;
            // }

            // if (Input.Universal.Look.ReadValue<Vector2>() != Vector2.zero)
            // {
            //     if (MultiPlatformHelper.Instance.IsPlayingOnDesktop())
            //     {
            //         axis = Input.Universal.Look.ReadValue<Vector2>() * playerSettings.mouseSensitivity;
            //     }
            // }
        }

        LookRotation.x += axis.y;
        LookRotation.y += axis.x;
        LookRotation.x = Mathf.Clamp(LookRotation.x, -90, 90);

        cameraPivot.transform.localEulerAngles = new Vector3(-LookRotation.x, LookRotation.y, 0);
    }

    private void AvatarFaceDirection()
    {
        if (MovementInputValue != Vector3.zero && Controller.isGrounded)
        {
            Quaternion targetAngle = cameraPivot.localRotation;
            targetAngle.x = 0;
            targetAngle.z = 0;

            avatarPivot.localRotation = Quaternion.RotateTowards(avatarPivot.localRotation,
                targetAngle, 5);
        }
    }

    private void AdjustPerspective()
    {
        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            Math.Abs(Mathf.Max(Mathf.Abs(MovementInputValue.z), Mathf.Abs(MovementInputValue.x))) <= 0.5f
                ? idleWalkCameraPivot.position
                : runCameraPivot.position,
            playerSettings.cameraLerpSpeed * Time.deltaTime
        );
    }
    
    // private void DetectInteractable()
    // {
    //     // Interaction
    //     IInteractable newInteractable = null;
    //     
    //     Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 2));
    //     if (Physics.Raycast(ray, out var hit, 4f))
    //     {
    //         if (hit.collider.GetComponent<IInteractable>() != null)
    //         {
    //             newInteractable = hit.collider.GetComponent<IInteractable>();
    //         }
    //     }
    //
    //     if (interactable != newInteractable)
    //     {
    //         interactable = newInteractable;
    //         OnInteractableChanged?.Invoke(interactable);
    //     }
    // }

    // public void InvokeInteractable()
    // {
    //     OnInteractableInvoked?.Invoke(interactable);
    // }

    public void Relocate(Vector3 position, Quaternion rotation)
    {
        IEnumerator Relocate()
        {
            _isTeleported = true;

            yield return new WaitForEndOfFrame();

            transform.position = position;
            avatarPivot.rotation = rotation;

            _isTeleported = false;
        }

        StartCoroutine(Relocate());
    }
}