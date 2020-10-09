using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joybutton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public Image knob;

    [HideInInspector]
    public bool pressed;


    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        knob.GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        knob.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);        
    }

}
