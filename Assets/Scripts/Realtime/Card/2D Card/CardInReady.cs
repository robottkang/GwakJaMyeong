using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInReady : DragalbeCard
{
    private Transform originParent;

    protected override void Start()
    {
        originParent = transform.parent;
        base.Start();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (targetParent.Equals(originParent))
            transform.position = originParent.transform.position;
        targetParent = originParent;
    }
}
