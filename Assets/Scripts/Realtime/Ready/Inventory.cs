using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount >= 11) return;

        AppendChild(eventData.pointerDrag);
    }

    public void AppendChild(GameObject cardObject)
    {
        cardObject.GetComponent<DragalbeCard>().targetParent = transform;
    }
}
