using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FaceArrowLeft : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    public Image arrow;

    [HideInInspector]
    public bool pressed;


    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 70);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(52, 79);
    }

}
