using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class DropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void DropItemMoveStartEvent(DropItem item);
    public event DropItemMoveStartEvent onMoveStart;

    public delegate void DropItemMoveEndEvent(DropItem item);
    public event DropItemMoveEndEvent onMoveEnd;

    public delegate void DropItemNoEvent(DropItem item);
    public event DropItemNoEvent onNothing;

    private RectTransform rectTransform;
    private RectTransform clampRectTransform;

    private Vector3 originalWorldPos;
    private Vector3 originalRectWorldPos;

    private Vector3 minWorldPosition;
    private Vector3 maxWorldPosition;

    private DropArea droppedArea;
    private DropArea prevDropArea;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        clampRectTransform = rectTransform.root.GetComponent<RectTransform>();
    }

    public void SetDroppedArea(DropArea dropArea)
    {
        this.droppedArea = dropArea;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        originalRectWorldPos = rectTransform.position;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(clampRectTransform, eventData.position, eventData.pressEventCamera, out originalWorldPos);

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onMoveStart != null) onMoveStart(this);
        DropArea.SetDropArea(true);

        if (droppedArea != null) droppedArea.TriggerOnLift(this);
        prevDropArea = droppedArea;
        droppedArea = null;

        Rect clamp = new Rect(Vector2.zero, clampRectTransform.rect.size);
        Vector3 minPosition = clamp.min - rectTransform.rect.min;
        Vector3 maxPosition = clamp.max - rectTransform.rect.max;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(clampRectTransform, minPosition, eventData.pressEventCamera, out minWorldPosition);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(clampRectTransform, maxPosition, eventData.pressEventCamera, out maxWorldPosition);

        Debug.Log(minWorldPosition + "/" + maxWorldPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPointerPosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(clampRectTransform, eventData.position, eventData.pressEventCamera, out worldPointerPosition))
        {
            Vector3 offsetToOriginal = worldPointerPosition - originalRectWorldPos;
            rectTransform.position = originalRectWorldPos + offsetToOriginal;
        }

        Vector3 worldPos = rectTransform.position;
        worldPos.x = Mathf.Clamp(rectTransform.position.x, minWorldPosition.x, maxWorldPosition.x);
        worldPos.y = Mathf.Clamp(rectTransform.position.y, minWorldPosition.y, maxWorldPosition.y);
        rectTransform.position = worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DropArea.SetDropArea(false);
        if (onMoveEnd != null) onMoveEnd(this);

        bool noEvent = true;
        foreach (var go in eventData.hovered)
        {
            // Debug. Log("on end drag : " + go.name) ;
            var dropArea = go.GetComponent<DropArea>();
            if (dropArea != null)
            {
                noEvent = false;
                break;
            }
        }
        if (noEvent)
        {
            if (onNothing != null) onNothing(this);
        }

    }

}
