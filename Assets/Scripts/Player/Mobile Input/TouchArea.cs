using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TouchArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
{
    private bool _isDragged;
    private Vector2 _newAxis;

    // Outputs
    public Vector2 Axis { get; private set; }

    private void Update()
    {
        // Update axis value
        if (_isDragged)
        {
            if (_newAxis == Axis)
            {
                _newAxis = Vector2.zero;
            }

            Axis = _newAxis;
        }
    }

    #region interface

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _newAxis = eventData.delta;
    }

    public void OnDrop(PointerEventData eventData)
    {
        _isDragged = false;
        Axis = Vector2.zero;
    }

    #endregion
}