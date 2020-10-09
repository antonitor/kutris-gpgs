using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    Vector3 initialPos;

    protected override void Start()
    {
        initialPos = background.transform.position;
        base.Start();
        //background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        //background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(initialPos);
        //background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
}