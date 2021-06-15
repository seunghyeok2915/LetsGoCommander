using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropTest : MonoBehaviour
{
    public DropArea dropArea;

    public RectTransform dropRectParent;
    public RectTransform hoverRectParent;

    private void Awake()
    {
        dropArea.onLifted += ObjectLiftedFromDrop;
        dropArea.onDropped += ObjectDroppedToDrop;
    }

    private void ObjectLiftedFromDrop(DropArea area, GameObject gameObject)
    {
        gameObject.transform.SetParent(hoverRectParent, true);
    }

    private void ObjectDroppedToDrop(DropArea area, GameObject gameObject)
    {
        gameObject.transform.SetParent(dropRectParent, true);
    }

    private void SetDropArea(bool active)
    {
        dropArea.gameObject.SetActive(active);
    }
}
