using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DeckBuilding;

namespace Card
{
    public class DeckBuildingCard : DragalbeCard
    {
        public bool addedToInventory = false;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            addedToInventory = false;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            if (!addedToInventory)
            {
                Inventory.RemoveCard(gameObject);
                transform.position = OriginParent.position;
                transform.SetParent(OriginParent);
                transform.GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
            }
        }
    }
}
