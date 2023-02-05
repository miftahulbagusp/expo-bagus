using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Attributes")]
    // Essentials
    [SerializeField]
    private GameObject joystickObject;

    [SerializeField] private GameObject innerJoystickObject;

    // Attributes
    [SerializeField] private float visualDistance = 100f;
    private bool _isPressed;
    private bool _isEnabled = true;

    private RectTransform _joystickRectTransform;
    private RectTransform _innerJoystickRectTransform;

    [Header("Configs")]
    // Configs
    public bool isNormalized;

    // Outputs
    public Vector2 Axis { get; private set; }
    public float Angle { get; private set; }

    private void Start()
    {
        _joystickRectTransform = joystickObject.GetComponent<RectTransform>();
        _innerJoystickRectTransform = innerJoystickObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Adjust raw axis input to thumbstick output
        if (!_isPressed)
        {
            UpdateValue(0, 0);
        }
    }

    private void UpdateValue(float x, float y)
    {
        // Normalize axis output value
        Axis = isNormalized
            ? new Vector2(x, y).normalized
            : new Vector2(x, y);

        // Convert axis to angle
        Angle = Mathf.Atan2(Axis.y, Axis.x) * -(180 / Mathf.PI);

        // Move stick image
        _innerJoystickRectTransform.anchoredPosition = new Vector3(
            Axis.x * visualDistance,
            Axis.y * visualDistance
        );
    }

    public void EnableJoystick(bool value)
    {
        if (value)
        {
            _isEnabled = true;
        }
        else
        {
            _isEnabled = false;
            _isPressed = false;
            _innerJoystickRectTransform.anchoredPosition = Vector2.zero;
            Axis = Vector2.zero;
            Angle = 0f;
        }
    }
    
    #region interface

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isEnabled) return;
        
        _isPressed = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isEnabled) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _joystickRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var stickPos))
        {
            // Calculate axis from pivot to stick position
            stickPos.x = (stickPos.x / _joystickRectTransform.sizeDelta.x);
            stickPos.y = (stickPos.y / _joystickRectTransform.sizeDelta.y);

            stickPos.x += _joystickRectTransform.pivot.x - 0.5f;
            stickPos.y += _joystickRectTransform.pivot.y - 0.5f;

            // Limit raw input
            float x = Mathf.Clamp(stickPos.x, -1, 1);
            float y = Mathf.Clamp(stickPos.y, -1, 1);

            UpdateValue(x, y);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isEnabled) return;

        // Reset value on release
        _isPressed = false;
        UpdateValue(0, 0);
    }

    #endregion
}