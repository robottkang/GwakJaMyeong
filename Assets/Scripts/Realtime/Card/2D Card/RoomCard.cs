using System.Collections;
using Room;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Card
{
    public class RoomCard : DragalbeCard
    {
        public override void OnEndDrag(PointerEventData eventData)
        {
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
    }
}
