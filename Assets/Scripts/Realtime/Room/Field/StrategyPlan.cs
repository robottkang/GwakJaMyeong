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
        private PlanCard firstPlacedPlanCard;

        public PlanCard PlacedPlanCardOnTop
        {
            get
            {
                if (placedCardObjects.Count == 0) return null;
                else return placedCardObjects.Peek().GetComponent<PlanCard>();
            }
        }
        public PlanCard FirstPlacedPlanCard => firstPlacedPlanCard;

        public void ClearStrategyPlan()
        {
            firstPlacedPlanCard = null;

            int count = placedCardObjects.Count;
            for (int i = 0; i < count; i++)
            {
                ObjectPool.ReturnObject("Card Pool", placedCardObjects.Pop());
            }
        }

        public void PlaceCard(GameObject obj)
        {
            if (obj == null) return;
            
            placedCardObjects.Push(obj);
            obj.transform.SetParent(transform);
            Vector3 position = transform.position +
                (placedCardObjects.Count - 1) * Vector3.forward + 0.1f * placedCardObjects.Count * Vector3.up;
            placedCardObjects.Peek().transform.position = position;
            placedCardObjects.Peek().transform.localRotation = new Quaternion(0f, 0f, 180f, 0f);

            PlanCard planCard;
            (planCard = obj.GetComponent<PlanCard>()).CurrentStrategyPlan = this;
            if (placedCardObjects.Count == 1)
                firstPlacedPlanCard = planCard;
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
