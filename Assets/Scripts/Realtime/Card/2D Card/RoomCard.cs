using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Room;

namespace Card
{
    public class RoomCard : DragalbeCard
    {
        bool canMove = true;

        protected override void Awake()
        {
            base.Awake();

            PageEventBus.Subscribe(Page.Drow, () =>
            {
                canMove = true;
            });
            PageEventBus.Subscribe(Page.Duel, () => 
            {
                canMove = false;
            });
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!canMove) return;

            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!canMove) return;

            base.OnEndDrag(eventData);

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Idea Field"));

            StrategyPlan strategyPlan;
            if (hitInfo.collider != null && (strategyPlan = hitInfo.collider.GetComponentInParent<StrategyPlan>()).PlacedCardInfo == null)
            {
                strategyPlan.PlacedCardInfo = CardInfo;
                ObjectPool.ReturnObject("Hand Pool", gameObject);
            }
            else
            {
                transform.position = OriginPosition;
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!canMove) return;

            base.OnDrag(eventData);
        }
    }
}
