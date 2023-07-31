using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;

namespace Room
{
    public class StrategyPlan : MonoBehaviour
    {
        private Stack<GameObject> placedCardObjects = new(2);
        [SerializeField, ReadOnly]
        private PlanCard placedPlanCard;

        public PlanCard PlacedPlanCard
        {
            get
            {
                if (placedCardObjects.Count == 0) return null;
                else return placedCardObjects.Peek().GetComponent<PlanCard>();
            }
        }

        public void ClearStrategyPlan()
        {
            placedPlanCard = null;

            int count = placedCardObjects.Count;
            for (int i = 0; i < count; i++)
            {
                ObjectPool.ReturnObject("Card Pool", placedCardObjects.Pop());
            }
        }

        public void PlaceCard(GameObject planCard)
        {
            if (planCard == null) return;

            placedPlanCard.CurrentStrategyPlan = this;
            placedCardObjects.Push(planCard);

            Vector3 position = transform.position +
                (placedCardObjects.Count - 1) * Vector3.forward + 0.08f * placedCardObjects.Count * Vector3.up;
            placedCardObjects.Peek().transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, 0f));
        }

        public void TradeCard(StrategyPlan target)
        {
            var targetCards = new Queue<GameObject>(target.placedCardObjects);
            var thisCards = new Queue<GameObject>(this.placedCardObjects);
            this.placedCardObjects.Clear();
            target.placedCardObjects.Clear();
            while (thisCards.Count > 0)
                target.PlaceCard(thisCards.Dequeue());
            while (targetCards.Count > 0)
                this.PlaceCard(targetCards.Dequeue());
        }
    }
}
