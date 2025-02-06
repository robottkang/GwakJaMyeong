using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DeckBuilding;

namespace Card
{
    public class DeckBuildingCard : DragalbeCard, IPointerDownHandler, IPointerClickHandler
    {
        public bool addedToInventory = false;
        private float timeToStartButtonDown = 0f;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            addedToInventory = false;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            if (!addedToInventory) ComeBackToSpace();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            timeToStartButtonDown = Time.time;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (timeToStartButtonDown - 0.5f < Time.time && eventData.button == PointerEventData.InputButton.Left)
            {
                if (!addedToInventory)
                    FindObjectOfType<Inventory>().OnDrop(eventData);
                else
                    ComeBackToSpace();
            }
        }

        private void ComeBackToSpace()
        {
            Inventory.RemoveCard(gameObject);
            transform.position = OriginParent.position;
            transform.SetParent(OriginParent);
            transform.GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
        }
    }
}
