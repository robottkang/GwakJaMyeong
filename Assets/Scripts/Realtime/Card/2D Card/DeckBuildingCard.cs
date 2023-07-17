using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Card
{
    public class DeckBuildingCard : DragalbeCard
    {
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out IDropHandler _))
            {
                transform.position = OriginPosition;
                transform.SetParent(OriginParent);
            }
            
            base.OnEndDrag(eventData);
        }
    }
}
