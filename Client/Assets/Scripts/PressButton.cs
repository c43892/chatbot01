using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onButtonDown;
    public UnityEvent onButtonUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        onButtonDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onButtonUp.Invoke();
    }
}