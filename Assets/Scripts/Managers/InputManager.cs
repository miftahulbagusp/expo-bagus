using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public DefaultInputAction input;

    // Settings
    public bool enableAvatarMove;
    public bool enableJump;
    public bool enableSprint;

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        input = new DefaultInputAction();
        input.Menu.ToggleCursorLock.performed += context => LockCursor();
    }

    public void LockCursor(bool? setLocked = null)
    {
        // Return if on mobile platform
        if (MultiPlatformHelper.Instance.IsPlayingOnAndroid()) return;

        // Set cursor state
        if (setLocked != null)
        {
            if ((bool)setLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            Cursor.lockState = Cursor.visible ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !Cursor.visible;
        }

        if (Cursor.lockState == CursorLockMode.None)
        {
            EnableMovementInput(false);
        }
        else
        {
            EnableMovementInput(true);
        }
    }

    public void EnableMovementInput(bool enable)
    {
        if (enable)
        {
            if (enableAvatarMove)
            {
                input.Universal.Enable();
            }
            else
            {
                input.Universal.Disable();
            }
        }
        else
        {
            input.Universal.Disable();
        }
    }
}