using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnButtonDown;

    public UnityEvent OnButtonUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnButtonDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnButtonUp?.Invoke();
    }
}