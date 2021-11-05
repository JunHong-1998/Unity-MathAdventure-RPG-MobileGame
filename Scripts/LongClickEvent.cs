using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongClickEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerDown;
    public UnityEvent onLongClick;
    [SerializeField] private Image fillImage;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
    }

    private void Update()
    {
        if (pointerDown)
        {
            if (onLongClick != null) onLongClick.Invoke();
            AddEffect();
        }
        else fillImage.fillAmount = 0;
    }

    private void AddEffect()
    {
        if (fillImage.fillAmount < 1) fillImage.fillAmount += 0.2f;
        else fillImage.fillAmount = 0;
    }
}
