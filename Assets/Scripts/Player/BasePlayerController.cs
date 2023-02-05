using System;
using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    // // Attributes
    protected Vector3 MovementInputValue;
    protected Vector3 LookRotation;
    
    protected CharacterController Controller;

    [SerializeField] protected Settings.PlayerSettings playerSettings;
    
    // Inputs
    protected DefaultInputAction Input;

    protected float scaleModifier = 1f;
    public Action<float> OnScaleChanged;

    protected abstract void SetAnimationProperties();

    protected abstract void Navigate();
    protected abstract void Look();

    public virtual void ChangeScale(float scaleModifier)
    {
        this.scaleModifier = scaleModifier;
        Vector3 newScale = Vector3.one * Mathf.Clamp(scaleModifier, 1, 2); 
        transform.localScale = newScale;
        OnScaleChanged?.Invoke(newScale.x);
    }
}
