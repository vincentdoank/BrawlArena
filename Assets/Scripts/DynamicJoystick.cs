using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MoreMountains.Tools;

public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    public Camera uiCamera;
    public RectTransform joystick;
    public MMTouchJoystick touchJoystick;

    public void OnDrag(PointerEventData eventData)
    {
        touchJoystick.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        touchJoystick.OnEndDrag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        joystick.position = eventData.pointerCurrentRaycast.worldPosition;
        touchJoystick.transform.localPosition = Vector3.zero;
        touchJoystick.SetNeutralPosition(joystick.position);
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //touchJoystick.OnEndDrag(eventData);
    }
}
