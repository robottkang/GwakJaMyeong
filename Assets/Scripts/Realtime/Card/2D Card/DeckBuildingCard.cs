using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DeckBuilding;

namespace Card
{
    public class DeckBuildingCard : DragalbeCard
    {
        private bool isInInventroy = false;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (isInInventroy)
            {
                Inventory.AddToInventory(gameObject);
                isInInventroy = true;
            }
            else
            {
                transform.position = OriginPosition;
                Inventory.RemoveFromInventory(gameObject);
                isInInventroy = false;
            }
        }

        public override void OnDrag(PointerEventData eventData) { }

        public override void OnEndDrag(PointerEventData eventData) { }
    }
}
