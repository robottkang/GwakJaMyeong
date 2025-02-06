using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Room;

namespace Card
{
    public class RoomCard : DragalbeCard
    {
        [ReadOnly, SerializeField]
        private bool canMove = false;

        protected override void Awake()
        {
            base.Awake();

            PhaseEventBus.Subscribe(Phase.StrategyPlan, () =>
            {
                canMove = true;
            });
            PhaseEventBus.Subscribe(Phase.Duel, () => 
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

            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Plan Field"));

            StrategyPlan strategyPlan;
            if (hitInfo.collider != null && (strategyPlan = hitInfo.collider.GetComponentInParent<StrategyPlan>()).PlacedPlanCardOnTop == null)
            {
                GameObject planCard = ObjectPool.GetObject("Card Pool");
                planCard.GetComponent<PlanCard>().Initialize(CardData, PlayerController.Instance);
                planCard.GetComponent<PlanCard>().CanMove = true;
                strategyPlan.PlaceCard(planCard);
                ObjectPool.ReturnObject("Hand Pool", gameObject);
            }
            else
            {
                ObjectPool.ReturnObject("Hand Pool", gameObject);
                HandManager.Instance.AddCard(CardData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!canMove) return;

            base.OnDrag(eventData);
        }
    }
}
