
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NextShape : MonoBehaviour
{
    public Sprite l;
    public Sprite j;
    public Sprite i;
    public Sprite s;
    public Sprite z;
    public Sprite o;
    public Sprite t;

    public void UpdateShape(int shape)
    {
        switch(shape)
        {
            case 0:
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = i;
                break;
            case 1:
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = j;
                break;
            case 2:
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = l;
                break;
            case 3:
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = o;
                break;
            case 4:
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = s;
                break;
            case 5:
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = t;
                break;
            case 6:
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = z;
                break;
            case -1:
                gameObject.GetComponent<UnityEngine.UI.Image>().sprite = null;
                break;
        }
    }
}
